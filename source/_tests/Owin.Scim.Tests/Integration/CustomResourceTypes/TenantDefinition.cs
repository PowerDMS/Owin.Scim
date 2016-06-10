namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using Configuration;

    public class TenantDefinition : ScimResourceTypeDefinitionBuilder<Tenant>
    {
        public TenantDefinition(ScimServerConfiguration serverConfiguration)
            : base(
                  serverConfiguration,
                  "Tenant",
                  CustomSchemas.Tenant,
                  "tenants",
                  typeof(TenantValidator),
                  schemaIdentifiers => schemaIdentifiers.Contains(CustomSchemas.Tenant))
        {
            AddSchemaExtension<SalesForceExtension, SalesForceExtensionValidator>(CustomSchemas.SalesForceExtension, true);

            For(tenant => tenant.Name)
                .SetRequired(true)
                .SetDescription("The name of the tenant.");
        }
    }
}