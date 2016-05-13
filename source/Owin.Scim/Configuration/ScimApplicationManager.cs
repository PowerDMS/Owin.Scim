namespace Owin.Scim.Configuration
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Dispatcher;

    using DryIoc;
    using DryIoc.WebApi;

    using Endpoints;

    using Middleware;

    using Model;

    using NContext.Common;
    using NContext.Configuration;
    using NContext.Extensions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Querying;

    using Security;

    using Serialization;

    using Services;

    public class ScimApplicationManager : IApplicationComponent
    {
        private readonly IAppBuilder _AppBuilder;

        private readonly IContainer _Container;

        private readonly Action<ScimServerConfiguration> _ConfigureScimServerAction;

        private bool _IsConfigured;

        public ScimApplicationManager(
            IAppBuilder appBuilder,
            IContainer container,
            Action<ScimServerConfiguration> configureScimServerAction)
        {
            _AppBuilder = appBuilder;
            _Container = container;
            _ConfigureScimServerAction = configureScimServerAction;
        }

        public bool IsConfigured
        {
            get { return _IsConfigured; }
            private set { _IsConfigured = value; }
        }

        public void Configure(ApplicationConfigurationBase applicationConfiguration)
        {
            if (IsConfigured) return;

            // my catch all exception handler
            _AppBuilder.Use<ExceptionHandlerMiddleware>();

            var serverConfiguration = new ScimServerConfiguration();
            applicationConfiguration.CompositionContainer.ComposeExportedValue(serverConfiguration);
            _Container.RegisterInstance(serverConfiguration, Reuse.Singleton);
            
            _AppBuilder.Use((context, task) =>
            {
                AmbientRequestService.SetRequestInformation(context, serverConfiguration);

                return task.Invoke();
            });

            // discover and register all type definitions
            var owinScimAssembly = Assembly.GetExecutingAssembly();
            var typeDefinitions = applicationConfiguration.CompositionContainer.GetExportTypesThatImplement<IScimTypeDefinition>();
            foreach (var typeDefinition in typeDefinitions)
            {
                Type distinctTypeDefinition;
                var typeDefinitionTarget = GetTargetDefinitionType(typeDefinition); // the type of object being defined (e.g. User, Group, Name)
                if (serverConfiguration.TypeDefinitionRegistry.TryGetValue(typeDefinitionTarget, out distinctTypeDefinition))
                {
                    // already have a definition registered for the target type
                    // let's favor non-Owin.Scim definitions over built-in defaults
                    if (distinctTypeDefinition.Assembly == owinScimAssembly && typeDefinition.Assembly != owinScimAssembly)
                        serverConfiguration.TypeDefinitionRegistry[typeDefinitionTarget] = typeDefinition;

                    continue;
                }

                // register type definition
                serverConfiguration.TypeDefinitionRegistry[typeDefinitionTarget] = typeDefinition;
            }

            // instantiate an instance of each type definition and add it to configuration
            // type definitions MAY instantiate new type definitions themselves during attribute definition composition
            var enumerator = serverConfiguration.TypeDefinitionRegistry.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                // creating type definitions may be expensive due to reflection
                // when a type definition is instantiated, it may implicitly instantiate/register other type 
                // definitions for complex attributes, therefore, no need to re-create the same definition more than once
                if (serverConfiguration.ContainsTypeDefinition(enumerator.Current))
                    continue;

                var typeDefinition = (IScimTypeDefinition)enumerator.Current.CreateInstance(serverConfiguration);
                serverConfiguration.AddTypeDefiniton(typeDefinition);
            }

            var httpConfiguration = serverConfiguration.HttpConfiguration = new HttpConfiguration();

            // Invoke custom configuration action if not null
            if (_ConfigureScimServerAction != null)
                _ConfigureScimServerAction.Invoke(serverConfiguration);

            // Configure SCIM http configuration
            ConfigureHttpConfiguration(serverConfiguration);

            if (serverConfiguration.RequireSsl)
                _AppBuilder.Use<RequireSslMiddleware>();
            
            _Container.WithWebApi(httpConfiguration);
            _AppBuilder.UseWebApi(httpConfiguration);

            IsConfigured = true;
        }

        private Type GetTargetDefinitionType(Type typeDefinition)
        {
            Type genericTypeDefinition;
            var baseType = typeDefinition.BaseType;
            if (baseType == null)
                throw new Exception("Invalid type defintion. Must inherit from either ScimResourceTypeDefinitionBuilder or ScimTypeDefinitionBuilder.");

            while (!baseType.IsGenericType ||
                (((genericTypeDefinition = baseType.GetGenericTypeDefinition()) != typeof(ScimResourceTypeDefinitionBuilder<>)) && 
                 genericTypeDefinition != typeof(ScimTypeDefinitionBuilder<>)))
            {
                if (baseType.BaseType == null)
                    throw new Exception("Invalid type defintion. Must inherit from either ScimResourceTypeDefinitionBuilder or ScimTypeDefinitionBuilder.");

                baseType = baseType.BaseType;
            }

            return baseType.GetGenericArguments()[0];
        }

        private static void ConfigureHttpConfiguration(ScimServerConfiguration serverConfiguration)
        {
            var httpConfiguration = serverConfiguration.HttpConfiguration;
            httpConfiguration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            httpConfiguration.MapHttpAttributeRoutes();

            var settings = httpConfiguration.Formatters.JsonFormatter.SerializerSettings;
            settings.ContractResolver = new ScimContractResolver(serverConfiguration)
            {
                IgnoreSerializableAttribute = true,
                IgnoreSerializableInterface = true
            };
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new ScimQueryOptionsConverter(serverConfiguration));
            settings.Converters.Add(new ResourceJsonConverter(serverConfiguration, JsonSerializer.Create(settings)));

            httpConfiguration.Filters.Add(new ScimAuthorizationAttribute(serverConfiguration));

            httpConfiguration.ParameterBindingRules.Insert(
                0,
                descriptor =>
                {
                    if (typeof(Resource).IsAssignableFrom(descriptor.ParameterType))
                        return new ResourceParameterBinding(
                            serverConfiguration,
                            descriptor,
                            descriptor.Configuration.DependencyResolver.GetService(typeof(ISchemaTypeFactory)) as ISchemaTypeFactory);

                    return null;
                });
            httpConfiguration.ParameterBindingRules.Insert(
                1,
                descriptor =>
                {
                    if (typeof(ScimQueryOptions).IsAssignableFrom(descriptor.ParameterType))
                        return new ScimQueryOptionsParameterBinding(descriptor, serverConfiguration);

                    return null;
                });

            // refer to https://tools.ietf.org/html/rfc7644#section-3.1
            httpConfiguration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("application/scim+json"));

            httpConfiguration.Services.Replace(
                typeof(IHttpControllerTypeResolver),
                new DefaultHttpControllerTypeResolver(IsControllerType));

            httpConfiguration.Filters.Add(new ModelBindingResponseAttribute());
            httpConfiguration.Services.Replace(typeof(IExceptionHandler), new PassthroughExceptionHandler());
        }

        private static bool IsControllerType(Type t)
        {
            return
                typeof(ScimControllerBase).IsAssignableFrom(t) &&
                t != null &&
                t.IsClass &&
                t.IsVisible &&
                !t.IsAbstract &&
                typeof(IHttpController).IsAssignableFrom(t) &&
                HasValidControllerName(t);
        }

        private static bool HasValidControllerName(Type controllerType)
        {
            string controllerSuffix = DefaultHttpControllerSelector.ControllerSuffix;
            return controllerType.Name.Length > controllerSuffix.Length &&
                controllerType.Name.EndsWith(controllerSuffix, StringComparison.OrdinalIgnoreCase);
        }
    }
}