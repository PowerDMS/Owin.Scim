namespace Owin.Scim.Filtering
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

    using Newtonsoft.Json.Serialization;

    public class ScimFilter
    {
        private JsonProperty _PropertyToFilter;

        public ScimFilter(string filter)
        {
            ProcessFilter(filter);
        }

        public ScimFilter(JsonProperty propertyToFilter, string filterExpression)
        {
            _PropertyToFilter = propertyToFilter;
            ProcessFilter(filterExpression);
        }

        private void ProcessFilter(string filter)
        {
            
        }

        private class Filter
        {
            public string PropertyName { get; set; }

            public Expression Operator { get; set; }

            public object Value { get; set; }
        }
    }
}