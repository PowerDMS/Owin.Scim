namespace Owin.Scim.Patching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }
            
            /*
            With SCIM 2.0, path is only required for the remove operation:
            
                   o  If omitted, the target location is assumed to be the resource
                      itself.  The "value" parameter contains a set of attributes to be
                      added to the resource.

            PATCH /Users/2819c223-7f76-453a-919d-413861904646
            Host: example.com
            Accept: application/scim+json
            Content-Type: application/scim+json
            Authorization: Bearer h480djs93hd8
            If-Match: W/"a330bc54f0671c9"

            {
                "schemas":
                ["urn:ietf:params:scim:api:messages:2.0:PatchOp"],
                "Operations":[{
                "op":"add",
                "value":{
                    "emails":[
                    {
                        "value":"babs@jensen.org",
                        "type":"home"
                    }
                    ],
                    "nickname":"Babs"
                }]
            }

            In the above example, an additional value is added to the
            multi-valued attribute "emails".  The second attribute, "nickname",
            is added to the User resource.  If the resource already had an
            existing "nickname", the value is replaced per the processing rules
            above for single-valued attributes.
            */

            // Below we handle the null path by reflecting the "value" object;
            // treating each property on the object as a patch operation.
            if (string.IsNullOrWhiteSpace(operation.Path))
            {
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(operation.Value.ToString());
                foreach (var kvp in resourcePatch)
                {
                    operations.AddRange(Add(kvp.Key, kvp.Value, objectToApplyTo, operation));
                }

                return operations;
            }

            return Add(operation.Path, operation.Value, objectToApplyTo, operation);
        }
        
        private IEnumerable<PatchOperationResult> Add(
            string path,
            object value,
            object objectToApplyTo,
            Operation operation)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            /* 
                ScimObjectTreeAnalysisResult.cs will handle resolving the actual 
                path and parsing any filters including,

                Examples:
                "path":"members"
                "path":"name.familyName"
                "path":"addresses[type eq \"work\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName"
            */
            var treeAnalysisResult = new ScimObjectTreeAnalysisResult(
                objectToApplyTo,
                path, 
                ContractResolver);

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
                    operations.AddRange(AddDynamic(value, treeAnalysisResult, patchMember));
                else
                    operations.AddRange(AddNonDynamic(value, operation, patchMember));
            }

            return operations;
        }

        private IEnumerable<PatchOperationResult> AddNonDynamic(object value, Operation operation, PatchMember patchMember)
        {
            var patchProperty = patchMember.JsonPatchProperty;
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

            // Here we are going to be modifying an existing enumerable:

            /*
                TODO: (DG) Handle case when:

                    o  If the target location already contains the value specified, no
                        changes SHOULD be made to the resource, and a success response
                        SHOULD be returned.  Unless other operations change the resource,
                        this operation SHALL NOT change the modify timestamp of the
                        resource.
            */
                
            // The first case below handles the following SCIM rule:
            // o  If the target location specifies a complex attribute, a set of
            //    sub - attributes SHALL be specified in the "value" parameter.
            if (patchMember.Target is MultiValuedAttribute)
            {
                // value should be an object composed of sub-attributes of the parent, a MultiValuedAttribute
                var operations = new List<PatchOperationResult>();
                var resourcePatch = JObject.Parse(operation.Value.ToString());
                var jsonContract = (JsonObjectContract)ContractResolver.ResolveContract(patchMember.Target.GetType());
                foreach (var kvp in resourcePatch)
                {
                    var attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(kvp.Key);
                    var patch = new PatchMember(kvp.Key, new JsonPatchProperty(attemptedProperty, patchMember.Target));
                    operations.AddRange(
                        AddNonDynamic(
                            kvp.Value, 
                            new Operation(operation.Operation, kvp.Key, kvp.Value), 
                            patch));
                }
                    
                return operations;
            }

            // The second case handles the following SCIM rule:
            // o If the target location specifies a multi-valued attribute, a new
            //   value is added to the attribute.
            var genericTypeOfArray = patchProperty.Property.PropertyType.GetEnumerableType();
            var conversionResult = ConvertToActualType(genericTypeOfArray, value);
            if (!conversionResult.CanBeConverted)
            {
                throw new ScimPatchException(ScimErrorType.InvalidValue, operation);
            }

            if (!patchProperty.Property.Readable)
            {
                throw new Exception(); // TODO: (DG) This is int server error.
            }

            var listType = typeof (List<>).MakeGenericType(genericTypeOfArray.GetGenericArguments()[0]);
            var array = (IList) listType.CreateInstance(instanceValue);
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

        private IEnumerable<PatchOperationResult> AddDynamic(object value, ScimObjectTreeAnalysisResult treeAnalysisResult, PatchMember patchMember)
        {
            // TODO: (DG) NOT SURE IF THIS IS EVER NEEDED!
            /*
                    o  If the target location specifies an attribute that does not exist
                        (has no value), the attribute is added with the new value.
            */
            // possibly with resource extensions like enterpriseuser support

            var container = treeAnalysisResult.Container;
            if (container.ContainsCaseInsensitiveKey(patchMember.PropertyPathInParent))
            {
                // Existing property.  
                // If it's not an array, we need to check if the value fits the property type
                // 
                // If it's an array, we need to check if the value fits in that array type,
                // and add it at the correct position (if allowed).
                if (patchMember.JsonPatchProperty.Property.PropertyType.IsNonStringEnumerable())
                {
                    // get the actual type
                    var propertyValue =
                        container.GetValueForCaseInsensitiveKey(patchMember.PropertyPathInParent);
                    var typeOfPathProperty = propertyValue.GetType();

                    if (!typeOfPathProperty.IsNonStringEnumerable())
                    {
                        throw new Exception();
                    }

                    // now, get the generic type of the enumerable
                    var genericTypeOfArray = typeOfPathProperty.GetEnumerableType();
                    var conversionResult = ConvertToActualType(genericTypeOfArray, value);
                    if (!conversionResult.CanBeConverted)
                    {
                        throw new Exception();
                    }

                    // get value (it can be cast, we just checked that) 
                    var array = treeAnalysisResult.Container.GetValueForCaseInsensitiveKey(
                        patchMember.PropertyPathInParent) as IList;

                    array.Add(conversionResult.ConvertedInstance);
                    treeAnalysisResult.Container.SetValueForCaseInsensitiveKey(
                        patchMember.PropertyPathInParent, array);
                }
                else
                {
                    // get the actual type
                    var typeOfPathProperty = treeAnalysisResult.Container
                        .GetValueForCaseInsensitiveKey(patchMember.PropertyPathInParent).GetType();

                    // can the value be converted to the actual type?
                    var conversionResult = ConvertToActualType(typeOfPathProperty, value);
                    if (conversionResult.CanBeConverted)
                    {
                        treeAnalysisResult.Container.SetValueForCaseInsensitiveKey(
                            patchMember.PropertyPathInParent,
                            conversionResult.ConvertedInstance);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            else
            {
                // New property - add it.  
                treeAnalysisResult.Container.Add(patchMember.PropertyPathInParent, value);
            }

            return null;
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

            var treeAnalysisResult = new ScimObjectTreeAnalysisResult(
                objectToApplyTo,
                operation.Path,
                ContractResolver);

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
                    operations.Add(RemoveNonDynamic(patchMember));
            }

            return operations;
        }

        private PatchOperationResult RemoveNonDynamic(PatchMember patchMember)
        {
            var patchProperty = patchMember.JsonPatchProperty;
            if (!patchProperty.Property.Writable)
            {
                throw new Exception(); // TODO: (DG) This is int server error.
            }

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
            //    sub - attributes, the matching records are removed. Sub-attributes
            //    whose values have been removed SHALL be considered unassigned. If
            //    the complex multi-valued attribute has no remaining records, the
            //    attribute SHALL be considered unassigned.
            if (patchMember.Target is MultiValuedAttribute)
            {
                var listType = typeof(List<>).MakeGenericType(patchMember.Target.GetType());
                var array = (IList)listType.CreateInstance(instanceValue);
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
            throw new NotImplementedException();
        }
        
        private ConversionResult ConvertToActualType(Type propertyType, object value)
        {
            try
            {
                var o = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value), propertyType);

                return new ConversionResult(true, o);
            }
            catch (Exception)
            {
                return new ConversionResult(false, null);
            }
        }
    }
}