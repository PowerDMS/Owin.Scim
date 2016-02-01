﻿namespace Owin.Scim.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;

    using Configuration;

    using DryIoc;
    using DryIoc.WebApi;

    using Middleware;

    using NContext.Configuration;
    using NContext.EventHandling;
    using NContext.Extensions.Ninject.Configuration;
    using NContext.Security.Cryptography;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseScimServer(this IAppBuilder app, ScimServerConfiguration serverConfig)
        {
            //ncrunch: no coverage start
            if (app == null) throw new ArgumentNullException("app");
            if (serverConfig == null) throw new ArgumentNullException("serverConfig");

            if (serverConfig.RequireSsl)
            {
                app.Use<RequireSslMiddleware>();
            }

            var httpConfig = CreateHttpConfiguration();
            IContainer container = new Container(
                rules =>
                {
                    return rules.WithoutThrowIfDependencyHasShorterReuseLifespan();
                },
                new AsyncExecutionFlowScopeContext())
                .WithWebApi(httpConfig);

            container.RegisterInstance<ScimServerConfiguration>(serverConfig, Reuse.Singleton);
            
            var executionDirectory = Assembly.GetEntryAssembly() == null
                ? AppDomain.CurrentDomain.BaseDirectory
                : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            ApplicationConfiguration appConfig = new ApplicationConfigurationBuilder()
                .ComposeWith(new[] { executionDirectory },
                    fileInfo => 
                    fileInfo.Name.StartsWith("Owin.Scim", StringComparison.OrdinalIgnoreCase) && 
                    (new[] { ".dll" }).Contains(fileInfo.Extension.ToLower()))
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
            
            app.UseWebApi(httpConfig);
            //ncrunch: no coverage end

            return app;
        }
        
        private static HttpConfiguration CreateHttpConfiguration()
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            httpConfiguration.MapHttpAttributeRoutes();

            var settings = httpConfiguration.Formatters.JsonFormatter.SerializerSettings;
            settings.Converters.Add(new StringEnumConverter());
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                IgnoreSerializableAttribute = true,
                IgnoreSerializableInterface = true
            };

            // refer to https://tools.ietf.org/html/rfc7644#section-3.1
            httpConfiguration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/scim+json"));

            return httpConfiguration;
        }
    }
}