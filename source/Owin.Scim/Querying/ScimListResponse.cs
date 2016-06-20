namespace Owin.Scim.Querying
{
    using System.Collections.Generic;
    using System.Linq;

    using Model;

    using Newtonsoft.Json;
    
    public abstract class ScimListResponse : SchemaBase
    {
        protected ScimListResponse()
        {
            Resources = new List<Resource>();
        }

        [JsonProperty("totalResults", Order = 0)]
        public int TotalResults
        {
            get { return Resources == null ? 0 : Resources.Count(); }
        }
        
        [JsonProperty("Resources", Order = 1)]
        public IEnumerable<Resource> Resources { get; protected set; }

        [JsonProperty("startIndex", Order = 2)]
        public int StartIndex { get; set; }

        [JsonProperty("itemsPerPage", Order = 3)]
        public int ItemsPerPage { get; set; }
    }
}