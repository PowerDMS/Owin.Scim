namespace Owin.Scim.Configuration
{
    using System.ComponentModel.Composition;

    using DryIoc;

    [InheritedExport]
    public interface IConfigureDryIoc
    {
        /// <summary>
        /// Gets the priority which determines the order in which the instance is executed. The higher the priority, the first it executes.
        /// </summary>
        /// <value>The priority.</value>
        int Priority { get; }

        /// <summary>
        /// Configures the DryIoc <see cref="IContainer"/> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        void ConfigureContainer(IContainer container);
    }
}