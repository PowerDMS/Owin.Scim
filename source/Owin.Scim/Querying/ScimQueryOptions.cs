namespace Owin.Scim.Querying
{
    using System.Collections.Generic;

    using Model;
    
    using Newtonsoft.Json;
    
    public class ScimQueryOptions : SchemaBase
    {
        public ScimQueryOptions()
        {
            Attributes = new HashSet<string>();
            ExcludedAttributes = new HashSet<string>();
        }

        [JsonProperty("attribues")]
        public ISet<string> Attributes { get; internal set; }

        [JsonProperty("excludedAttributes")]
        public ISet<string> ExcludedAttributes { get; internal set; }

        [JsonProperty("filter")]
        public PathFilterExpression Filter { get; internal set; }

        [JsonProperty("sortBy")]
        public string SortBy { get; internal set; }

        [JsonProperty("sortOrder")]
        public SortOrder SortOrder { get; internal set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; internal set; }

        [JsonProperty("count")]
        public int Count { get; internal set; }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Messages.SearchRequest; }
        }
    }
}