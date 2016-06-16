namespace ConsoleHost
{
    using AutoMapper;

    using DryIoc;

    using Kernel;

    using Owin.Scim.Configuration;
    using Owin.Scim.Model.Users;
    using Owin.Scim.Repository;
    using Owin.Scim.v2.Model;

    using Scim;

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

            // Configure AutoMapper to translate ScimUser and your application's user.
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.CreateMap<KernelUser, ScimUser2>();
                    config.CreateMap<ScimUser, KernelUser>()
                        .ReverseMap()
                        .Include<KernelUser, ScimUser2>();
                });
            container.RegisterDelegate<IMapper>(resolver => mapperConfiguration.CreateMapper());

            // Replace any IUserRepository implementation with our own.
            container.Register<IUserRepository, CustomUserRepository>(ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        }
    }
}