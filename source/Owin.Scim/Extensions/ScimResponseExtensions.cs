namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Model;

    /// <summary>
    /// Defines extension methods for <see cref="IScimResponse{T}"/>.
    /// </summary>
    public static class ScimResponseExtensions
    {
        /// <summary>
        /// Returns a new <see cref="HttpResponseMessage"/> with the <see cref="HttpResponseMessage.Content"/> set to <paramref name="scimResponse"/>. If 
        /// <paramref name="scimResponse"/> contains an error, it will attempt to parse the <see cref="ScimError.Status"/> as an <see cref="HttpStatusCode"/> 
        /// and assign it to the response message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scimResponse">The content contained in the HTTP response.</param>
        /// <param name="httpRequestMessage">The active <see cref="HttpRequestMessage"/>.</param>
        /// <param name="statusCode">The <see cref="HttpStatusCode"/> to set if <paramref name="scimResponse"/> has no errors.</param>
        /// <returns>HttpResponseMessage instance.</returns>
        public static HttpResponseMessage ToHttpResponseMessage<T>(
            this IScimResponse<T> scimResponse,
            HttpRequestMessage httpRequestMessage,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (scimResponse == null)
            {
                throw new ArgumentNullException("scimResponse");
            }

            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException("httpRequestMessage");
            }

            var responseStatusCode = statusCode;
            if (scimResponse.IsLeft)
            {
                responseStatusCode = GetStatusCode(scimResponse.GetLeft());
            }

            return ShouldSetResponseContent(httpRequestMessage, responseStatusCode)
                ? httpRequestMessage.CreateResponse(responseStatusCode, scimResponse.GetContent())
                : httpRequestMessage.CreateResponse(responseStatusCode);
        }

        /// <summary>
        /// Invokes the specified <paramref name="responseBuilder" /> action if <paramref name="scimResponse" /> does not contain an error - returning the configured <see cref="HttpResponseMessage" />.
        /// If <paramref name="scimResponse" /> contains errors, the returned response with contain the error content and will attempt to parse the <see cref="Error.Code" /> as an
        /// <see cref="HttpStatusCode" /> and assign it to the response message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scimResponse">The <see cref="IScimResponse{t}" /> used to build the <see cref="HttpResponseMessage" />.</param>
        /// <param name="httpRequestMessage">The active <see cref="HttpRequestMessage" />.</param>
        /// <param name="responseBuilder">The response builder method to invoke when no errors exist.</param>
        /// <param name="setResponseContent">Determines whether to set the <param name="scimResponse.Data"></param> as the response content.</param>
        /// <returns>HttpResponseMessage instance.</returns>
        public static HttpResponseMessage ToHttpResponseMessage<T>(
            this IScimResponse<T> scimResponse,
            HttpRequestMessage httpRequestMessage,
            Action<T, HttpResponseMessage> responseBuilder,
            Boolean setResponseContent = true)
        {
            if (scimResponse == null)
            {
                throw new ArgumentNullException("scimResponse");
            }

            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException("httpRequestMessage");
            }

            var responseStatusCode = HttpStatusCode.OK;
            if (scimResponse.IsLeft)
            {
                responseStatusCode = GetStatusCode(scimResponse.GetLeft());
            }

            var response = setResponseContent && ShouldSetResponseContent(httpRequestMessage, responseStatusCode)
                ? httpRequestMessage.CreateResponse(responseStatusCode, scimResponse.GetContent())
                : httpRequestMessage.CreateResponse(responseStatusCode);

            if (scimResponse.IsRight && responseBuilder != null)
            {
                responseBuilder.Invoke(scimResponse.GetRight(), response);
            }

            return response;
        }

        private static Object GetContent<T>(this IScimResponse<T> response)
        {
            return response.IsLeft ? (Object)response.GetLeft() : (Object)response.GetRight();
        }

        private static HttpStatusCode GetStatusCode(IEnumerable<ScimError> errors)
        {
            if (errors.Count() == 1) return errors.First().Status;

            return HttpStatusCode.BadRequest;
        }

        private static Boolean ShouldSetResponseContent(HttpRequestMessage httpRequestMessage, HttpStatusCode responseStatusCode)
        {
            return httpRequestMessage.Method != HttpMethod.Head &&
                   responseStatusCode != HttpStatusCode.NoContent &&
                   responseStatusCode != HttpStatusCode.ResetContent &&
                   responseStatusCode != HttpStatusCode.NotModified &&
                   responseStatusCode != HttpStatusCode.Continue;
        }
    }
}