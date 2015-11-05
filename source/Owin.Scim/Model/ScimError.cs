namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class ScimError
    {
        private IEnumerable<string> _Schemas;

        [JsonConstructor]
        public ScimError(
            HttpStatusCode status, 
            ScimErrorType scimErrorType = null,
            string detail = null)
        {
            _Schemas = new[] { ScimConstants.Messages.Error };
            Status = status;
            ScimErrorType = scimErrorType;
            Detail = detail;
        }
        
        public IEnumerable<string> Schemas
        {
            get { return _Schemas; }
            set { _Schemas = value; }
        }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public HttpStatusCode Status { get; private set; }

        public ScimErrorType ScimErrorType { get; set; }

        public string Detail { get; set; }
    }
}