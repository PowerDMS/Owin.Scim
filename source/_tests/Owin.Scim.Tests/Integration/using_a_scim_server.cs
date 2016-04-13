namespace Owin.Scim.Tests.Integration
{
    using System;
    using System.IO;

    using Configuration;

    using Machine.Specifications;

    using Microsoft.Owin.Testing;

    using Model;
    using Model.Groups;
    using Model.Users;

    using SchemaExtensions;

    using Scim.Extensions;

    public class using_a_scim_server : IAssemblyContext
    {
        protected static TestServer Server
        {
            get { return _Server; }
        }

        public void OnAssemblyStart()
        {
            if (_Server != null) return;

            _Server = TestServer.Create(app =>
            {
                app.UseScimServer(
                    null,
                    configuration =>
                    {
                        configuration.RequireSsl = false;
                        configuration.PublicOrigin = new Uri("https://helloworld.org/scim/v2");

                        configuration
                            .AddAuthenticationScheme(
                                new AuthenticationScheme(
                                    "oauthbearertoken",
                                    "OAuth Bearer Token",
                                    "Authentication scheme using the OAuth Bearer Token standard.",
                                    specUri: new Uri("https://tools.ietf.org/html/rfc6750"),
                                    isPrimary: true))
                            .ConfigureETag(supported: true, isWeak: true)
                            .ModifyResourceType<User>(ModifyUserResourceType)
                            .ModifyResourceType<Group>(ModifyGroupResourceType);
                    });
            });
        }

        private void ModifyUserResourceType(ScimResourceTypeDefinitionBuilder<User> builder)
        {
            // this adds custom schemas, need play with custom validation next
            builder.AddSchemaExtension<MyUserSchema, MyUserSchemaValidator>(MyUserSchema.Schema);
        }

        private void ModifyGroupResourceType(ScimResourceTypeDefinitionBuilder<Group> builder)
        {
            // this adds custom schemas, need play with custom validation next
            builder.AddSchemaExtension<MyGroupSchema, MyGroupSchemaValidator>(MyGroupSchema.Schema);
        }

        public void OnAssemblyComplete()
        {
            if (_Server == null) return;
            
            _Server.Dispose();
        }

        private static TestServer _Server;
    }
}