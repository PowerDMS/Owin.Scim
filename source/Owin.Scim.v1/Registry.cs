namespace Owin.Scim.v1
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
            container.Register<IServiceProviderConfigurationService, ServiceProviderConfiguration1Service>(Reuse.Singleton, serviceKey: ScimVersion.One);
            container.Register<ISchemaService, ScimSchema1Service>(Reuse.Singleton, serviceKey: ScimVersion.One);
        }
    }
}