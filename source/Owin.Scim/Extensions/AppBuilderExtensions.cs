namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Configuration;

    using NContext.Configuration;
    using NContext.EventHandling;
    using NContext.Security.Cryptography;

    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Configures the specified <paramref name="appBuilder"/> with a new SCIM 2.0-compliant server. Owin.Scim 
        /// will use the <see cref="Assembly.Location"/> of the calling assembly create a new 
        /// <see cref="System.ComponentModel.Composition.Hosting.CompositionContainer"/>.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="configureScimServerAction">The function used to configure the scim server.</param>
        /// <returns>IAppBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">appBuilder</exception>
        public static IAppBuilder UseScimServer(
            this IAppBuilder appBuilder,
            Action<ScimServerConfiguration> configureScimServerAction = null)
        {
            var callingAssemblyLocation = Assembly.GetCallingAssembly().Location;
            return appBuilder.UseScimServer(
                new Predicate<FileInfo>[]
                {
                    fileInfo => fileInfo.FullName.Equals(callingAssemblyLocation, StringComparison.Ordinal)
                },
                configureScimServerAction);
        }

        /// <summary>
        /// Configures the specified <paramref name="appBuilder"/> with a new SCIM 2.0-compliant server.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="fileCompositionConstraints">The file composition constraints used to create a 
        /// new <see cref="System.ComponentModel.Composition.Hosting.CompositionContainer"/>. Specify 
        /// multiple <see cref="System.IO.FileInfo"/> predicates to include any assemblies which contain
        /// SCIM-related extensibility points.</param>
        /// <param name="configureScimServerAction">The function used to configure the scim server.</param>
        /// <returns>IAppBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">appBuilder</exception>
        public static IAppBuilder UseScimServer(
            this IAppBuilder appBuilder,
            IEnumerable<Predicate<FileInfo>> fileCompositionConstraints,
            Action<ScimServerConfiguration> configureScimServerAction = null)
        {
            if (appBuilder == null)
                throw new ArgumentNullException("appBuilder");
            
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
                .RegisterComponent(() => new ScimApplicationManager(appBuilder, compositionConstraints.Skip(1).ToList(), configureScimServerAction))
                .RegisterComponent<IManageCryptography>()
                    .With<CryptographyManagerBuilder>()
                        .SetDefaults<SHA256Cng, HMACSHA256, AesCryptoServiceProvider>()
                .RegisterComponent<DryIocManager>()
                    .With<DryIocManagerBuilder>()
                .RegisterComponent<IManageEvents>()
                    .With<ScimEventManagerBuilder>();

            Configure.Using(appConfig);

            return appBuilder;
        }
    }
}