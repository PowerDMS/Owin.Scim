namespace Owin.Scim.Tests
{
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    /// <summary>
    /// Must support a media type of "application/scim+json"
    /// See: https://tools.ietf.org/html/rfc7644#section-3.1
    /// </summary>
    public class ScimJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public override void SetDefaultContentHeaders(System.Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/scim+json");
        }
    }
}