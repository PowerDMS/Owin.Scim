namespace Owin.Scim.Configuration
{
    using System;

    using DryIoc;

    using NContext.Configuration;

    public class DryIocManagerBuilder : ApplicationComponentConfigurationBuilderBase
    {
        private Func<ApplicationConfigurationBase, IContainer> _ContainerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationComponentConfigurationBuilderBase"/> class.
        /// </summary>
        /// <param name="applicationConfigurationBuilder">The application configuration.</param>
        /// <remarks></remarks>
        public DryIocManagerBuilder(ApplicationConfigurationBuilder applicationConfigurationBuilder)
            : base(applicationConfigurationBuilder)
        {
            _ContainerFactory = appConfig => appConfig.GetComponent<ScimApplicationManager>().Container;
        }

        /// <summary>
        /// Sets the <see cref="IContainer"/> instance to use.  By default, NContext will use <see cref="Container"/>.  
        /// </summary>
        /// <param name="containerFactory">The <see cref="IContainer"/> factory.</param>
        /// <returns>This <see cref="DryIocManagerBuilder"/> instance.</returns>
        /// <remarks></remarks>
        public DryIocManagerBuilder SetContainer(Func<ApplicationConfigurationBase, IContainer> containerFactory)
        {
            _ContainerFactory = containerFactory;

            return this;
        }
        
        /// <summary>
        /// Register's an <see cref="DryIocManager"/> application component instance.
        /// </summary>
        /// <remarks></remarks>
        protected override void Setup()
        {
            Builder.ApplicationConfiguration
                   .RegisterComponent<DryIocManager>(
                   () =>
                       new DryIocManager(
                           new DryIocConfiguration(_ContainerFactory)));
        }
    }
}