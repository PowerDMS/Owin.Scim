namespace Owin.Scim.Querying
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Extensions;

    public class ScimFilter
    {
        private readonly ISet<string> _ResourceExtensionSchemas;

        private readonly IList<PathFilterExpression> _Paths;

        private string _NormalizedFilterExpression;

        public ScimFilter(ISet<string> resourceExtensionSchemas, string filterExpression)
        {
            _ResourceExtensionSchemas = resourceExtensionSchemas;
            _Paths = new List<PathFilterExpression>();
            ProcessFilter(filterExpression);
        }

        public static implicit operator string(ScimFilter filter)
        {
            return filter.NormalizedFilterExpression;
        }

        public IEnumerable<PathFilterExpression> Paths
        {
            get { return _Paths; }
        }

        public string NormalizedFilterExpression
        {
            get { return _NormalizedFilterExpression; }
        }

        private void ProcessFilter(string filterExpression)
        {
            /*
            This processing of a SCIM filter adds support for both search query filters and patch path filters.
            See: https://tools.ietf.org/html/rfc7644#section-3.4.2

            Query Examples

            filter=name.givenName eq "Daniel"
            filter=meta[lastModified gt "2011-05-13T04:42:34Z"]
            filter=userType eq "Employee" and (emails co "example.com" or emails.value co "example.org")
            filter=userType eq "Employee" and (emails.type eq "work")            
            filter=userType eq "Employee" and emails[type eq "work" and value co "@example.com"]

            PATCH Path Examples

            "path":"name.familyName"
            "path":"addresses[type eq \"work\"]"
            "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\" or ]"
            "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName"
            "path":"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber"

            In the below code, we look to differentiate a '.' from a path after a filter expression and one
            that's used before a potential filter expression.
            */
            filterExpression = filterExpression.Trim(' ');
            var expressionBuilder = new StringBuilder();
            var boundaryStack = new Stack<char>();
            var boundaryDelimiters = new Dictionary<char, char>
            {
                { '"', '"' },
                { '[', ']' },
                { '(', ')' }
            };

            var pathFilterSeparators = new HashSet<char>
            {
                '[',
                '.'
            };

            var isPathOnly = true;
            var pathEndIndex = -1; // TODO: (DG) this should be a list to support > 1 depth
            var possibleResourceExtension = false;

            for (int index = 0; index < filterExpression.Length; index++)
            {
                var currentChar = filterExpression[index];

                // if we're in a boundary and the current char is the closing boundary char
                // example:
                // boundaryStack -> { '[' }
                // charAt[index] == ']'
                if (boundaryStack.Any() &&
                    boundaryDelimiters[boundaryStack.Peek()] == currentChar)
                {
                    boundaryStack.Pop();
                }
                else if (boundaryDelimiters.ContainsKey(currentChar))
                {
                    // if we've hit a boundary opener (e.g. '"', '(', '[')
                    boundaryStack.Push(currentChar);
                }

                // determine whether we're still parsing a path-only expression
                if (isPathOnly)
                {
                    // we only can have a path end if there's actually a path (ie. length > 0)
                    if (expressionBuilder.Length > 0 &&
                        !possibleResourceExtension &&
                        pathFilterSeparators.Contains(currentChar))
                    {
                        pathEndIndex = index;
                    }

                    if (boundaryStack.Any()) // shortcut
                        isPathOnly = false;
                    else if (
                        index > 0 &&
                        index + 1 < filterExpression.Length &&
                        filterExpression[index - 1] == ' ')
                    {
                        // do we have a valid operator?
                        switch (filterExpression.Substring(index, 2))
                        {
                            case "eq":
                            case "ne":
                            case "pr":
                            case "co":
                            case "sw":
                            case "ew":
                            case "lt":
                            case "le":
                            case "gt":
                            case "ge":
                                isPathOnly = false;
                                break;
                        }
                    }
                }

                if (!possibleResourceExtension && currentChar == ':')
                {
                    possibleResourceExtension = true;
                }
                
                // determine if we've identified a post-filter, sub-attribute (e.g. emails[type eq "work"].value
                if (currentChar == '.' && filterExpression[index - 1] == ']')
                {
                    var path = expressionBuilder.ToString(0, pathEndIndex);
                    var filter = expressionBuilder[pathEndIndex] == '.'
                        ? expressionBuilder.ToString(pathEndIndex + 1, expressionBuilder.Length - (pathEndIndex + 1))
                        : expressionBuilder.ToString(pathEndIndex + 1, expressionBuilder.Length - (pathEndIndex + 2));

                    _Paths.Add(new PathFilterExpression(path, filter));

                    // reset, we're starting at a new path
                    expressionBuilder.Clear();
                    pathEndIndex = -1;
                    isPathOnly = true;
                    possibleResourceExtension = false;

                    continue; // we don't want to append this '.' character as part of the expression because it represents a path boundary
                }

                if (possibleResourceExtension)
                {
                    // Determine if we've identified a resource extension
                    // the goal here is to break-up an extension from its sub-attribute into multiple paths
                    // Therefore, we must constantly check our ScimServerConfiguration to 
                    // determine if we've found a valid extension.
                    var possibleExtensionSchema = expressionBuilder.ToString() + currentChar;
                    if (_ResourceExtensionSchemas.Contains(possibleExtensionSchema))
                    {
                        _Paths.Add(new PathFilterExpression(possibleExtensionSchema, null));

                        // reset, we're starting at a new path
                        expressionBuilder.Clear();
                        pathEndIndex = -1;
                        isPathOnly = true;
                        possibleResourceExtension = false;

                        if (filterExpression[index + 1] == ':')
                        {
                            // we have a sub-attribute. ignore the colon
                            ++index;
                        }

                        continue;
                    }
                }

                // determine whether we need to replace an inner filter expression '.' with a bracket grouping
                if (currentChar == '.' && 
                    !isPathOnly &&
                    boundaryStack.Peek() != '"')
                {
                    // we are in a filter expression but not a string literal (valuePath)
                    // (e.g. userType ne \"Employee\" and not (emails co \"example.com\" or emails.value co \"example.org\")
                    //                                                                           ^^^
                    // what we want to do here is normalize our expression to use brackets instead of periods!
                    currentChar = '[';
                    var bracketBoundaryEnd = filterExpression.NthIndexOf('"', index, 2) + 1;
                    filterExpression = filterExpression.Insert(bracketBoundaryEnd, "]");
                    boundaryStack.Push(currentChar);
                }

                expressionBuilder.Append(currentChar);
            }

            if (expressionBuilder.Length > 0)
            {
                if (pathEndIndex > -1)
                {
                    if (isPathOnly)
                    {
                        foreach (var pathExp in expressionBuilder.ToString().Split('.'))
                        {
                            _Paths.Add(new PathFilterExpression(pathExp, null));
                        }
                    }
                    else
                    {
                        var path = expressionBuilder.ToString(0, pathEndIndex);
                        var filter = expressionBuilder[pathEndIndex] == '.'
                            ? expressionBuilder.ToString(pathEndIndex + 1, expressionBuilder.Length - (pathEndIndex + 1))
                            : expressionBuilder.ToString(pathEndIndex + 1, expressionBuilder.Length - (pathEndIndex + 2));

                        _Paths.Add(new PathFilterExpression(path, filter));
                    }
                }
                else
                {
                    if (expressionBuilder[0] == '[' && expressionBuilder[expressionBuilder.Length - 1] == ']')
                    {
                        expressionBuilder.Remove(0, 1);
                        expressionBuilder.Remove(expressionBuilder.Length - 1, 1);
                    }

                    _Paths.Add(
                        isPathOnly
                            ? PathFilterExpression.CreatePathOnly(expressionBuilder.ToString())
                            : PathFilterExpression.CreateFilterOnly(expressionBuilder.ToString()));
                }
            }

            // normalize the filter expression by reversing our logic above
            _NormalizedFilterExpression = 
                string.Concat(
                    _Paths.Select((expression, index) =>
                    {
                        if (expression.Filter == null)
                        {
                            return index == 0
                                ? expression.Path
                                : '.' + expression.Path;
                        }

                        return expression.Path == null
                            ? expression.Filter
                            : expression.Path + '[' + expression.Filter + ']';
                    }));
        }
    }
}