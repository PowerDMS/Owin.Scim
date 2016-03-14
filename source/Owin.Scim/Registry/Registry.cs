﻿namespace Owin.Scim.Registry
{
    using System.ComponentModel.Composition;
    using System.Linq;

    using Canonicalization;

    using Configuration;

    using DryIoc;

    using NContext.Extensions;
    using NContext.Security.Cryptography;

    using Repository;
    using Repository.InMemory;

    using Security;

    using Services;

    using Validation;
    using Validation.Users;

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
            get { return -1; }
        }

        public void ConfigureContainer(IContainer container)
        {
            container.RegisterDelegate<IProvideHashing>(r => _CryptograhyManager.HashProvider);
            container.Register<ISchemaTypeFactory, DefaultSchemaTypeFactory>(Reuse.Singleton);
            container.Register<IManagePasswords, DefaultPasswordManager>(Reuse.Singleton);
            container.Register<IVerifyPasswordComplexity, DefaultPasswordComplexityVerifier>(Reuse.Singleton);
            container.Register<IResourceVersionProvider, DefaultResourceVersionProvider>(Reuse.Singleton);
            container.Register<IResourceValidatorFactory, ServiceLocatorResourceValidatorFactory>();
            container.Register<DefaultCanonicalizationService>(Reuse.Singleton);

            // validation
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

#if DEBUG
            // TODO: (CY) because of this, should I set priority higher? so my custom IConfigureDryIoc runs first
            container.Register<IUserRepository, InMemoryUserRepository>(Reuse.Singleton, ifAlreadyRegistered:IfAlreadyRegistered.Keep);
            container.Register<IGroupRepository, InMemoryGroupRepository>(Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Keep);
#endif
            container.Register<IUserService, UserService>(
                made: Made.Of(propertiesAndFields: PropertiesAndFields.Auto));

            container.Register<IGroupService, GroupService>(
                made: Made.Of(propertiesAndFields: PropertiesAndFields.Auto));
        }
    }
}