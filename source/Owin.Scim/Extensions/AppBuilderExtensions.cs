namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Configuration;

    using DryIoc;

    using NContext.Configuration;
    using NContext.EventHandling;
    using NContext.Security.Cryptography;

    using Services;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseScimServer(
            this IAppBuilder appBuilder,
            IEnumerable<Predicate<FileInfo>> fileCompositionConstraints,
            Action<ScimServerConfiguration> configureScimServerAction)
        {
            if (appBuilder == null)
                throw new ArgumentNullException("appBuilder");

            appBuilder.Use((context, task) =>
            {
                AmbientRequestMessageService.SetRequestInformation(context);
                return task.Invoke();
            });

            IContainer container = new Container(
                rules => rules.WithoutThrowIfDependencyHasShorterReuseLifespan(),
                new AsyncExecutionFlowScopeContext());
           
            var executionDirectory = Assembly.GetEntryAssembly() == null
                ? AppDomain.CurrentDomain.BaseDirectory
                : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var compositionConstraints = new List<Predicate<FileInfo>>
            {
                fileInfo =>
                    fileInfo.Name.StartsWith("Owin.Scim", StringComparison.OrdinalIgnoreCase) &&
                    new[] { ".dll" }.Contains(fileInfo.Extension.ToLower())
            };

            if (fileCompositionConstraints != null)
                compositionConstraints.AddRange(fileCompositionConstraints);
            
            ApplicationConfiguration appConfig = new ApplicationConfigurationBuilder()
                .ComposeWith(new[] { executionDirectory }, compositionConstraints.ToArray())
                .RegisterComponent(() => new ScimApplicationManager(appBuilder, container, configureScimServerAction))
                .RegisterComponent<IManageCryptography>()
                    .With<CryptographyManagerBuilder>()
                        .SetDefaults<SHA256Cng, HMACSHA256, AesCryptoServiceProvider>()
                .RegisterComponent<DryIocManager>()
                    .With<DryIocManagerBuilder>()
                        .SetContainer(() => container)
                .RegisterComponent<IManageEvents>()
                    .With<EventManagerBuilder>()
                        .SetActivationProvider(() => new DryIocActivationProvider(container));

            Configure.Using(appConfig);

            return appBuilder;
        }
    }
}