namespace Owin.Scim.Model
{
    using Configuration;

    public class ServiceProviderConfigurationDefinition : ScimSchemaTypeDefinitionBuilder<ServiceProviderConfiguration>
    {
        public ServiceProviderConfigurationDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration, ScimConstants.Schemas.ServiceProviderConfig)
        {
            SetName("Service Provider Configuration");
            SetDescription("Schema for representing the service provider's configuration");

            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(spc => spc.Id)
                .SetReturned(Returned.Never);

            For(spc => spc.ExternalId)
                .SetReturned(Returned.Never);

            For(spc => spc.DocumentationUri)
                .SetDescription(@"An HTTP-addressable URL pointing to the service provider's human-consumable help documentation.")
                .SetMutability(Mutability.ReadOnly)
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External);

            For(spc => spc.Patch)
                .SetDescription(@"A complex type that specifies PATCH configuration options.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.Bulk)
                .SetDescription(@"A complex type that specifies bulk configuration options.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.Filter)
                .SetDescription(@"A complex type that specifies FILTER options.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.ChangePassword)
                .SetDescription(@"A complex type that specifies configuration options related to changing a password.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.Sort)
                .SetDescription(@"A complex type that specifies Sort configuration options.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.ETag)
                .SetDescription(@"A complex type that specifies ETag configuration options.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(spc => spc.AuthenticationSchemes)
                .SetDescription(@"A multi-valued complex type that specifies supported authentication scheme properties.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);
        }
    }
}