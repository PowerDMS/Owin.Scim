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
                var responseException = ex as ScimException;

                context.Response.Headers["Content-Type"] = "application/scim+json; charset=utf-8";

                if (responseException != null)
                {
                    context.Response.StatusCode = (int)responseException.ScimError.Status;
                    context.Response.Write(
                        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseException.ScimError,
                            _SerializationSettings)));
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

#if DEBUG
                    Byte[] responseBody = Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new ScimError(HttpStatusCode.InternalServerError,
                                detail: String.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace))
                            , _SerializationSettings));
#else
                Byte[] responseBody = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new ScimError(HttpStatusCode.InternalServerError, detail: ex.Message)
                        , _SerializationSettings));
#endif
                    context.Response.Write(responseBody);
                }
            }
        }
    }
}