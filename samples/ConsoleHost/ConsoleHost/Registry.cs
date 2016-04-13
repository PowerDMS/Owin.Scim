namespace ConsoleHost
{
    using DryIoc;

    using Owin.Scim.Configuration;
    using Owin.Scim.Repository;

    public class Registry : IConfigureDryIoc
    {
        /// <summary>
        /// Gets the priority which determines the order in which the instance is executed. The higher the priority, the first it executes.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Configures the DryIoc <see cref="T:DryIoc.IContainer" /> instance.
        /// </summary>
        /// <param name="container">The container.</param>
        public void ConfigureContainer(IContainer container)
        {
            // Since the priority is lower than Owin.Scim's default registry, this method will execute after all
            // default registrations occur.

            // Unregister any IUserRepository implementation and add our own.
            container.Unregister<IUserRepository>();
            container.Register<IUserRepository, CustomInMemoryUserRepository>();
        }
    }
}