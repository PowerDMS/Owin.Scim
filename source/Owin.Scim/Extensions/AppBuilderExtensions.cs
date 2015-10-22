namespace Owin.Scim.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Dependencies;

    using Configuration;

    using DryIoc;
    using DryIoc.WebApi;

    using Middleware;

    using NContext.Configuration;
    using NContext.EventHandling;
    using NContext.Extensions.AutoMapper.Configuration;
    using NContext.Extensions.Ninject.Configuration;
    using NContext.Security.Cryptography;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseScimServer(this IAppBuilder app, ScimServerConfiguration serverConfig)
        {
            if (app == null) throw new ArgumentNullException("app");
            if (serverConfig == null) throw new ArgumentNullException("serverConfig");

            if (serverConfig.RequireSsl)
            {
                app.Use<RequireSslMiddleware>();
            }

            var container = new Container(rules =>
            {
                return rules.WithoutThrowIfDependencyHasShorterReuseLifespan();
            });

            var executionDirectory = Assembly.GetEntryAssembly() == null
                ? AppDomain.CurrentDomain.BaseDirectory
                : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Defining a composition directory just to make testing easier with ncrunch
            var compositionDirectory = executionDirectory.Contains("_tests")
                ? executionDirectory.Substring(0, executionDirectory.IndexOf("_tests"))
                : executionDirectory;

            ApplicationConfiguration appConfig = new ApplicationConfigurationBuilder()
                .ComposeWith(new[] { compositionDirectory },
                    fileInfo => 
                    fileInfo.Name.StartsWith("Owin.Scim", StringComparison.OrdinalIgnoreCase) && 
                    (new[] { ".dll" }).Contains(fileInfo.Extension.ToLower()))
                .RegisterComponent<IManageCryptography>()
                    .With<CryptographyManagerBuilder>()
                        .SetDefaults<SHA256Cng, HMACSHA256, AesCryptoServiceProvider>()
                .RegisterComponent<DryIocManager>()
                    .With<DryIocManagerBuilder>()
                        .SetContainer(() => container)
                .RegisterComponent<IManageAutoMapper>(() => new AutoMapperManager())
                .RegisterComponent<IManageEvents>()
                    .With<EventManagerBuilder>()
                        .SetActivationProvider(() => new DryIocActivationProvider(container));

            Configure.Using(appConfig);

            var httpConfig = CreateHttpConfiguration(new DryIocDependencyResolver(container));

            container.WithWebApi(httpConfig);
            app.UseWebApi(httpConfig);

            return app;
        }

        private static HttpConfiguration CreateHttpConfiguration(IDependencyResolver dependencyResolver)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            httpConfiguration.MapHttpAttributeRoutes();
            httpConfiguration.DependencyResolver = dependencyResolver;

            var settings = httpConfiguration.Formatters.JsonFormatter.SerializerSettings;
            settings.Converters.Add(new StringEnumConverter());
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                IgnoreSerializableAttribute = true,
                IgnoreSerializableInterface = true
            };

            return httpConfiguration;
        }
    }
}