// Any comments, input: @KevinDockx
// Any issues, requests: https://github.com/KevinDockx/JsonPatch
//
// Enjoy :-)

namespace Owin.Scim.Patching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using Adapters;

    using Exceptions;

    using Extensions;

    using Helpers;

    using NContext.Common;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    using Operations;

    using Properties;

    public class ScimObjectAdapter<T> : IObjectAdapter where T : class
    {

        /// <summary>
        /// Initializes a new instance of <see cref="ObjectAdapter"/>.
        /// </summary>
        /// <param name="contractResolver">The <see cref="IContractResolver"/>.</param>
        /// <param name="logErrorAction">The <see cref="Action"/> for logging <see cref="JsonPatchError"/>.</param>
        public ScimObjectAdapter(
            IContractResolver contractResolver,
            Action<JsonPatchError> logErrorAction)
        {
            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            ContractResolver = contractResolver;
            LogErrorAction = logErrorAction;
        }

        /// <summary>
        /// Gets or sets the <see cref="IContractResolver"/>.
        /// </summary>
        public IContractResolver ContractResolver { get; }

        /// <summary>
        /// Action for logging <see cref="JsonPatchError"/>.
        /// </summary>
        public Action<JsonPatchError> LogErrorAction { get; }

        /// <summary>
        /// The "add" operation performs one of the following functions,
        /// depending upon what the target location references:
        ///
        /// o  If the target location specifies an array index, a new value is
        ///    inserted into the array at the specified index.
        ///
        /// o  If the target location specifies an object member that does not
        ///    already exist, a new member is added to the object.
        ///
        /// o  If the target location specifies an object member that does exist,
        ///    that member's value is replaced.
        ///
        /// The operation object MUST contain a "value" member whose content
        /// specifies the value to be added.
        ///
        /// For example:
        ///
        /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
        ///
        /// When the operation is applied, the target location MUST reference one
        /// of:
        ///
        /// o  The root of the target document - whereupon the specified value
        ///    becomes the entire content of the target document.
        ///
        /// o  A member to add to an existing object - whereupon the supplied
        ///    value is added to that object at the indicated location.  If the
        ///    member already exists, it is replaced by the specified value.
        ///
        /// o  An element to add to an existing array - whereupon the supplied
        ///    value is added to the array at the indicated location.  Any
        ///    elements at or above the specified index are shifted one position
        ///    to the right.  The specified index MUST NOT be greater than the
        ///    number of elements in the array.  If the "-" character is used to
        ///    index the end of the array (see [RFC6901]), this has the effect of
        ///    appending the value to the array.
        ///
        /// Because this operation is designed to add to existing objects and
        /// arrays, its target location will often not exist.  Although the
        /// pointer's error handling algorithm will thus be invoked, this
        /// specification defines the error handling behavior for "add" pointers
        /// to ignore that error and add the value as specified.
        ///
        /// However, the object itself or an array containing it does need to
        /// exist, and it remains an error for that not to be the case.  For
        /// example, an "add" with a target location of "/a/b" starting with this
        /// document:
        ///
        /// { "a": { "foo": 1 } }
        ///
        /// is not an error, because "a" exists, and "b" will be added to its
        /// value.  It is an error in this document:
        ///
        /// { "q": { "bar": 2 } }
        ///
        /// because "a" does not exist.
        /// </summary>
        /// <param name="operation">The add operation.</param>
        /// <param name="objectToApplyTo">Object to apply the operation to.</param>
        public void Add(Operation operation, object objectToApplyTo)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            Add(operation.path, operation.value, objectToApplyTo, operation);
        }
        
        /// <summary>
        /// Add is used by various operations (eg: add, copy, ...), yet through different operations;
        /// This method allows code reuse yet reporting the correct operation on error
        /// </summary>
        private void Add(
            string path,
            object value,
            object objectToApplyTo,
            Operation operationToReport)
        {
            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (operationToReport == null)
            {
                throw new ArgumentNullException(nameof(operationToReport));
            }

            #region Scim Edit

            // With SCIM 2.0, path is only required for the remove operation:

            // If omitted (path), the target location is assumed to be the resource
            // itself. The "value" parameter contains a set of attributes to be
            // added to the resource.

            /*
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

            if (string.IsNullOrWhiteSpace(path))
            {
                JObject resourcePatch;
                try
                {
                    resourcePatch = JObject.Parse(value.ToString());

                    foreach (var kvp in resourcePatch)
                    {
                        Add("/" + kvp.Key, kvp.Value, objectToApplyTo, operationToReport);
                    }

                    return;
                }
                catch (Exception)
                {
                    // TODO: (DG) FINISH
                    throw new JsonPatchException(
                        new JsonPatchError(
                            objectToApplyTo,
                            operationToReport,
                            ""));
                }
            }

            #endregion

            // first up: if the path ends in a numeric value, we're inserting in a list and
            // that value represents the position; if the path ends in "-", we're appending
            // to the list.

            // get path result
            var pathResult = GetActualPropertyPath(
                path,
                objectToApplyTo,
                operationToReport);

            if (pathResult == null)
            {
                return;
            }
            var actualPathToProperty = pathResult.PathToProperty;

            var treeAnalysisResult = new ObjectTreeAnalysisResult(
                objectToApplyTo,
                actualPathToProperty,
                ContractResolver);
            
            if (!treeAnalysisResult.IsValidPathForAdd)
            {
                LogError(new JsonPatchError(
                    objectToApplyTo,
                    operationToReport,
                    Resources.FormatPropertyCannotBeAdded(path)));
                return;
            }
            
            if (!treeAnalysisResult.UseDynamicLogic)
            {
                var patchProperty = treeAnalysisResult.JsonPatchProperty;

                // If it's null or not an array, replace. Else, add to the array.
                var instanceValue = patchProperty.Property.ValueProvider.GetValue(patchProperty.Parent);
                if (instanceValue != null && patchProperty.Property.PropertyType.IsNonStringEnumerable())
                {
                    var genericTypeOfArray = patchProperty.Property.PropertyType.GetEnumerableType();
                    var conversionResult = ConvertToActualType(genericTypeOfArray, value);
                    if (!conversionResult.CanBeConverted)
                    {
                        LogError(new JsonPatchError(
                            objectToApplyTo,
                            operationToReport,
                            Resources.FormatInvalidValueForProperty(conversionResult.ConvertedInstance, path)));
                        return;
                    }

                    if (!patchProperty.Property.Readable)
                    {
                        LogError(new JsonPatchError(
                            objectToApplyTo,
                            operationToReport,
                            Resources.FormatCannotReadProperty(path)));
                        return;
                    }

                    var listType = typeof (List<>).MakeGenericType(genericTypeOfArray.GetGenericArguments()[0]);
                    var array = (IList) listType.CreateInstance(instanceValue);
                    array.AddPossibleRange(conversionResult.ConvertedInstance);

                    patchProperty.Property.ValueProvider.SetValue(
                        patchProperty.Parent,
                        array);
                }
                else
                {
                    var conversionResultTuple = ConvertToActualType(
                        patchProperty.Property.PropertyType,
                        value);

                    if (!conversionResultTuple.CanBeConverted)
                    {
                        LogError(new JsonPatchError(
                            objectToApplyTo,
                            operationToReport,
                            Resources.FormatInvalidValueForProperty(value, path)));
                        return;
                    }

                    if (!patchProperty.Property.Writable)
                    {
                        LogError(new JsonPatchError(
                            objectToApplyTo,
                            operationToReport,
                            Resources.FormatCannotUpdateProperty(path)));
                        return;
                    }

                    patchProperty.Property.ValueProvider.SetValue(
                        patchProperty.Parent,
                        conversionResultTuple.ConvertedInstance);
                }
            }
            else
            {
                var container = treeAnalysisResult.Container;
                if (container.ContainsCaseInsensitiveKey(treeAnalysisResult.PropertyPathInParent))
                {
                    // Existing property.  
                    // If it's not an array, we need to check if the value fits the property type
                    // 
                    // If it's an array, we need to check if the value fits in that array type,
                    // and add it at the correct position (if allowed).
                    if (treeAnalysisResult.JsonPatchProperty.Property.PropertyType.IsNonStringEnumerable())
                    {
                        // get the actual type
                        var propertyValue =
                            container.GetValueForCaseInsensitiveKey(treeAnalysisResult.PropertyPathInParent);
                        var typeOfPathProperty = propertyValue.GetType();

                        if (!typeOfPathProperty.IsNonStringEnumerable())
                        {
                            LogError(new JsonPatchError(
                                objectToApplyTo,
                                operationToReport,
                                Resources.FormatInvalidIndexForArrayProperty(operationToReport.op, path)));
                            return;
                        }

                        // now, get the generic type of the enumerable
                        var genericTypeOfArray = typeOfPathProperty.GetEnumerableType();
                        var conversionResult = ConvertToActualType(genericTypeOfArray, value);
                        if (!conversionResult.CanBeConverted)
                        {
                            LogError(new JsonPatchError(
                                objectToApplyTo,
                                operationToReport,
                                Resources.FormatInvalidValueForProperty(value, path)));
                            return;
                        }

                        // get value (it can be cast, we just checked that) 
                        var array = treeAnalysisResult.Container.GetValueForCaseInsensitiveKey(
                            treeAnalysisResult.PropertyPathInParent) as IList;

                        array.Add(conversionResult.ConvertedInstance);
                        treeAnalysisResult.Container.SetValueForCaseInsensitiveKey(
                            treeAnalysisResult.PropertyPathInParent, array);
                    }
                    else
                    {
                        // get the actual type
                        var typeOfPathProperty = treeAnalysisResult.Container
                            .GetValueForCaseInsensitiveKey(treeAnalysisResult.PropertyPathInParent).GetType();

                        // can the value be converted to the actual type?
                        var conversionResult = ConvertToActualType(typeOfPathProperty, value);
                        if (conversionResult.CanBeConverted)
                        {
                            treeAnalysisResult.Container.SetValueForCaseInsensitiveKey(
                                treeAnalysisResult.PropertyPathInParent,
                                conversionResult.ConvertedInstance);
                        }
                        else
                        {
                            LogError(new JsonPatchError(
                                objectToApplyTo,
                                operationToReport,
                                Resources.FormatInvalidValueForProperty(conversionResult.ConvertedInstance, path)));
                            return;
                        }
                    }
                }
                else
                {
                    // New property - add it.  
                    treeAnalysisResult.Container.Add(treeAnalysisResult.PropertyPathInParent, value);
                }
            }
        }

        public void Copy(Operation operation, object objectToApplyTo)
        {
            throw new NotSupportedException();
        }

        public void Move(Operation operation, object objectToApplyTo)
        {
            throw new NotSupportedException();
        }

        public void Remove(Operation operation, object objectToApplyTo)
        {
            throw new NotImplementedException();
        }

        public void Replace(Operation operation, object objectToApplyTo)
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

        private void LogError(JsonPatchError jsonPatchError)
        {
            if (LogErrorAction != null)
            {
                LogErrorAction(jsonPatchError);
            }
            else
            {
                throw new JsonPatchException(jsonPatchError);
            }
        }

        private ActualPropertyPathResult GetActualPropertyPath(
            string propertyPath,
            object objectToApplyTo,
            Operation operationToReport)
        {
            if (propertyPath == null)
            {
                throw new ArgumentNullException(nameof(propertyPath));
            }

            if (objectToApplyTo == null)
            {
                throw new ArgumentNullException(nameof(objectToApplyTo));
            }

            if (operationToReport == null)
            {
                throw new ArgumentNullException(nameof(operationToReport));
            }

            if (propertyPath.Contains("["))
            {
                // TOOD: (DG) Support filters
                throw new NotSupportedException("Filters are currently not supported.");
            }

            return new ActualPropertyPathResult(-1, propertyPath, true);
        }
    }
}