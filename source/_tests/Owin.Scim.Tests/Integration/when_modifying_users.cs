namespace Owin.Scim.Tests.Integration
{
    using Configuration;

    using Extensions;

    using Machine.Specifications;

    using Microsoft.Owin.Testing;

    public abstract class when_modifying_users
    {
        Establish context = () =>
        {
            Server = TestServer.Create(app =>
            {
                app.UseScimServer(
                    new ScimServerConfiguration
                    {
                        RequireSsl = false
                    });
            });
        };
        
        Cleanup dispose = () => Server.Dispose();

        protected static TestServer Server;
    }
}