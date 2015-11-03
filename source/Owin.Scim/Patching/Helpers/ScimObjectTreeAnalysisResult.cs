// This code borrows from Microsoft's (Microsoft.AspNet.JsonPatch) ObjectTreeAnalysisResult.cs
// This has been extended and heavily modified to add support for IEnumerable types (not restricted to IList)
// as well as support for SCIM query filters.

// Copyright (c) PowerDMS Inc.
// Licensed under the MIT License.

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Owin.Scim.Patching.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Antlr;

    using Antlr4.Runtime;

    using Extensions;

    using NContext.Common;

    using Newtonsoft.Json.Serialization;

    using Querying;

    public class ScimObjectTreeAnalysisResult
    {
        public bool UseDynamicLogic { get; private set; }

        public bool IsValidPathForAdd { get; private set; }

        public bool IsValidPathForRemove { get; private set; }

        public IDictionary<string, object> Container { get; private set; }

        public string PropertyPathInParent { get; private set; }

        public JsonPatchProperty JsonPatchProperty { get; private set; }

        public ScimObjectTreeAnalysisResult(
            object objectToSearch,
            string filter,
            IContractResolver contractResolver)
        {
            /* 
                ScimFilter will handle normalizing the actual path string. 
                
                Examples:
                "path":"members"
                "path":"name.familyName"
                "path":"addresses[type eq \"work\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName"

                Once normalized, associate each resource member with its filter (if present). -> Tuple<memberName, memberFilter>
            */
            var propertyPathTree = new ScimFilter(filter)
                .Paths
                .Select(pp =>
                {
                    var bracketIndex = pp.IndexOf('[');
                    if (bracketIndex == -1) return new Tuple<string, string>(pp, null);

                    return new Tuple<string, string>(pp.Substring(0, bracketIndex), pp.Substring(bracketIndex + 1, pp.Length - bracketIndex - 2));
                })
                .ToList();
            
            int lastPosition = 0;
            object targetObject = objectToSearch;
            for (int i = 0; i < propertyPathTree.Count; i++)
            {
                lastPosition = i;

                // if the current target object is an ExpandoObject (IDictionary<string, object>),
                // we cannot use the ContractResolver.
                var dictionary = targetObject as IDictionary<string, object>;
                if (dictionary != null)
                {
                    // find the value in the dictionary                   
                    if (dictionary.ContainsCaseInsensitiveKey(propertyPathTree[i].Item1))
                    {
                        // TODO: (DG) This (Item1) needs to support complex property paths (ie. name.familyName)
                        var possibleNewTargetObject = dictionary.GetValueForCaseInsensitiveKey(propertyPathTree[i].Item1);

                        // unless we're at the last item, we should set the targetobject
                        // to the new object.  If we're at the last item, we need to stop
                        if (i != propertyPathTree.Count - 1)
                        {
                            targetObject = possibleNewTargetObject;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var jsonContract = (JsonObjectContract) contractResolver.ResolveContract(targetObject.GetType());
                    
                    var attemptedProperty = jsonContract
                        .Properties
                        .FirstOrDefault(p => string.Equals(p.PropertyName, propertyPathTree[i].Item1, StringComparison.OrdinalIgnoreCase));

                    if (attemptedProperty == null)
                    {
                        // property cannot be found, and we're not working with dynamics. 
                        // Stop, and return invalid path.

                        // TODO: (DG) Set SCIM Patch error for invalid path
                        break;
                    }

                    if (propertyPathTree[i].Item2 != null)
                    {
                        // we can only filter enumerable types
                        if (!attemptedProperty.PropertyType.IsNonStringEnumerable())
                        {
                            // TODO: (DG) Set SCIM Patch error for invalid filter attribute
                            break;
                        }
                        
                        var enumerable = attemptedProperty.ValueProvider.GetValue(targetObject) as IEnumerable;
                        if (enumerable == null)
                        {
                            // if the value of the attribute is null then there's nothing to filter
                            // it should never get here beause ScimObjectAdapter should apply a 
                            // different ruleset for null values; replacing or setting the attribute value
                            break;
                        }

                        Delegate predicate;
                        try
                        {
                            // parse our filter into an expression tree
                            var lexer = new ScimFilterLexer(new AntlrInputStream(propertyPathTree[i].Item2));
                            var parser = new ScimFilterParser(new CommonTokenStream(lexer));
                            var filterVisitorType = typeof(ScimFilterVisitor<>).MakeGenericType(attemptedProperty.PropertyType.GetGenericArguments()[0]);
                            var filterVisitor = (IScimFilterVisitor)filterVisitorType.CreateInstance();
                            predicate = filterVisitor.VisitExpression(parser.parse()).Compile();

                        }
                        catch (Exception)
                        {
                            // TODO: (DG) Set SCIM Patch error for an invalid filter

                            throw;
                        }

                        /*
                            TODO: (DG) Handle case when:

                                o  If the target location already contains the value specified, no
                                  changes SHOULD be made to the resource, and a success response
                                  SHOULD be returned.  Unless other operations change the resource,
                                  this operation SHALL NOT change the modify timestamp of the
                                  resource.
                        */

                        var enumerator = enumerable.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if ((bool)predicate.DynamicInvoke(enumerator.Current))
                            {
                                targetObject = enumerator.Current;
                                break; // UNSURE ABOUT THIS. This acts like Single() and will only affect the first positive case
                            }
                        }
                    }
                    else
                    {
                        // unless we're at the last item, we should continue searching.
                        // If we're at the last item, we need to stop
                        if ((i != propertyPathTree.Count - 1))
                        {
                            targetObject = attemptedProperty.ValueProvider.GetValue(targetObject);
                        }
                    }
                }
            }

            if (propertyPathTree.Count - lastPosition != 1)
            {
                IsValidPathForAdd = false;
                IsValidPathForRemove = false;
                return;
            }

            // two things can happen now.  The targetproperty can be an IDictionary - in that
            // case, it's valid for add if there's 1 item left in the propertyPathTree.
            //
            // it can also be a property info.  In that case, if there's nothing left in the path
            // tree we're at the end, if there's one left we can try and set that. 
            if (targetObject is IDictionary<string, object>)
            {
                UseDynamicLogic = true;

                Container = (IDictionary<string, object>)targetObject;
                IsValidPathForAdd = true;
                PropertyPathInParent = propertyPathTree[propertyPathTree.Count - 1].Item1;

                // to be able to remove this property, it must exist
                IsValidPathForRemove = Container.ContainsCaseInsensitiveKey(PropertyPathInParent);
            }
            else if (targetObject is IEnumerable)
            {
                UseDynamicLogic = false;
                IsValidPathForAdd = true;
                IsValidPathForRemove = true;
                PropertyPathInParent = propertyPathTree[propertyPathTree.Count - 1].Item1;
            }
            else
            {
                UseDynamicLogic = false;

                var property = propertyPathTree[propertyPathTree.Count - 1].Item1;
                var jsonContract = (JsonObjectContract)contractResolver.ResolveContract(targetObject.GetType());
                var attemptedProperty = jsonContract
                    .Properties
                    .FirstOrDefault(p => string.Equals(p.PropertyName, property, StringComparison.OrdinalIgnoreCase));

                if (attemptedProperty == null)
                {
                    IsValidPathForAdd = false;
                    IsValidPathForRemove = false;
                }
                else
                {
                    IsValidPathForAdd = true;
                    IsValidPathForRemove = true;
                    JsonPatchProperty = new JsonPatchProperty(attemptedProperty, targetObject);
                    PropertyPathInParent = property;
                }
            }
        }
    }
}