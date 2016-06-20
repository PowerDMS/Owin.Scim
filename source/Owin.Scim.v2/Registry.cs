namespace Owin.Scim.v2
{
    using Configuration;

    using DryIoc;

    using Scim.Model;
    using Scim.Services;

    using Services;

    public class Registry : IConfigureDryIoc
    {
        public int Priority
        {
            get { return int.MaxValue; }
        }

        public void ConfigureContainer(IContainer container)
        {
            container.Register<IResourceTypeService, ResourceTypeService>(Reuse.Singleton);
            container.Register<IServiceProviderConfigurationService, ServiceProviderConfiguration2Service>(Reuse.Singleton, serviceKey: ScimVersion.Two);
            container.Register<ISchemaService, ScimSchema2Service>(Reuse.Singleton, serviceKey: ScimVersion.Two);
        }
    }
}