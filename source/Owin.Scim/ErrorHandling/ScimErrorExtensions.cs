namespace Owin.Scim.ErrorHandling
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    using Model;

    public static class ScimErrorExtensions
    {
        public static HttpResponseException ToResponseException(this ScimError scimError)
        {
            var errorCollection = new[] {scimError};

            var resp = new HttpResponseMessage(scimError.Status)
            {
                Content = new ObjectContent<ScimError[]>(errorCollection,
                            new System.Net.Http.Formatting.JsonMediaTypeFormatter())
            };

            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/scim+json");

            return new HttpResponseException(resp);
        }
    }
}