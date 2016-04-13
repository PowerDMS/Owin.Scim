namespace Owin.Scim.Model
{
    using Configuration;

    public class ServiceProviderConfigDefinition : ScimTypeDefinitionBuilder<ServiceProviderConfig>
    {
        public ServiceProviderConfigDefinition()
        {
            SetName("Service Provider Configuration");
            SetDescription("Schema for representing the service provider's configuration");

            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(spc => spc.Id)
                .SetReturned(Returned.Never);

            For(spc => spc.ExternalId)
                .SetReturned(Returned.Never);
        }
    }
}