namespace Owin.Scim.Registry
{
    using System.ComponentModel.Composition;
    using System.Linq;

    using Configuration;

    using DryIoc;

    using NContext.Extensions;
    using NContext.Security.Cryptography;

    using Repository;
    using Repository.InMemory;

    using Security;

    using Services;

    using Validation;

    public class Registry : IConfigureDryIoc
    {
        private readonly IManageCryptography _CryptograhyManager;

        private readonly ScimServerConfiguration _ServerConfiguration;

        [ImportingConstructor]
        public Registry(
            [Import]IManageCryptography cryptograhyManager,
            [Import]ScimServerConfiguration serverConfiguration)
        {
            _CryptograhyManager = cryptograhyManager;
            _ServerConfiguration = serverConfiguration;
        }

        public int Priority
        {
            get { return int.MaxValue; }
        }

        public void ConfigureContainer(IContainer container)
        {
            container.RegisterDelegate<IProvideHashing>(r => _CryptograhyManager.HashProvider);
            container.Register<ISchemaTypeFactory, DefaultSchemaTypeFactory>(Reuse.Singleton);
            container.Register<IResourceVersionProvider, DefaultResourceVersionProvider>(Reuse.Singleton);
            container.Register<ICanonicalizationService, DefaultCanonicalizationService>(Reuse.Singleton);
            container.Register<IResourceValidatorFactory, ServiceLocatorResourceValidatorFactory>();
            container.Register<IManagePasswords, DefaultPasswordManager>(Reuse.Singleton);

            // register all resource and resource extension validators
            container.Register<ResourceExtensionValidators>(Reuse.Singleton);
            _ServerConfiguration
                .ResourceTypeDefinitions
                .ForEach(rtd =>
                {
                    if (rtd.SchemaExtensions.Any())
                    {
                        rtd.SchemaExtensions
                           .ForEach(ext => container.Register(typeof(IResourceExtensionValidator), ext.ExtensionValidatorType, reuse: Reuse.Singleton));
                    }

                    container.Register(rtd.ValidatorType, reuse: Reuse.Singleton);
                });
            
            container.Register<IUserRepository, InMemoryUserRepository>(Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
            container.Register<IGroupRepository, InMemoryGroupRepository>(Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Keep);

            // register all business logic services for built-in endpoints
            container.Register<IResourceTypeService, ResourceTypeService>(Reuse.Singleton);
            container.Register<IServiceProviderConfigurationService, ServiceProviderConfigurationService>(Reuse.Singleton);
            container.Register<ISchemaService, SchemaService>(Reuse.Singleton);
            container.Register<IUserService, UserService>(Reuse.Singleton);
            container.Register<IGroupService, GroupService>(Reuse.Singleton);
        }
    }
}