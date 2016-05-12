namespace Owin.Scim.ErrorHandling
{
    using System;
    using System.Net;

    using Model;

    /// <summary>
    /// Use this to throw exceptions inside Scim processing
    /// </summary>
    public class ScimException : ApplicationException
    {
        /// <summary>
        /// Scim Error
        /// </summary>
        public ScimError ScimError { get; private set; }

        /// <summary>
        /// Use this to control the type of response status returned to client.
        /// </summary>
        /// <param name="statusCode">HTTP response status code</param>
        /// <param name="detail">body text as raw string</param>
        /// <param name="errorType">Only applicable for status=400</param>
        public ScimException(HttpStatusCode statusCode, String detail, ScimErrorType errorType = null)
        {
            ScimError = new ScimError(statusCode, errorType, detail);
        }
    }
}