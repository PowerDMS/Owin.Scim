namespace Owin.Scim.Tests.Integration
{
    using System;

    using Configuration;

    using Machine.Specifications;

    using Microsoft.Owin.Testing;

    using Model;
    using Model.Users;

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
                    .ConfigureETag(true, true)

                    // Below will eventually be moved inside ScimServerConfiguration to add core schema 2.0 resource types.
                    // This is an example of the extensibility to define new resource types, canonicalization rules, 
                    // validation rules, attribute behaviors which will determine how Owin.Scim verifies a request.
                    .AddOrModifyResourceType<User>(ScimConstants.ResourceTypes.User, ScimConstants.Schemas.User, ScimConstants.Endpoints.Users)
                        .SetDescription("User accounts")
                        .For(u => u.Id)
                            .SetDescription("Unique identifier for the user.")
                            .SetMutability(Mutable.ReadOnly)
                            .SetReturned(Return.Always)
                            .SetUniqueness(Unique.Server)
                            .SetCaseExact(true)
                        .For(u => u.Password)
                            .SetDescription(@"The user's cleartext password. This attribute is intended to be used as a means to specify an initial password when creating a new User or to reset an existing user's password.")
                            .SetMutability(Mutable.WriteOnly)
                            .SetReturned(Return.Never)

                        // Canonicalization Support?
//                        .For(u => u.Locale)
//                            .AddCanonicalizationRules(
//                                (User user) =>
//                                {
//                                    if (!string.IsNullOrWhiteSpace(user.Locale))
//                                    {
//                                        user.Locale = user.Locale.Replace('_', '-'); // Supports backwards compatability
//                                    }
//                                })
                        .For(u => u.Addresses)
                        .For(u => u.Emails)
                            .SetDescription("")
                            .ForSubAttributes(mva => mva
                                .For(e => e.Display)
                                    .SetMutability(Mutable.ReadOnly))
//                            .AddCanonicalizationRules(
//                                (Email attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
//                                (Email attribute, ref object state) =>
//                                {
//                                    if (string.IsNullOrWhiteSpace(attribute.Value)) return;

//                                    var atIndex = attribute.Value.IndexOf('@') + 1;
//                                    if (atIndex == 0) return; // IndexOf returned -1

//                                    var cEmail = attribute.Value.Substring(0, atIndex) + attribute.Value.Substring(atIndex).ToLower();
//                                    attribute.Display = cEmail;
//                                },
//                                (Email attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state))

                // SUPPORT SCHEMA EXTENSIONS!
//                        .AddOrModifySchemaExtension<EnterpriseUser, EnterpriseUserExtension>(ScimConstants.Schemas.UserEnterprise, true)
//                            .ForMember(eu => eu.EmployeeNumber)
//                                .SetDescription("A string identifier, typically numeric or alphanumeric, assigned to a person, typically based on order of hire or association with an organization.")
                );
            });
            // ncrunch: no coverage end
        }

        public void OnAssemblyComplete()
        {
            if (_Server == null) return;
            
            _Server.Dispose();
        }

        private static TestServer _Server;
    }
}