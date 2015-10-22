namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;

    using DryIoc;

    using Newtonsoft.Json;

    public class ScimError
    {
        private IEnumerable<string> _Schemas;

        [JsonConstructor]
        public ScimError(
            HttpStatusCode status, 
            ScimType scimType = null,
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

        [JsonProperty(ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public HttpStatusCode Status { get; private set; }

        public ScimType ScimType { get; set; }

        public string Detail { get; set; }
    }
}