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

    using v2.Model;

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
                            // only required for using Owin.Scim types as a client for serialization
                            // unfortunately, Owin.Scim models are not pure POCOs in order to support
                            // resource extensions
                            ClientJsonFormatter = new ScimClientJsonMediaTypeFormatter(configuration);

                            configuration.RequireSsl = false;
                            configuration.EnableEndpointAuthorization = false;

                            configuration
                                .AddAuthenticationScheme(
                                    new AuthenticationScheme(
                                        "oauthbearertoken",
                                        "OAuth Bearer Token",
                                        "Authentication scheme using the OAuth Bearer Token standard.",
                                        specUri: new Uri("https://tools.ietf.org/html/rfc6750"),
                                        isPrimary: true))
                                .ModifyResourceType<ScimUser2>(ModifyUserResourceType)
                                .ModifyResourceType<ScimGroup2>(ModifyGroupResourceType);
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

        private static void ModifyUserResourceType(ScimResourceTypeDefinitionBuilder<ScimUser2> builder)
        {
            // this adds custom schemas, need play with custom validation next
            builder.AddSchemaExtension<MyUserSchema, MyUserSchemaValidator>(MyUserSchema.Schema);
        }

        private static void ModifyGroupResourceType(ScimResourceTypeDefinitionBuilder<ScimGroup2> builder)
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