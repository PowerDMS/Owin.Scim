namespace Owin.Scim.Middleware
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using ErrorHandling;
    using Model;

    /// <summary>
    /// This should pickup all exceptions from any downstream Owin Middleware.
    /// </summary>
    public class ExceptionHandlerMiddleware : OwinMiddleware
    {
        private readonly JsonSerializerSettings _SerializationSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionHandlerMiddleware(OwinMiddleware next) : base(next)
        {
            _SerializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    IgnoreSerializableAttribute = true,
                    IgnoreSerializableInterface = true
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                ScimException responseException = FindScimException(ex);
                context.Response.Headers["Content-Type"] = "application/scim+json; charset=utf-8";
                ScimError scimError;
                int statusCode;

                if (responseException != null)
                {
                    statusCode = (int)responseException.ScimError.Status;
                    scimError = responseException.ScimError;
                }
                else
                {
                    Exception realException = GetFirstRealException(ex);
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    scimError = new ScimError(HttpStatusCode.InternalServerError,
                        detail:
#if DEBUG
                        realException.ToString()
#else
                        realException.Message
#endif
                    );
                }

                context.Response.StatusCode = statusCode;
                context.Response.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(scimError, _SerializationSettings)));
            }
        }

        private static ScimException FindScimException(Exception exception)
        {
            var scimException = exception as ScimException;

            if (scimException == null)
            {
                var aggregateException = exception as AggregateException;

                if (aggregateException != null)
                {
                    foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                    {
                        scimException = IterateForScimException(innerException);

                        if (scimException != null)
                        {
                            return scimException;
                        }
                    }
                }
            }

            return scimException ?? IterateForScimException(exception);
        }

        private static ScimException IterateForScimException(Exception exception)
        {
            ScimException scimException = null;

            if (exception != null)
            {
                scimException = exception as ScimException;

                if (scimException == null)
                {
                    do
                    {
                        scimException = exception.InnerException as ScimException;
                        exception = exception.InnerException;
                    }
                    while (scimException == null && exception != null);
                }
            }

            return scimException;
        }

        private static Exception GetFirstRealException(Exception exception)
        {
            Exception realException = exception;
            var aggregateException = realException as AggregateException;

            if (aggregateException != null)
            {
                realException = aggregateException.Flatten().InnerException; // take first real exception

                while (realException != null && realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }
            }

            return realException ?? exception;
        }
    }
}