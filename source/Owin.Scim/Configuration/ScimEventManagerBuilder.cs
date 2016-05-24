namespace Owin.Scim.Configuration
{
    using NContext.Configuration;
    using NContext.EventHandling;

    public class ScimEventManagerBuilder : ApplicationComponentConfigurationBuilderBase
    {
        public ScimEventManagerBuilder(ApplicationConfigurationBuilder applicationConfigurationBuilder) 
            : base(applicationConfigurationBuilder)
        {
        }

        protected override void Setup()
        {
            Builder.ApplicationConfiguration.RegisterComponent<IManageEvents>(
                () => new EventManager(
                    new DryIocActivationProvider(Builder.ApplicationConfiguration.GetComponent<ScimApplicationManager>().Container)));
        }
    }
}