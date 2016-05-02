namespace Owin.Scim.Querying
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Model;
    using Model.Users;

    using Newtonsoft.Json;

    [KnownType(typeof(User))]
    public class ScimListResponse : SchemaBase
    {
        public ScimListResponse(IEnumerable<Resource> resources)
        {
            Resources = resources?.ToList() ?? new List<Resource>();
        }

        [JsonConstructor]
        private ScimListResponse() { }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Messages.ListResponse; }
        }

        [JsonProperty("totalResults", Order = 0)]
        public int TotalResults
        {
            get { return Resources?.Count() ?? 0; }
        }
        
        [JsonProperty("Resources", Order = 1)]
        public IEnumerable<Resource> Resources { get; private set; }

        [JsonProperty("startIndex", Order = 2)]
        public int StartIndex { get; set; }

        [JsonProperty("itemsPerPage", Order = 3)]
        public int ItemsPerPage { get; set; }
    }
}