namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Http;

    using Dependencies;

    using Model;

    using NContext.Common;

    /// <summary>
    /// This class is used for central configuration around the SCIM service provider. It 
    /// is registered as a singleton and may be injected throughout your objects via DryIoc or MEF.
    /// </summary>
    public class ScimServerConfiguration
    {
        private readonly ConcurrentDictionary<Type, IScimTypeDefinition> _TypeDefinitionCache;

        private readonly IDictionary<Type, Type> _TypeDefinitionRegistry;

        private readonly IDictionary<ScimFeatureType, ScimFeature> _Features;

        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        private Lazy<IList<SchemaBindingRule>> _SchemaBindingRules;

        private Lazy<Dictionary<string, Type>> _ResourceExtensionCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScimServerConfiguration"/> class.
        /// </summary>
        public ScimServerConfiguration()
        {
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            _TypeDefinitionCache = new ConcurrentDictionary<Type, IScimTypeDefinition>();
            _TypeDefinitionRegistry = new Dictionary<Type, Type>();
            _Features = CreateDefaultFeatures();
            InitializeResourceExtensionCache();
            InitializeSchemaBindingRules();

            EnableEndpointAuthorization = true;
            RequireSsl = true;
        }

        /// <summary>
        /// Gets or sets the dependency resolver. If Owin.Scim cannot resolve a dependency and the 
        /// service type requested exists within an assembly that is part of your composition constraints,
        /// then Owin.Scim will use this resolver to retrieve an instance for injection.
        /// </summary>
        /// <value>The dependency resolver.</value>
        public IDependencyResolver DependencyResolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable endpoint authorization]. Default is set to true.
        /// </summary>
        /// <value><c>true</c> if [enable endpoint authorization]; otherwise, <c>false</c>.</value>
        public bool EnableEndpointAuthorization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require SSL]. Owin.Scim will reject all non-HTTPS requests. 
        /// Default is set to true.
        /// </summary>
        /// <value><c>true</c> if [require SSL]; otherwise, <c>false</c>.</value>
        public bool RequireSsl { get; set; }

        /// <summary>
        /// Gets the HTTP configuration used by Owin.Scim.
        /// </summary>
        /// <value>The HTTP configuration.</value>
        public HttpConfiguration HttpConfiguration { get; internal set; }

        /// <summary>
        /// Gets the authentication schemes supported by the SCIM provider.
        /// </summary>
        /// <value>The authentication schemes.</value>
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }

        public IEnumerable<IScimSchemaTypeDefinition> SchemaTypeDefinitions
        {
            get
            {
                return _TypeDefinitionCache.Values.OfType<IScimSchemaTypeDefinition>();
            }
        }

        public IEnumerable<IScimResourceTypeDefinition> ResourceTypeDefinitions
        {
            get { return _TypeDefinitionCache.Values.OfType<IScimResourceTypeDefinition>(); }
        }

        internal IEnumerable<KeyValuePair<ScimFeatureType, ScimFeature>> Features
        {
            get { return _Features; }
        }

        internal IList<SchemaBindingRule> SchemaBindingRules
        {
            get { return _SchemaBindingRules.Value; }
        }
        
        internal IReadOnlyDictionary<string, Type> ResourceExtensionSchemas
        {
            get { return _ResourceExtensionCache.Value; }
        }

        internal IDictionary<Type, Type> TypeDefinitionRegistry
        {
            get { return _TypeDefinitionRegistry; }
        }

        /// <summary>
        /// Gets the scim type definition for the specified <paramref name="type"/>. 
        /// If none is found, then it creates one.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IScimTypeDefinition.</returns>
        public IScimTypeDefinition GetScimTypeDefinition(Type type)
        {
            return _TypeDefinitionCache.GetOrAdd(
                type,
                t =>
                {
                    Type typeDefinitionType;
                    if (TypeDefinitionRegistry.TryGetValue(type, out typeDefinitionType))
                        return (IScimTypeDefinition)typeDefinitionType.CreateInstance(this);

                    var typeDefinitionBuilder = typeof (ScimTypeDefinitionBuilder<>).MakeGenericType(type);
                    return (IScimTypeDefinition)typeDefinitionBuilder.CreateInstance(this);
                });
        }

        /// <summary>
        /// Adds the authentication scheme which can be viewed from the /serviceproviderconfig endpoint.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            // Enforce only one primary
            if (authenticationScheme.Primary)
                foreach (var scheme in _AuthenticationSchemes)
                    scheme.Primary = false;

            _AuthenticationSchemes.Add(authenticationScheme);
            return this;
        }

        /// <summary>
        /// Configures patch support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigurePatch(bool supported = true)
        {
            _Features[ScimFeatureType.Patch] = new ScimFeature(supported);
            return this;
        }

        /// <summary>
        /// Configures bulk operation support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <param name="maxOperations">The maximum operations.</param>
        /// <param name="maxPayloadSizeInBytes">The maximum payload size in bytes.</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigureBulk(bool supported = true, int maxOperations = ScimConstants.Defaults.BulkMaxOperations, int maxPayloadSizeInBytes = ScimConstants.Defaults.BulkMaxPayload)
        {
            _Features[ScimFeatureType.Bulk] = supported
                ? ScimFeatureBulk.Create(maxOperations, maxPayloadSizeInBytes)
                : ScimFeatureBulk.CreateUnsupported();

            return this;
        }

        /// <summary>
        /// Configures filtering support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <param name="maxResults">The maximum results.</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigureFilter(bool supported = true, int maxResults = ScimConstants.Defaults.FilterMaxResults)
        {
            _Features[ScimFeatureType.Filter] = supported
                ? ScimFeatureFilter.Create(maxResults)
                : ScimFeatureFilter.CreateUnsupported();

            return this;
        }

        /// <summary>
        /// Configures password change support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigureChangePassword(bool supported = true)
        {
            _Features[ScimFeatureType.ChangePassword] = new ScimFeature(supported);
            return this;
        }

        /// <summary>
        /// Configures sorting support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigureSort(bool supported = true)
        {
            _Features[ScimFeatureType.Sort] = new ScimFeature(supported);
            return this;
        }

        /// <summary>
        /// Configures etag support for the SCIM service provider.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        /// <param name="isWeak">if set to <c>true</c> [is weak].</param>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration ConfigureETag(bool supported = true, bool isWeak = true)
        {
            _Features[ScimFeatureType.ETag] = new ScimFeatureETag(supported, isWeak);
            return this;
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>ScimFeature.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public ScimFeature GetFeature(ScimFeatureType feature)
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return _Features[feature];
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <typeparam name="TScimFeature">The type of the t scim feature.</typeparam>
        /// <param name="feature">The feature.</param>
        /// <returns>TScimFeature.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public TScimFeature GetFeature<TScimFeature>(ScimFeatureType feature)
            where TScimFeature : ScimFeature
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return (TScimFeature) _Features[feature];
        }

        /// <summary>
        /// Determines whether the SCIM feature is supported.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns><c>true</c> if [the specified feature] is supported; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public bool IsFeatureSupported(ScimFeatureType feature)
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return _Features[feature].Supported;
        }

        /// <summary>
        /// Modifies the <see cref="ScimResourceTypeDefinitionBuilder{T}"/> with the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>ScimServerConfiguration.</returns>
        /// <exception cref="System.Exception"></exception>
        public ScimServerConfiguration ModifyResourceType<T>(Action<ScimResourceTypeDefinitionBuilder<T>> builder)
            where T : Resource, new()
        {
            IScimTypeDefinition td;
            if (!_TypeDefinitionCache.TryGetValue(typeof(T), out td))
                throw new Exception(string.Format("There is no resource type defined for type '{0}'.", typeof(T).FullName));

            builder((ScimResourceTypeDefinitionBuilder<T>)td);
            if (_ResourceExtensionCache.IsValueCreated)
            {
                // re-initialize since we may have modified extensions for the resource
                InitializeResourceExtensionCache();
            }

            return this;
        }

        /// <summary>
        /// Removes the type of the resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>ScimServerConfiguration.</returns>
        public ScimServerConfiguration RemoveResourceType<T>() where T : Resource, new()
        {
            IScimTypeDefinition td;
            if (_TypeDefinitionCache.TryRemove(typeof(T), out td) && _ResourceExtensionCache.IsValueCreated)
            {
                // re-initialize since we've modified our resource type cache
                InitializeResourceExtensionCache();
                InitializeSchemaBindingRules();
            }

            return this;
        }

        /// <summary>
        /// Gets the type of the scim resource validator.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>Type.</returns>
        public Type GetScimResourceValidatorType(Type resourceType)
        {
            IScimTypeDefinition std;

            if (!_TypeDefinitionCache.TryGetValue(resourceType, out std))
                return null;

            var rtd = std as IScimResourceTypeDefinition;
            if (rtd == null)
                return null;

            return rtd.ValidatorType;
        }

        internal void AddTypeDefiniton(IScimTypeDefinition scimTypeDefinition)
        {
            if (_TypeDefinitionCache.ContainsKey(scimTypeDefinition.DefinitionType)) return;

            if (!_TypeDefinitionCache.TryAdd(scimTypeDefinition.DefinitionType, scimTypeDefinition))
                throw new Exception(
                    string.Format(
                        "ScimServerConfiguration was unable to add a type definition for type '{0}'.",
                        scimTypeDefinition.DefinitionType.Name));
        }

        internal bool ContainsTypeDefinition(Type scimTypeDefinition)
        {
            return _TypeDefinitionCache.Values.Any(definition => definition.GetType() == scimTypeDefinition);
        }
        
        /// <summary>
        /// Gets the concrete type of the extension specified <paramref name="extensionSchemaIdentifier"/>. 
        /// If the extension cannot be found for the given <paramref name="resourceType"/> then the 
        /// value of null is returned.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="extensionSchemaIdentifier">The extension schema identifier.</param>
        /// <returns>Type.</returns>
        internal Type GetResourceExtensionType(Type resourceType, string extensionSchemaIdentifier)
        {
            IScimTypeDefinition std;
            if (!_TypeDefinitionCache.TryGetValue(resourceType, out std)) return null;

            var rtd = std as IScimResourceTypeDefinition;
            if (rtd == null)
                return null;

            var extension = rtd.GetExtension(extensionSchemaIdentifier);
            if (extension == null)
                return null;

            return extension.ExtensionType;
        }

        /// <summary>
        /// Gets the schema identifier for the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        internal string GetSchemaIdentifierForResourceType(Type resourceType)
        {
            IScimTypeDefinition td;
            if (_TypeDefinitionCache.TryGetValue(resourceType, out td))
                return ((IScimResourceTypeDefinition)td).Schema;

            throw new InvalidOperationException(
                string.Format("Type '{0}' is not a valid resource.", resourceType.Name));
        }

        /// <summary>
        /// Returns whether extensionSchemaIdentifier exists as a registered schema extension.
        /// </summary>
        /// <param name="extensionSchemaIdentifier">The extension schema identifier.</param>
        /// <returns><c>true</c> if [extensionSchemaIdentifier is registered], <c>false</c> otherwise.</returns>
        internal bool ResourceExtensionExists(string extensionSchemaIdentifier)
        {
            return ResourceExtensionSchemas.ContainsKey(extensionSchemaIdentifier);
        }

        private void InitializeResourceExtensionCache()
        {
            _ResourceExtensionCache = new Lazy<Dictionary<string, Type>>(
                () => _TypeDefinitionCache
                        .Where(td => td.Value is IScimResourceTypeDefinition)
                        .SelectMany(rtd => ((IScimResourceTypeDefinition)rtd.Value).SchemaExtensions)
                        .ToDictionary(e => e.Schema, e => e.ExtensionType));
        }
        
        private void InitializeSchemaBindingRules()
        {
            _SchemaBindingRules = new Lazy<IList<SchemaBindingRule>>(
                () =>
                    ResourceTypeDefinitions.Select(
                        rtd =>
                            new SchemaBindingRule(rtd.SchemaBindingPredicate, rtd.DefinitionType)).ToList());
        }

        private IDictionary<ScimFeatureType, ScimFeature> CreateDefaultFeatures()
        {
            var features = new Dictionary<ScimFeatureType, ScimFeature>
            {
                { ScimFeatureType.Patch, new ScimFeature(true) },
                { ScimFeatureType.Bulk, ScimFeatureBulk.Create(ScimConstants.Defaults.BulkMaxOperations, ScimConstants.Defaults.BulkMaxPayload) },
                { ScimFeatureType.Filter, ScimFeatureFilter.Create(ScimConstants.Defaults.FilterMaxResults) },
                { ScimFeatureType.ChangePassword, new ScimFeature(true) },
                { ScimFeatureType.Sort, new ScimFeature(true) },
                { ScimFeatureType.ETag, new ScimFeatureETag(true, true) }
            };

            return features;
        }
    }
}