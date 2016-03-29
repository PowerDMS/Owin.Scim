using System.Collections.Generic;

namespace Owin.Scim.Querying
{
    using System;
    using System.Linq;

    [Serializable]
    public class ScimQueryOptions
    {
        private ISet<string> _Attributes;

        private ISet<string> _ExcludedAttributes;

        public ScimQueryOptions(IEnumerable<KeyValuePair<string, string>> queryNameValuePairs)
        {
            if (queryNameValuePairs != null)
            {
                foreach (var kvp in queryNameValuePairs)
                {
                    if (kvp.Key.Equals("attributes", StringComparison.OrdinalIgnoreCase))
                        _Attributes = new HashSet<string>(
                            kvp.Value.Split(new [] { ','}, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase), 
                            StringComparer.OrdinalIgnoreCase);
                    else if (kvp.Key.Equals("excludedAttributes", StringComparison.OrdinalIgnoreCase))
                        _ExcludedAttributes = new HashSet<string>(
                            kvp.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase), 
                            StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public ISet<string> Attributes
        {
            get
            {
                if (_Attributes == null)
                    _Attributes = new HashSet<string>();

                return _Attributes;
            }
        }

        public ISet<string> ExcludedAttributes
        {
            get
            {
                if (_ExcludedAttributes == null)
                    _ExcludedAttributes = new HashSet<string>();

                return _ExcludedAttributes;
            }
        }
    }
}