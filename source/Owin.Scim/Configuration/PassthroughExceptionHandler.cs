namespace Owin.Scim.Configuration
{
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;

    /// <summary>
    /// This disables WebApi exception handling
    /// ref https://www.jayway.com/2016/01/08/improving-error-handling-asp-net-web-api-2-1-owin/
    /// </summary>
    public class PassthroughExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var info = ExceptionDispatchInfo.Capture(context.Exception);
            info.Throw();

            return Task.FromResult(0);
        }
    }
}