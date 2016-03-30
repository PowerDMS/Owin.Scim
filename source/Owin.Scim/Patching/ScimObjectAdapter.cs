namespace Owin.Scim.Patching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Configuration;

    using Exceptions;

    using Extensions;

    using Helpers;

    using Model;

    using NContext.Common;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using Operations;

    public class ScimObjectAdapter<T> : IObjectAdapter where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ScimObjectAdapter{T}"/>.
        /// </summary>
        /// <param name="contractResolver">The <see cref="IContractResolver"/>.</param>
        public ScimObjectAdapter(IContractResolver contractResolver)
        {
            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            ContractResolver = contractResolver;
        }

        /// <summary>
        /// Gets or sets the <see cref="IContractResolver"/>.
        /// </summary>
        public IContractResolver ContractResolver { get; private set; }

        /// <summary>
        /// The "add" operation is used to add a new attribute value to an existing resource. 
        /// The operation MUST contain a "value" member whose content specifies 
        /// the value to be added.
        /// 
        /// The value MAY be a quoted value, or it may be 
        /// a JSON object containing the sub-attributes of the complex attribute 
        /// specified in the operation's "path".
        /// </summary>
        /// <param name="operation">The add operation.</param>
        /// <param name="objectToApplyTo">Object to apply the operation to.</param>
        public IEnumerable<PatchOperationResult> Add(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

            /*
                With SCIM 2.0, path is only required for the remove operation:
            
                   o  If omitted, the target location is assumed to be the resource
                      itself.  The "value" parameter contains a set of attributes to be
                      added to the resource.
            */

            // Below we handle the null path by reflecting the "value" object;
            // treating each property on the object as a patch operation.
            if (string.IsNullOrWhiteSpace(operation.Path))
            {
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(operation.Value.ToString());
                foreach (var kvp in resourcePatch)
                {
                    operations.AddRange(AddInternal(kvp.Key, kvp.Value, objectToApplyTo, operation));
                }

                return operations;
            }

            return AddInternal(operation.Path, operation.Value, objectToApplyTo, operation);
        }
        
        private IEnumerable<PatchOperationResult> AddInternal(
            string path, 
            object value,
            object objectToApplyTo,
            Operation operation)
        {
            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            // ScimPatchObjectAnalysis.cs will handle resolving the actual 
            // path to object members and parsing any filters.
            var patchAnalysis = new ScimPatchObjectAnalysis(
                objectToApplyTo,
                path, 
                ContractResolver,
                operation);

            if (patchAnalysis.ErrorType != null)
            {
                throw new ScimPatchException(
                    patchAnalysis.ErrorType,
                    operation);
            }
            
            var operations = new List<PatchOperationResult>();
            foreach (var patchMember in patchAnalysis.PatchMembers)
            {
                if (patchAnalysis.UseDynamicLogic)
                    throw new NotSupportedException(); // TODO: (DG) Add support if needed.

                operations.AddRange(AddNonDynamic(value, operation, patchMember));
            }

            return operations;
        }

        private IEnumerable<PatchOperationResult> AddNonDynamic(object value, Operation operation, PatchMember patchMember)
        {
            var patchProperty = patchMember.JsonPatchProperty;

            EnforceMutability(operation, patchProperty);

            var instanceValue = patchProperty.Property.ValueProvider.GetValue(patchProperty.Parent);
            if (instanceValue == null || !patchProperty.Property.PropertyType.IsNonStringEnumerable())
            {
                /*
                    Here we are going to be setting or replacing a current value:
                        o  If the target location does not exist, the attribute and value are added.
                            (instanceValue == null)
                        o  If the target location specifies a single-valued attribute, the existing value is replaced. 
                            (!patchProperty.Property.PropertyType.IsNonStringEnumerable())
                        o  If the target location exists, the value is replaced.
                            (!patchProperty.Property.PropertyType.IsNonStringEnumerable())
                */

                var conversionResultTuple = ConvertToActualType(
                    patchProperty.Property.PropertyType,
                    value);

                if (!conversionResultTuple.CanBeConverted)
                {
                    throw new ScimPatchException(
                        ScimErrorType.InvalidValue,
                        operation);
                }

                if (!patchProperty.Property.Writable)
                {
                    throw new Exception(); // TODO: (DG) This is int server error.
                }
                
                patchProperty.Property.ValueProvider.SetValue(
                    patchProperty.Parent,
                    conversionResultTuple.ConvertedInstance);

                return new[]
                {
                    new PatchOperationResult(
                        patchProperty,
                        instanceValue,
                        conversionResultTuple.ConvertedInstance)
                };
            }
            
            /*
                    o  If the target location already contains the value specified, no
                        changes SHOULD be made to the resource, and a success response
                        SHOULD be returned.  Unless other operations change the resource,
                        this operation SHALL NOT change the modify timestamp of the
                        resource.
            */
                
            // The first case below handles the following SCIM rule:
            // o  If the target location specifies a complex attribute, a set of
            //    sub - attributes SHALL be specified in the "value" parameter.
            if (patchMember.Target != null &&
                (patchMember.Target is MultiValuedAttribute ||
                 !patchMember.Target.GetType().IsTerminalObject()))
            {
                // value should be an object composed of sub-attributes of the parent, a MultiValuedAttribute
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(operation.Value.ToString());
                var jsonContract = (JsonObjectContract)ContractResolver.ResolveContract(patchMember.Target.GetType());
                foreach (var kvp in resourcePatch)
                {
                    var attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(kvp.Key);
                    var pp = new JsonPatchProperty(attemptedProperty, patchMember.Target);
                    var patch = new PatchMember(kvp.Key, pp);
                    
                    EnforceMutability(operation, pp);

                    operations.AddRange(
                        AddNonDynamic(
                            kvp.Value, 
                            new Operation(operation.OperationType, kvp.Key, kvp.Value), 
                            patch));
                }
                    
                return operations;
            }
            
            // Here we are going to be modifying an existing enumerable:
            // The second case handles the following SCIM rule:
            // o If the target location specifies a multi-valued attribute, a new
            //   value is added to the attribute.
            var genericTypeOfArray = patchProperty.Property.PropertyType.GetEnumerableType();
            var conversionResult = ConvertToActualType(genericTypeOfArray, value);
            if (!conversionResult.CanBeConverted)
            {
                throw new ScimPatchException(ScimErrorType.InvalidValue, operation);
            }
            
            var array = CreateGenericListFromObject(instanceValue);
            array.AddPossibleRange(conversionResult.ConvertedInstance);

            patchProperty.Property.ValueProvider.SetValue(
                patchProperty.Parent,
                array);

            return new[]
            {
                new PatchOperationResult(
                    patchProperty,
                    instanceValue,
                    array)
            };
        }
        
        /// <summary>
        /// The "remove" operation removes the value at the target location 
        /// specified by the required attribute "path". 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="objectToApplyTo"></param>
        /// <returns></returns>
        public IEnumerable<PatchOperationResult> Remove(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

            // o  If "path" is unspecified, the operation fails with HTTP status
            //    code 400 and a "scimType" error code of "noTarget".
            if (string.IsNullOrWhiteSpace(operation.Path))
                throw new ScimPatchException(ScimErrorType.NoTarget, operation);

            var treeAnalysisResult = new ScimPatchObjectAnalysis(
                objectToApplyTo,
                operation.Path,
                ContractResolver,
                operation);

            if (treeAnalysisResult.ErrorType != null)
            {
                throw new ScimPatchException(
                    treeAnalysisResult.ErrorType,
                    operation);
            }

            var operations = new List<PatchOperationResult>();
            foreach (var patchMember in treeAnalysisResult.PatchMembers)
            {
                if (treeAnalysisResult.UseDynamicLogic)
                    throw new NotSupportedException(); // TODO: (DG) If actually needed.
                else
                    operations.Add(RemoveNonDynamic(operation, patchMember));
            }

            return operations;
        }

        private PatchOperationResult RemoveNonDynamic(Operation operation, PatchMember patchMember)
        {
            var patchProperty = patchMember.JsonPatchProperty;
            if (!patchProperty.Property.Writable)
            {
                throw new Exception(); // TODO: (DG) This is int server error.
            }

            EnforceMutability(operation, patchProperty);

            var defaultValue = patchProperty.Property.PropertyType.GetDefaultValue();
            var instanceValue = patchProperty.Property.ValueProvider.GetValue(patchProperty.Parent);
            if (instanceValue == defaultValue)
            {
                return new PatchOperationResult(patchProperty, instanceValue, defaultValue);
            }

            // Here we are going to be setting or replacing a current value:
            // o  If the target location is a single-value attribute, the attribute
            //    and its associated value is removed, and the attribute SHALL be
            //    considered unassigned.
            if (!patchProperty.Property.PropertyType.IsNonStringEnumerable())
            {
                patchProperty.Property.ValueProvider.SetValue(
                    patchProperty.Parent,
                    defaultValue);

                return new PatchOperationResult(patchProperty, instanceValue, defaultValue);
            }

            // Here we are going to be modifying an existing enumerable:
            // The first case below handles the following SCIM rule:
            // o  If the target location is a multi-valued attribute and a complex
            //    filter is specified comparing a "value", the values matched by the
            //    filter are removed.  If no other values remain after removal of
            //    the selected values, the multi-valued attribute SHALL be
            //    considered unassigned.
            // o  If the target location is a complex multi-valued attribute and a
            //    complex filter is specified based on the attribute's
            //    sub-attributes, the matching records are removed. Sub-attributes
            //    whose values have been removed SHALL be considered unassigned. If
            //    the complex multi-valued attribute has no remaining records, the
            //    attribute SHALL be considered unassigned.
            if (patchMember.Target != null &&
                (patchMember.Target is MultiValuedAttribute ||
                 !patchMember.Target.GetType().IsTerminalObject()))
            {
                var array = CreateGenericListFromObject(instanceValue);
                array.Remove(patchMember.Target);

                var newValue = array.Count == 0
                    ? defaultValue
                    : array;

                patchProperty.Property.ValueProvider.SetValue(
                    patchProperty.Parent,
                    newValue);

                return new PatchOperationResult(patchProperty, instanceValue, newValue);
            }

            // The second case handles the following SCIM rule:
            // o  If the target location is a multi-valued attribute and no filter
            //    is specified, the attribute and all values are removed, and the
            //    attribute SHALL be considered unassigned.
            patchProperty.Property.ValueProvider.SetValue(
                patchProperty.Parent,
                defaultValue);

            return new PatchOperationResult(patchProperty, instanceValue, defaultValue);
        }

        public IEnumerable<PatchOperationResult> Replace(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (objectToApplyTo == null)
                throw new ArgumentNullException(nameof(objectToApplyTo));

            /*
                With SCIM 2.0, path is only required for the remove operation:
            
                   o  If omitted, the target location is assumed to be the resource
                      itself.  The "value" parameter contains a set of attributes to be
                      added to the resource.
            */

            // Below we handle the null path by reflecting the "value" object;
            // treating each property on the object as a patch operation.
            if (string.IsNullOrWhiteSpace(operation.Path))
            {
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(operation.Value.ToString());
                foreach (var kvp in resourcePatch)
                {
                    operations.AddRange(ReplaceInternal(kvp.Key, kvp.Value, objectToApplyTo, operation));
                }

                return operations;
            }

            return ReplaceInternal(operation.Path, operation.Value, objectToApplyTo, operation);
        }

        private IEnumerable<PatchOperationResult> ReplaceInternal(string path, object value, object objectToApplyTo, Operation operation)
        {
            var treeAnalysisResult = new ScimPatchObjectAnalysis(
                objectToApplyTo,
                path,
                ContractResolver,
                operation);

            if (treeAnalysisResult.ErrorType != null)
            {
                throw new ScimPatchException(
                    treeAnalysisResult.ErrorType,
                    operation);
            }

            var operations = new List<PatchOperationResult>();
            foreach (var patchMember in treeAnalysisResult.PatchMembers)
            {
                if (treeAnalysisResult.UseDynamicLogic)
                    throw new NotSupportedException(); // TODO: (DG) Added support if needed.
                else
                    operations.AddRange(ReplaceNonDynamic(value, operation, patchMember));
            }

            return operations;
        }

        private IEnumerable<PatchOperationResult> ReplaceNonDynamic(object value, Operation operation, PatchMember patchMember)
        {
            var patchProperty = patchMember.JsonPatchProperty;
            if (!patchProperty.Property.Writable)
            {
                throw new Exception(); // TODO: (DG) This is int server error.
            }

            EnforceMutability(operation, patchProperty);

            var instanceValue = patchProperty.Property.ValueProvider.GetValue(patchProperty.Parent);
            // if the instance's property value is null or it is not an enumerable type
            if (instanceValue == null || !patchProperty.Property.PropertyType.IsNonStringEnumerable())
            {
                /*
                    Here we are going to be setting or replacing a current value:
                        o  If the target location is a single-value attribute, the attributes
                           value is replaced.
                        o  If the target location path specifies an attribute that does not
                           exist, the service provider SHALL treat the operation as an "add".
                           (instanceValue == null)
                        o  If the target location is a complex multi-valued attribute with a
                           value selection filter ("valuePath") and a specific sub-attribute
                           (e.g., "addresses[type eq "work"].streetAddress"), the matching
                           sub-attribute of all matching records is replaced.
                           (!patchProperty.Property.PropertyType.IsNonStringEnumerable())
                */
                
                var conversionResultTuple = ConvertToActualType(
                    patchProperty.Property.PropertyType,
                    value,
                    instanceValue);

                if (!conversionResultTuple.CanBeConverted)
                {
                    throw new ScimPatchException(
                        ScimErrorType.InvalidValue,
                        operation);
                }

                patchProperty.Property.ValueProvider.SetValue(
                    patchProperty.Parent,
                    conversionResultTuple.ConvertedInstance);

                return new[]
                {
                    new PatchOperationResult(
                        patchProperty,
                        instanceValue,
                        conversionResultTuple.ConvertedInstance)
                };
            }
            
            // Here we are going to be modifying a complex object:
            // The first case below handles the following SCIM rules:
            // o  If the target location is a multi-valued attribute and a value
            //    selection("valuePath") filter is specified that matches one or
            //    more values of the multi-valued attribute, then all matching
            //    record values SHALL be replaced.
            // o  If the target location specifies a complex attribute, a set of
            //    sub-attributes SHALL be specified in the "value" parameter, which
            //    replaces any existing values or adds where an attribute did not
            //    previously exist.  Sub-attributes that are not specified in the
            //    "value" parameter are left unchanged.
            if (patchMember.Target != null &&
                (patchMember.Target is MultiValuedAttribute || 
                 !patchMember.Target.GetType().IsTerminalObject()))
            {
                // if value is null, we're setting the MultiValuedAttribute to null
                if (value == null)
                {
                    return new[]
                    {
                        RemoveNonDynamic(operation, patchMember)
                    };
                }

                // value should be an object composed of sub-attributes of the parent, a MultiValuedAttribute
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(value.ToString());
                var jsonContract = (JsonObjectContract)ContractResolver.ResolveContract(patchMember.Target.GetType());
                foreach (var kvp in resourcePatch)
                {
                    var attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(kvp.Key);
                    var pp = new JsonPatchProperty(attemptedProperty, patchMember.Target);

                    EnforceMutability(operation, pp);

                    var patch = new PatchMember(kvp.Key, pp);
                    operations.AddRange(
                        AddNonDynamic(
                            kvp.Value,
                            new Operation(operation.OperationType, kvp.Key, kvp.Value),
                            patch));
                }

                return operations;
            }

            // The second case handles the following SCIM rule:
            // o  If the target location is a multi-valued attribute and no filter
            //    is specified, the attribute and all values are replaced.
            var genericTypeOfArray = patchProperty.Property.PropertyType.GetEnumerableType();
            var conversionResult = ConvertToActualType(genericTypeOfArray, value);
            if (!conversionResult.CanBeConverted)
            {
                throw new ScimPatchException(ScimErrorType.InvalidValue, operation);
            }
            
            var array = CreateGenericListFromObject(instanceValue, false);
            array.AddPossibleRange(conversionResult.ConvertedInstance);

            patchProperty.Property.ValueProvider.SetValue(
                patchProperty.Parent,
                array);

            return new[]
            {
                new PatchOperationResult(
                    patchProperty,
                    instanceValue,
                    array)
            };
        }

        private static void EnforceMutability(Operation operation, JsonPatchProperty patchProperty)
        {
            IScimTypeAttributeDefinition attrDefinition;
            var typeDefinition = ScimServerConfiguration.GetScimTypeDefinition(patchProperty.Parent.GetType());
            var propertyInfo = patchProperty.Property.DeclaringType.GetProperty(patchProperty.Property.UnderlyingName);

            // if attribute is readOnly OR (immutable and isReferenceType and current value is not null)
            if (typeDefinition != null &&
                typeDefinition.AttributeDefinitions.TryGetValue(propertyInfo, out attrDefinition) &&
                (attrDefinition.Mutability == Mutability.ReadOnly || 
                 (attrDefinition.Mutability == Mutability.Immutable &&
                  !attrDefinition.AttributeDescriptor.PropertyType.IsValueType &&
                  patchProperty.Property.ValueProvider.GetValue(patchProperty.Parent) != null)))
            {
                throw new ScimPatchException(
                    ScimErrorType.Mutability,
                    operation);
            }
        }

        private ConversionResult ConvertToActualType(Type propertyType, object value, object instanceValue = null)
        {
            if (value == null)
            {
                return new ConversionResult(true, propertyType.GetDefaultValue());
            }

            try
            {
                if (!propertyType.IsTerminalObject() && instanceValue != null)
                {
                    JsonConvert.PopulateObject(value.ToString(), instanceValue);
                    return new ConversionResult(true, instanceValue);
                }

                var o = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), propertyType);
                return new ConversionResult(true, o);
            }
            catch (Exception)
            {
                return new ConversionResult(false, null);
            }
        }

        private IList CreateGenericListFromObject(object instanceValue, bool initializeWithInstanceValue = true)
        {
            if (instanceValue == null) return null;

            var enumerableInterface = instanceValue.GetType().GetEnumerableType();
            if (enumerableInterface == null) return null;

            if (instanceValue is IList && initializeWithInstanceValue)
            {
                return (IList) instanceValue;
            }

            var listArgumentType = enumerableInterface.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(listArgumentType);
            return initializeWithInstanceValue
                ? (IList)listType.CreateInstance(instanceValue)
                : (IList)listType.CreateInstance();
        }
    }
}