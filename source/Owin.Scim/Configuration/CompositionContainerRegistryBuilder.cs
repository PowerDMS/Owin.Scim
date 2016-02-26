namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    using NContext.Configuration;

    public class CompositionContainerRegistryBuilder : ApplicationComponentConfigurationBuilderBase
    {
        private readonly IList<object> _Entries = 
            new List<object>();
        
        public CompositionContainerRegistryBuilder(ApplicationConfigurationBuilder applicationConfigurationBuilder) 
            : base(applicationConfigurationBuilder)
        {
        }

        public CompositionContainerRegistryBuilder AddComposableInstance<T>(T instance)
        {
            _Entries.Add(instance);
            return this;
        }

        protected override void Setup()
        {
            Builder.ApplicationConfiguration
                .RegisterComponent<CompositionContainerRegistry>(
                    () => new CompositionContainerRegistry(_Entries));
        }
    }
}