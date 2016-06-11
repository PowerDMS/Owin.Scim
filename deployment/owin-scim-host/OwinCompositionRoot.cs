using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(owin_scim_host.OwinCompositionRoot))]

namespace owin_scim_host
{
    using System.Linq;
    using System.Web.Http;

    using DryIoc.WebApi;

    using Microsoft.Owin.Extensions;

    using Owin.Scim.Configuration;
    using Owin.Scim.Endpoints;
    using Owin.Scim.Extensions;

    public class OwinCompositionRoot
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/scim", builder =>
            {
                builder.UseScimServer(config =>
                {
                    config.RequireSsl = false;
                    config.EnableEndpointAuthorization = false;
                });
            });

            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}