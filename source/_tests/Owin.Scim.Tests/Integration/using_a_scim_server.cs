namespace Owin.Scim.Tests.Integration
{
    using System;

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
        private static volatile object _SyncLock = new object();

        Establish context = () =>
        {
            if (_Server != null) return;

            lock (_SyncLock)
            {
                if (_Server != null) return;

                _Server = TestServer.Create(app =>
                {
                    app.UseScimServer(
                        null,
                        configuration =>
                        {
                            ClientJsonFormatter = new ScimClientJsonMediaTypeFormatter(configuration);
                            configuration.RequireSsl = false;

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
        };

        protected static TestServer Server
        {
            get { return _Server; }
        }

        public void OnAssemblyStart()
        {
        }

        private static void ModifyUserResourceType(ScimResourceTypeDefinitionBuilder<User> builder)
        {
            // this adds custom schemas, need play with custom validation next
            builder.AddSchemaExtension<MyUserSchema, MyUserSchemaValidator>(MyUserSchema.Schema);
        }

        private static void ModifyGroupResourceType(ScimResourceTypeDefinitionBuilder<Group> builder)
        {
            // this adds custom schemas, need play with custom validation next
            builder.AddSchemaExtension<MyGroupSchema, MyGroupSchemaValidator>(MyGroupSchema.Schema);
        }

        public void OnAssemblyComplete()
        {
            if (_Server == null) return;
            
            _Server.Dispose();
        }
        
        public static ScimClientJsonMediaTypeFormatter ClientJsonFormatter;

        private static TestServer _Server;
    }
}