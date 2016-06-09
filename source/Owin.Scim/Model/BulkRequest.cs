namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    using Newtonsoft.Json;

    using Serialization;
    /*
    public class BulkRequest : SchemaBase
    {
        [JsonProperty("failOnErrors")]
        public int? FailOnErrors { get; set; }

        [JsonProperty("Operations")]
        public IEnumerable<BulkOperation> Operations { get; set; }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.BulkRequest; }
        }
    }

    public class BulkOperation : MultiValuedAttribute
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("bulkId")]
        public string BulkId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("data")]
        public ScimData Data { get; set; }

        [JsonProperty("location")]
        public HttpContent Response { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(IntAsStringEnumConverter))]
        public HttpStatusCode Status { get; set; }
    }

    public class BulkResponse
    {
        public IEnumerable<string> Schemas
        {
            get { return new[] { ScimConstants.Schemas.BulkResponse }; }
        }

        public IEnumerable<BulkOperationResponse> Operations { get; set; }
    }*/
}