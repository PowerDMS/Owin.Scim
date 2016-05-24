namespace Owin.Scim.Configuration
{
    using System;

    using DryIoc;

    using NContext.Configuration;

    public class DryIocConfiguration
    {
        private readonly Func<ApplicationConfigurationBase, IContainer> _ContainerFactory;

        public DryIocConfiguration(Func<ApplicationConfigurationBase, IContainer> containerFactory)
        {
            _ContainerFactory = containerFactory;
        }

        /// <summary>
        /// Creates the <see cref="IContainer"/> instance.
        /// </summary>
        /// <returns>Instance of <see cref="IContainer"/>.</returns>
        /// <remarks></remarks>
        public virtual IContainer CreateContainer(ApplicationConfigurationBase applicationConfiguration)
        {
            return _ContainerFactory.Invoke(applicationConfiguration);
        }
    }
}