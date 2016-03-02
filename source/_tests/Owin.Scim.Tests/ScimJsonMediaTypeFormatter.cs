namespace Owin.Scim.Tests
{
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Must support a media type of "application/scim+json"
    /// See: https://tools.ietf.org/html/rfc7644#section-3.1
    /// </summary>
    public class ScimJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public ScimJsonMediaTypeFormatter()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/scim+json"));
        }

        public override void SetDefaultContentHeaders(System.Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/scim+json");
        }

        public static ScimJsonMediaTypeFormatter[] AsArray()
        {
            return new[]
            {
                new ScimJsonMediaTypeFormatter()
            };
        }
    }
}