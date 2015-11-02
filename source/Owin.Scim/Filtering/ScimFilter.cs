namespace Owin.Scim.Filtering
{
    using System.Collections.Generic;
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
            var pathList = new List<string>();
            var pathBuilder = new StringBuilder();
            var openQuoteExists = false;
            var separatorIndex = -1;
            for (int index = 0; index < filterExpression.Length; index++)
            {
                var pathChar = filterExpression[index];
                if (pathChar == '.' && filterExpression[index - 1] != ']')
                {
                    // we are most likely parsing a query filter, not PATCH path filter
                    separatorIndex = index;
                    pathBuilder.Append('/');
                }
                else
                {
                    pathBuilder.Append(pathChar);
                    
                    // if we're filtering with the 'pr' operator OR we're at the end of a filter expression value (end quote)
                    // then let's close out our sub-attribute filter expression and reset
                    if (separatorIndex > -1)
                    {
                        if (pathChar == '"' || (pathChar == 'r' && filterExpression[index - 1] == 'p'))
                        {
                            if (pathChar == '"' && !openQuoteExists)
                            {
                                openQuoteExists = true; // set marker
                            }
                            else
                            {
                                pathBuilder.Replace('/', '[', separatorIndex, 1);
                                pathBuilder.Append(']');
                                pathList.Add(pathBuilder.ToString());

                                pathBuilder.Clear(); // reset
                                openQuoteExists = false; // reset
                                separatorIndex = -1; // reset
                            }
                        }
                    }
                }
            }

            if (pathBuilder.Length > 0)
                pathList.AddRange(pathBuilder.ToString().Split('/', '.'));

            _NormalizedFilterExpression = string.Concat(pathList);
            _Paths = pathList;
        }
    }
}