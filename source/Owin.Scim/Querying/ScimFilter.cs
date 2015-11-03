namespace Owin.Scim.Querying
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    public class ScimFilter
    {
        private IEnumerable<string> _Paths;

        private string _NormalizedFilterExpression;

        public ScimFilter(string filterExpression)
        {
            ProcessFilter(filterExpression);
        }

        public static implicit operator string(ScimFilter filter)
        {
            return filter.NormalizedFilterExpression;
        }

        public IEnumerable<string> Paths
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
            Processing a SCIM filter SHOULD support query filters and PATCH path filters

            Query Examples

            filter=meta.lastModified gt "2011-05-13T04:42:34Z"
            filter=userType eq "Employee" and (emails co "example.com" or emails.value co "example.org")
            filter=userType eq "Employee" and (emails.type eq "work")            
            filter=userType eq "Employee" and emails[type eq "work" and value co "@example.com"]

            PATCH Path Examples

            "path":"name.familyName"
            "path":"addresses[type eq \"work\"]"
            "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"]"
            "path":"members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName"

            In the below code, we look to differentiate a '.' from a path after a filter expression and one
            that's placed before a potential filter expression.  If it's before, then we replace the '.' with a 
            temporary '/' - used as a marker.  This way we can normalize our filters with brackets.

            This is very important because if we just did a string.Split('.') then:

            string.Split(members[value eq \"2819c223-7f76-453a-919d-413861904646\"].displayName)
                becomes [2] { "members[value eq \"2819c223-7f76-453a-919d-413861904646\"]", "displayName" } ... VALID!

            string.Split(meta.lastModified gt "2011-05-13T04:42:34Z")
                becomes [2] { "meta", "lastModified gt "2011-05-13T04:42:34Z" } ............................... INVALID!

            The second example there shows that we'd have TWO paths instead of ONE + filter. The approach taken here
            normalizes the second expression into: meta[lastModified gt "2011-05-13T04:42:34Z]"
            */

            var pathList = new List<string>();
            var pathBuilder = new StringBuilder();
            var openQuoteFound = false;
            var closing = false;
            var separatorIndex = -1;
            for (int index = 0; index < filterExpression.Length; index++)
            {
                var pathChar = filterExpression[index];
                var isQuote = pathChar == '"';
                if (isQuote && openQuoteFound)
                {
                    closing = true;
                }

                if (pathChar == '.' && filterExpression[index - 1] != ']' && !openQuoteFound)
                {
                    // we are most likely parsing a query filter, not PATCH path filter
                    separatorIndex = index;
                    pathBuilder.Append('/');
                }
                else
                {
                    if (pathChar != '.' || (pathChar == '.' && openQuoteFound))
                    {
                        pathBuilder.Append(pathChar);

                        if (separatorIndex > -1)
                        {
                            // if we're at the end of a filter expression value (end quote) OR 
                            // if we're filtering with the 'pr' operator
                            // then let's close out our sub-attribute filter expression and reset
                            if ((isQuote && openQuoteFound) ||
                                (pathChar == 'r' && filterExpression[index - 1] == 'p'))
                            {
                                pathBuilder.Replace('/', '[', separatorIndex, 1);
                                pathBuilder.Append(']');
                                pathList.Add(pathBuilder.ToString());

                                // reset
                                pathBuilder.Clear();
                                separatorIndex = -1;
                            }
                        }

                        if (isQuote)
                        {
                            if (!openQuoteFound)
                            {
                                openQuoteFound = true; // set marker
                            }
                            else if (closing)
                            {
                                openQuoteFound = false; // reset
                                closing = false;
                            }
                        }
                    }
                    else
                    {
                        pathBuilder.Append('/');
                    }
                }
            }

            if (pathBuilder.Length > 0)
                pathList.AddRange(pathBuilder.ToString().Split('/'));

            _NormalizedFilterExpression = string.Concat(pathList);
            _Paths = pathList;
        }
    }
}