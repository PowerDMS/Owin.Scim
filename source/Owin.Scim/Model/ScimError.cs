namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;

    using Newtonsoft.Json;

    using Serialization;

    public class ScimError
    {
        private IEnumerable<string> _Schemas;

        [JsonConstructor]
        public ScimError(
            HttpStatusCode status, 
            ScimErrorType scimType = null,
            string detail = null)
        {
            _Schemas = new[] { ScimConstants.Messages.Error };
            Status = status;
            ScimType = scimType;
            Detail = detail;
        }
        
        public IEnumerable<string> Schemas
        {
            get { return _Schemas; }
            set { _Schemas = value; }
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(IntAsStringEnumConverter))]
        public HttpStatusCode Status { get; private set; }

        [JsonProperty("scimType")]
        public ScimErrorType ScimType { get; private set; }

        [JsonProperty("detail")]
        public string Detail { get; private set; }
    }
}