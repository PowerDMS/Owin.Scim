namespace Owin.Scim.Tests
{
    using System.Collections.Generic;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    using Configuration;

    using Newtonsoft.Json;

    using Serialization;

    /// <summary>
    /// Must support a media type of "application/scim+json"
    /// See: https://tools.ietf.org/html/rfc7644#section-3.1
    /// </summary>
    public class ScimClientJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public ScimClientJsonMediaTypeFormatter(ScimServerConfiguration serverConfiguration)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/scim+json"));
            SerializerSettings.Converters.Add(new ResourceJsonConverter(serverConfiguration, JsonSerializer.Create(SerializerSettings)));
        }

        public override void SetDefaultContentHeaders(System.Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/scim+json");
        }

        public IEnumerable<MediaTypeFormatter> AsEnumerable
        {
            get { return new[] { this }; }
        }
    }
}