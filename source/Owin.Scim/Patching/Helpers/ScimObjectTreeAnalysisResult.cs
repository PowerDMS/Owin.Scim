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

    using Model;

    using NContext.Common;

    using Newtonsoft.Json.Serialization;

    using Querying;

    public class ScimObjectTreeAnalysisResult
    {
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

            var lastPosition = 0;
            var nodes = GetAffectedMembers(contractResolver, propertyPathTree, ref lastPosition, new Node(objectToSearch, null));
            
            if ((propertyPathTree.Count - lastPosition) > 1)
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
            PatchMembers = new List<PatchMember>();
            foreach (var node in nodes)
            {
                if (node.Target is IDictionary<string, object>)
                {
                    UseDynamicLogic = true;

                    Container = (IDictionary<string, object>)node.Target;
                    IsValidPathForAdd = true;

                    var member = new PatchMember(propertyPathTree[propertyPathTree.Count - 1].Item1, null);
                    IsValidPathForRemove = Container.ContainsCaseInsensitiveKey(member.PropertyPathInParent);

                    PatchMembers.Add(member);
                }
                else if (node.Target is IEnumerable)
                {
                    UseDynamicLogic = false;
                    IsValidPathForAdd = true;
                    IsValidPathForRemove = true;

                    PatchMembers.Add(new PatchMember(propertyPathTree[propertyPathTree.Count - 1].Item1, null));
                }
                else if (node.Target is MultiValuedAttribute && 
                    PathIsMultiValuedEnumerable(
                        contractResolver,
                        propertyPathTree[propertyPathTree.Count - 1].Item1, 
                        node))
                {
                    /* This handles the case when no sub-attribute has been specified. Meaning, the
                       filter used on a multiValuedAttribute had no sub-path.
                       (e.g. "path":"addresses[type eq \"work\"]")

                       This kind of filter can return us multiple elements of an enumerable that 
                       need to be modified.  Since the client did not specify a sub-attribute, SCIM
                       uses the Value property for assignment and we should therefore add that to the
                       PatchMembers.
                    */
                    var jsonContract = (JsonObjectContract)contractResolver.ResolveContract(node.Target.GetType());
                    var attemptedProperty = jsonContract
                        .Properties
                        .SingleOrDefault(p => p.PropertyName.Equals("Value", StringComparison.OrdinalIgnoreCase));

                    UseDynamicLogic = false;
                    IsValidPathForAdd = true;
                    IsValidPathForRemove = true;
                    PatchMembers.Add(new PatchMember("Value", new JsonPatchProperty(attemptedProperty, node.Target)));
                }
                else
                {
                    UseDynamicLogic = false;

                    var property = propertyPathTree[propertyPathTree.Count - 1].Item1;
                    var jsonContract = (JsonObjectContract)contractResolver.ResolveContract(node.Target.GetType());
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
                        PatchMembers.Add(
                            new PatchMember(
                                property, new JsonPatchProperty(attemptedProperty, node.Target)));
                    }
                }
            }
        }

        private bool PathIsMultiValuedEnumerable(IContractResolver contractResolver, string property, Node node)
        {
            var jsonContract = (JsonObjectContract)contractResolver.ResolveContract(node.Parent.GetType());
            var attemptedProperty = jsonContract
                .Properties
                .SingleOrDefault(p => p.PropertyName.Equals(property, StringComparison.OrdinalIgnoreCase));

            return attemptedProperty != null &&
                   attemptedProperty.PropertyType.IsNonStringEnumerable() &&
                   attemptedProperty.PropertyType.IsGenericType &&
                   attemptedProperty.PropertyType.GetGenericArguments()[0] == node.Target.GetType();
        }

        private IEnumerable<Node> GetAffectedMembers(
            IContractResolver contractResolver, 
            List<Tuple<string, string>> propertyPathTree, 
            ref int lastPosition,
            Node node)
        {
            for (int i = lastPosition; i < propertyPathTree.Count; i++)
            {
                lastPosition = i;
                // if the current target object is an ExpandoObject (IDictionary<string, object>),
                // we cannot use the ContractResolver.
                var dictionary = node.Target as IDictionary<string, object>;
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
                            node = new Node(possibleNewTargetObject, node.Target);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var jsonContract = (JsonObjectContract) contractResolver.ResolveContract(node.Target.GetType());

                    var attemptedProperty = jsonContract
                        .Properties
                        .FirstOrDefault(
                            p => string.Equals(p.PropertyName, propertyPathTree[i].Item1, StringComparison.OrdinalIgnoreCase));

                    if (attemptedProperty == null)
                    {
                        // property cannot be found, and we're not working with dynamics.
                        ErrorType = ScimErrorType.NoTarget;
                        break;
                    }

                    // if there's nothing to filter and we're not yet at the last path entry, continue
                    if (propertyPathTree[i].Item2 == null && i != propertyPathTree.Count - 1)
                    {
                        node = new Node(attemptedProperty.ValueProvider.GetValue(node.Target), node.Target);
                        continue;
                    }

                    if (propertyPathTree[i].Item2 != null)
                    {
                        // we can only filter enumerable types
                        if (!attemptedProperty.PropertyType.IsNonStringEnumerable())
                        {
                            ErrorType = ScimErrorType.InvalidFilter;
                            break;
                        }

                        var enumerable = attemptedProperty.ValueProvider.GetValue(node.Target) as IEnumerable;
                        if (enumerable == null)
                        {
                            // if the value of the attribute is null then there's nothing to filter
                            // it should never get here beause ScimObjectAdapter should apply a 
                            // different ruleset for null values; replacing or setting the attribute value
                            ErrorType = ScimErrorType.NoTarget;
                            break;
                        }

                        Delegate predicate;
                        try
                        {
                            // parse our filter into an expression tree
                            var lexer = new ScimFilterLexer(new AntlrInputStream(propertyPathTree[i].Item2));
                            var parser = new ScimFilterParser(new CommonTokenStream(lexer));
                            var filterVisitorType =
                                typeof (ScimFilterVisitor<>).MakeGenericType(
                                    attemptedProperty.PropertyType.GetGenericArguments()[0]);
                            var filterVisitor = (IScimFilterVisitor) filterVisitorType.CreateInstance();
                            predicate = filterVisitor.VisitExpression(parser.parse()).Compile();
                        }
                        catch (Exception)
                        {
                            ErrorType = ScimErrorType.InvalidFilter;
                            break;
                        }

                        var filteredNodes = new List<Node>();
                        var enumerator = enumerable.GetEnumerator();
                        lastPosition = i + 1;
                        while (enumerator.MoveNext())
                        {
                            if ((bool) predicate.DynamicInvoke(enumerator.Current))
                            {
                                filteredNodes.AddRange(
                                    GetAffectedMembers(
                                        contractResolver,
                                        propertyPathTree,
                                        ref lastPosition,
                                        new Node(enumerator.Current, node.Target)));
                            }
                        }

                        return filteredNodes;
                    }
                }
            }

            return new List<Node> { node };
        }

        public bool UseDynamicLogic { get; private set; }

        public bool IsValidPathForAdd { get; private set; }

        public bool IsValidPathForRemove { get; private set; }

        public IDictionary<string, object> Container { get; private set; }

        public IList<PatchMember> PatchMembers { get; private set; } 

        public ScimErrorType ErrorType { get; set; }

        public class PatchMember
        {
            public PatchMember(string propertyPathInParent, JsonPatchProperty jsonPatchProperty)
            {
                PropertyPathInParent = propertyPathInParent;
                JsonPatchProperty = jsonPatchProperty;
            }

            public string PropertyPathInParent { get; private set; }

            public JsonPatchProperty JsonPatchProperty { get; private set; }
        }

        private class Node
        {
            public Node(object target, object parent)
            {
                Target = target;
                Parent = parent;
            }

            public object Target { get; private set; }

            public object Parent { get; private set; }
        }
    }
}