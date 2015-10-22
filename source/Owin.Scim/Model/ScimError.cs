namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;

    using Newtonsoft.Json;

    public class ScimError
    {
        public ScimError(
            HttpStatusCode status, 
            ScimType scimType = null,
            string detail = null)
        {
            Status = status;
            ScimType = scimType;
        }

        public IEnumerable<string> Schemas
        {
            get { return new[] { ScimConstants.Messages.Error }; }
        }

        [JsonProperty(ItemConverterType = typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public HttpStatusCode Status { get; private set; }

        public ScimType ScimType { get; set; }

        public string Detail { get; set; }
    }
}