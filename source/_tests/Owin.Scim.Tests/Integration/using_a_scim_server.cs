namespace Owin.Scim.Tests.Integration
{
    using System;
    using System.Threading;

    using Configuration;

    using Extensions;

    using Microsoft.Owin.Testing;

    public abstract class using_a_scim_server
    {
        protected static TestServer Server
        {
            get { return _Server.Value; }
        }

        private static readonly Lazy<TestServer> _Server = 
            new Lazy<TestServer>(() =>
            {
                return TestServer.Create(app =>
                {
                    app.UseScimServer(
                        new ScimServerConfiguration
                        {
                            RequireSsl = false
                        });
                });
            }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
}