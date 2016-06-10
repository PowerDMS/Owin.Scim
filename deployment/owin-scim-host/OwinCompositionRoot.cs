using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(owin_scim_host.OwinCompositionRoot))]

namespace owin_scim_host
{
    using Owin.Scim.Extensions;

    public class OwinCompositionRoot
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseScimServer(config =>
            {
                config.RequireSsl = false;
                config.EnableEndpointAuthorization = false;
            });
        }
    }
}