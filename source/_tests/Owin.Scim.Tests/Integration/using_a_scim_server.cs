namespace Owin.Scim.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Configuration;

    using Machine.Specifications;

    using Microsoft.Owin.Testing;

    using Model;

    using Scim.Extensions;

    public class using_a_scim_server : IAssemblyContext
    {
        protected static TestServer Server
        {
            get { return _Server; }
        }

        public void OnAssemblyStart()
        {
            //ncrunch: no coverage start
            if (_Server != null) return;

            _Server = TestServer.Create(app =>
            {
                app.UseScimServer(
                    new ScimServerConfiguration
                    {
                        RequireSsl = false,
                        PublicOrigin = "https://helloworld.org/scim/v2"
                    }
                    .AddCompositionConditions(
                        fileInfo => fileInfo.Name.StartsWith("Sample.Host.Console", StringComparison.OrdinalIgnoreCase))
                    .AddAuthenticationScheme(
                        new AuthenticationScheme(
                            "oauthbearertoken",
                            "OAuth Bearer Token",
                            "Authentication scheme using the OAuth Bearer Token standard.", 
                            specUri: new Uri("https://tools.ietf.org/html/rfc6750"),
                            isPrimary: true))
                    .ConfigureETag(true, true));
            });
            //ncrunch: no coverage end
        }

        public void OnAssemblyComplete()
        {
            if (_Server == null) return;
            
            _Server.Dispose();
        }

        private static TestServer _Server;
    }
}