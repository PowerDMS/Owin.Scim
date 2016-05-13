namespace Owin.Scim.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    using Model;

    using Newtonsoft.Json;

    internal class RequireSslMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _Next;

        public RequireSslMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _Next = next;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            var context = new OwinContext(env);

            if (!context.Request.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 403;

                await context.Response.WriteAsync(
                    Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(
                            new ScimError(HttpStatusCode.Forbidden))));

                return;
            }

            await _Next(env);
        }
    }
}