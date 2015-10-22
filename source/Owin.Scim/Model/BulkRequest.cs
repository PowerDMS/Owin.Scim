namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    /*
    public class BulkRequest
    {
        public IEnumerable<string> Schemas
        {
            get { return new[] { ScimConstants.Schemas.BulkRequest }; }
        }

        public int FailOnErrors { get; set; }

        public IEnumerable<BulkOperation> Operations { get; set; }
    }

    public class BulkOperation : MultiValuedAttribute
    {
        public string Method { get; set; }

        public string BulkId { get; set; }

        public string Version { get; set; }

        public string Path { get; set; }

        public ScimData Data { get; set; }

        public HttpContent Response { get; set; }

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