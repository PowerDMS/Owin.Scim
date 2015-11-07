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

    // This code is based on ideas from Microsoft's (Microsoft.AspNet.JsonPatch) ObjectTreeAnalysisResult.cs
    // Pretty much all the original code is gone, however, and this has been heavily modified to add support 
    // for IEnumerable, SCIM query filters, and observe the rules surrounding SCIM Patch.
    public class ScimObjectTreeAnalysisResult
    {
        private readonly IContractResolver _ContractResolver;

        public ScimObjectTreeAnalysisResult(object objectToSearch, string filter, IContractResolver contractResolver)
        {
            _ContractResolver = contractResolver;

            /* 
                ScimFilter.cs will handle normalizing the actual path string. 
                
                Examples:
                "path":"members"
                "path":"name.familyName"
                "path":"addresses[type eq \"work\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"]"
                "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName"

                Once normalized, associate each resource member with its filter (if present). -> Tuple<memberName, memberFilter>
            */
            var pathTree = new ScimFilter(filter)
                .Paths
                .Select(pp =>
                {
                    var bracketIndex = pp.IndexOf('[');
                    if (bracketIndex == -1) return new PathMember(pp);

                    return new PathMember(pp.Substring(0, bracketIndex), pp.Substring(bracketIndex + 1, pp.Length - bracketIndex - 2));
                })
                .ToList();

            var lastPosition = 0;
            var nodes = GetAffectedMembers(pathTree, ref lastPosition, new Node(objectToSearch, null));
            
            if ((pathTree.Count - lastPosition) > 1)
            {
                IsValidPathForAdd = false;
                IsValidPathForRemove = false;
                return;
            }

            PatchMembers = new List<PatchMember>();
            foreach (var node in nodes)
            {
                if (node.Target is IDictionary<string, object>)
                {
                    UseDynamicLogic = true;

                    Container = (IDictionary<string, object>)node.Target;
                    IsValidPathForAdd = true;

                    var member = new PatchMember(pathTree[pathTree.Count - 1].Path, null);
                    IsValidPathForRemove = Container.ContainsCaseInsensitiveKey(member.PropertyPathInParent);

                    PatchMembers.Add(member);
                }
                else
                {
                    var attribute = node.Target as MultiValuedAttribute;
                    JsonProperty attemptedProperty;
                    if (attribute != null && PathIsMultiValuedEnumerable(pathTree[pathTree.Count - 1].Path, node, out attemptedProperty))
                    {
                        /* Check if we're at a MultiValuedAttribute.
                           If so, then we'll return a special PatchMember.  This is because our actual target is 
                           an element within an enumerable. (e.g. User->Emails[element])
                           So a PatchMember must have three pieces of information: (following the example above)
                           > Parent (User)
                           > PropertyPath (emails)
                           > Actual Target (email instance/element)
                        */

                        UseDynamicLogic = false;
                        IsValidPathForAdd = true;
                        IsValidPathForRemove = true;
                        PatchMembers.Add(
                            new PatchMember(
                                pathTree[pathTree.Count - 1].Path, 
                                new JsonPatchProperty(attemptedProperty, node.Parent),
                                node.Target));
                    }
                    else
                    {
                        UseDynamicLogic = false;
                        
                        var jsonContract = (JsonObjectContract)contractResolver.ResolveContract(node.Target.GetType());
                        attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(pathTree[pathTree.Count - 1].Path);
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
                                    pathTree[pathTree.Count - 1].Path, 
                                    new JsonPatchProperty(attemptedProperty, node.Target)));
                        }
                    }
                }
            }
        }

        private bool PathIsMultiValuedEnumerable(string propertyName, Node node, out JsonProperty attemptedProperty)
        {
            var jsonContract = (JsonObjectContract)_ContractResolver.ResolveContract(node.Parent.GetType());
            attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(propertyName);

            if (attemptedProperty != null &&
                attemptedProperty.PropertyType.IsNonStringEnumerable() &&
                attemptedProperty.PropertyType.IsGenericType &&
                attemptedProperty.PropertyType.GetGenericArguments()[0] == node.Target.GetType())
            {
                return true;
            }

            return false;
        }

        private IEnumerable<Node> GetAffectedMembers( 
            List<PathMember> pathTree, 
            ref int lastPosition,
            Node node)
        {
            for (int i = lastPosition; i < pathTree.Count; i++)
            {
                // seems absurd, but this MAY be called recursively, OR simply
                // interated via the for loop - 
                lastPosition = i; 

                // if the current target object is an ExpandoObject (IDictionary<string, object>),
                // we cannot use the ContractResolver.
                var dictionary = node.Target as IDictionary<string, object>;
                if (dictionary != null)
                {
                    // find the value in the dictionary                   
                    if (dictionary.ContainsCaseInsensitiveKey(pathTree[i].Path))
                    {
                        // TODO: (DG) This (Path) needs to support complex property paths (ie. name.familyName)
                        var possibleNewTargetObject = dictionary.GetValueForCaseInsensitiveKey(pathTree[i].Path);

                        // unless we're at the last item, we should set the targetobject
                        // to the new object.  If we're at the last item, we need to stop
                        if (i != pathTree.Count - 1)
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
                    var jsonContract = (JsonObjectContract)_ContractResolver.ResolveContract(node.Target.GetType());
                    var attemptedProperty = jsonContract.Properties.GetClosestMatchProperty(pathTree[i].Path);
                    if (attemptedProperty == null)
                    {
                        // property cannot be found, and we're not working with dynamics.
                        ErrorType = ScimErrorType.NoTarget;
                        break;
                    }

                    // if there's nothing to filter and we're not yet at the last path entry, continue
                    if (pathTree[i].Filter == null && i != pathTree.Count - 1)
                    {
                        // the Target becomes the Target's child property value
                        // the Parent becomes the current Target
                        node = new Node(attemptedProperty.ValueProvider.GetValue(node.Target), node.Target);
                        continue; // keep traversing the path tree
                    }
                    
                    if (pathTree[i].Filter != null)
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
                            var lexer = new ScimFilterLexer(new AntlrInputStream(pathTree[i].Filter));
                            var parser = new ScimFilterParser(new CommonTokenStream(lexer));

                            // create a visitor for the type of enumerable generic argument
                            var enumerableType = attemptedProperty.PropertyType.GetGenericArguments()[0];
                            var filterVisitorType = typeof (ScimFilterVisitor<>).MakeGenericType(enumerableType);
                            var filterVisitor = (IScimFilterVisitor) filterVisitorType.CreateInstance();
                            predicate = filterVisitor.VisitExpression(parser.parse()).Compile();
                        }
                        catch (Exception)
                        {
                            ErrorType = ScimErrorType.InvalidFilter;
                            break;
                        }

                        // we have an enumerable and a filter predicate
                        // for each element in the enumerable that satisfies the predicate, 
                        // visit that element as part of the path tree
                        var filteredNodes = new List<Node>();
                        var enumerator = enumerable.GetEnumerator();
                        lastPosition = i + 1; // increase the position in the tree
                        while (enumerator.MoveNext())
                        {
                            if ((bool) predicate.DynamicInvoke(enumerator.Current))
                            {
                                filteredNodes.AddRange(
                                    GetAffectedMembers(
                                        pathTree,
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
        
        private class PathMember
        {
            public PathMember(string path, string filter = null)
            {
                Path = path;
                Filter = filter;
            }

            public string Path { get; private set; }

            public string Filter { get; private set; }
        }
    }
}