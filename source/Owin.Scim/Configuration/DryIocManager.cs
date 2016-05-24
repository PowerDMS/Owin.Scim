namespace Owin.Scim.Configuration
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Linq.Expressions;

    using DryIoc;

    using NContext.Configuration;
    using NContext.Extensions;

    /// <summary>
    /// Defines a dependency injection application component using DryIoc.
    /// </summary>
    public class DryIocManager : IApplicationComponent
    {
        private readonly DryIocConfiguration _Configuration;

        private Boolean _IsConfigured;

        private IContainer _Container;
    
        public DryIocManager(DryIocConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _Configuration = configuration;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is configured.
        /// </summary>
        /// <remarks></remarks>
        public Boolean IsConfigured
        {
            get
            {
                return _IsConfigured;
            }
            protected set
            {
                _IsConfigured = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IContainer"/> instance.
        /// </summary>
        /// <remarks></remarks>
        public IContainer Container
        {
            get
            {
                return _Container;
            }
        }

        /// <summary>
        /// Configures the component instance. This method should set <see cref="IApplicationComponent.IsConfigured"/>.
        /// </summary>
        /// <param name="applicationConfiguration">The application configuration.</param>
        /// <remarks>
        /// </remarks>
        public virtual void Configure(ApplicationConfigurationBase applicationConfiguration)
        {
            if (_IsConfigured)
            {
                return;
            }

            _Container = _Configuration.CreateContainer(applicationConfiguration);

            applicationConfiguration.CompositionContainer.ComposeExportedValue<IContainer>(_Container);
            applicationConfiguration.CompositionContainer.ComposeExportedValue<DryIocManager>(this);
            Container.RegisterInstance<CompositionContainer>(applicationConfiguration.CompositionContainer, Reuse.Singleton);
            Container.RegisterDelegate(resolver => (IContainer)resolver, setup: Setup.With(allowDisposableTransient: true));

            applicationConfiguration.CompositionContainer
                .GetExportedValues<IConfigureDryIoc>()
                .OrderByDescending(configurable => configurable.Priority)
                .ForEach(configurable => configurable.ConfigureContainer(Container));

            _IsConfigured = true;
        }
    }
}