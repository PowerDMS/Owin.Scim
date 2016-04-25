namespace Owin.Scim.Extensions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    using Model;
    
    /// <summary>
    /// Defines extension methods for <see cref="IScimResponse{T}"/>.
    /// </summary>
    public static class ScimResponseExtensions
    {
        public static IScimResponse<T2> Bind<T, T2>(this IScimResponse<T> scimResponse, Func<T, IScimResponse<T2>> bindingFunction)
        {
            if (scimResponse.IsLeft)
            {
                return CreateGenericErrorResponse<T, T2>(scimResponse, scimResponse.GetLeft());
            }

            return bindingFunction.Invoke(scimResponse.GetRight());
        }

        public static Task<IScimResponse<TRight2>> BindAsync<TRight, TRight2>(
            this IScimResponse<TRight> scimResponse,
            Func<TRight, Task<IScimResponse<TRight2>>> bindFunc)
        {
            if (scimResponse.IsLeft)
            {
                var tcs = new TaskCompletionSource<IScimResponse<TRight2>>();
                tcs.SetResult(new ScimErrorResponse<TRight2>(scimResponse.GetLeft()));

                return tcs.Task;
            }

            return bindFunc(scimResponse.GetRight());
        }

        public static IScimResponse<TRight> Let<TRight>(this IScimResponse<TRight> scimResponse, Action<TRight> action)
        {
            if (scimResponse.IsRight)
            {
                action.Invoke(scimResponse.GetRight());
            }

            return scimResponse;
        }

        internal static IScimResponse<T2> CreateGenericErrorResponse<T, T2>(IScimResponse<T> originalResponse, ScimError error)
        {
            if (IsBuiltInErrorResponse(originalResponse))
            {
                return new ScimErrorResponse<T2>(error);
            }

            try
            {
                return Activator.CreateInstance(
                    originalResponse.GetType()
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(typeof(T2)),
                    error) as IScimResponse<T2>;
            }
            catch (TargetInvocationException)
            {
                // No supportable constructor found! Return default.
                return new ScimErrorResponse<T2>(error);
            }
        }

        private static Boolean IsBuiltInDataResponse<T>(IScimResponse<T> originalResponse)
        {
            var typeInfo = originalResponse.GetType().GetTypeInfo();
            return (originalResponse is ScimDataResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(ScimDataResponse<>).GetTypeInfo().IsAssignableFrom(typeInfo.GetGenericTypeDefinition().GetTypeInfo()));
        }

        private static Boolean IsBuiltInErrorResponse<T>(IScimResponse<T> originalResponse)
        {
            var typeInfo = originalResponse.GetType().GetTypeInfo();
            return (originalResponse is ScimErrorResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(ScimErrorResponse<>).GetTypeInfo().IsAssignableFrom(typeInfo.GetGenericTypeDefinition().GetTypeInfo()));
        }

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

            var response = ShouldSetResponseContent(httpRequestMessage, responseStatusCode)
                ? httpRequestMessage.CreateResponse(responseStatusCode, scimResponse.GetContent())
                : httpRequestMessage.CreateResponse(responseStatusCode);

            return response;
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

        private static HttpStatusCode GetStatusCode(ScimError error)
        {
            if (error != null) return error.Status;

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