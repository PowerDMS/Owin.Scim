namespace Owin.Scim.Registry
{
    using Configuration;

    using DryIoc;

    using Repository;
    using Repository.InMemory;

    using Security;

    using Services;

    using Validation.Users;

    public class Registry : IConfigureDryIoc
    {
        public int Priority
        {
            get { return 0; }
        }

        public void ConfigureContainer(IContainer container)
        {
            container.Register<IUserRepository, InMemoryUserRepository>(Reuse.Singleton);
            container.Register<IManagePasswords, DefaultPasswordManager>(Reuse.Singleton);
            container.Register<IVerifyPasswordComplexity, DefaultPasswordComplexityVerifier>(Reuse.Singleton);
            container.Register<UserValidator>(new SingletonReuse());
            container.Register<IUserService, UserService>(Reuse.Singleton);
        }
    }
}