namespace Owin.Scim.Model
{
    using Configuration;

    public class ServiceProviderConfigDefinition : ScimTypeDefinitionBuilder<ServiceProviderConfig>
    {
        public ServiceProviderConfigDefinition()
        {
            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(spc => spc.Id)
                .SetReturned(Returned.Never);

            For(spc => spc.ExternalId)
                .SetReturned(Returned.Never);
        }
    }
}