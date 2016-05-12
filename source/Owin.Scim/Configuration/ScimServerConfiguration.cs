namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Http;

    using Model;

    using NContext.Common;

    public class ScimServerConfiguration
    {
        private readonly ConcurrentDictionary<Type, IScimTypeDefinition> _TypeDefinitionCache =
            new ConcurrentDictionary<Type, IScimTypeDefinition>();

        private readonly IDictionary<Type, Type> _TypeDefinitionRegistry;

        private readonly Lazy<ISet<string>> _ResourceExtensionSchemas;

        private readonly IDictionary<ScimFeatureType, ScimFeature> _Features;

        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        private readonly Lazy<IList<SchemaBindingRule>> _SchemaBindingRules;

        public ScimServerConfiguration()
        {
            _TypeDefinitionRegistry = new Dictionary<Type, Type>();
            _ResourceExtensionSchemas = new Lazy<ISet<string>>(
                () =>
                    new HashSet<string>(_TypeDefinitionCache
                        .Where(td => td.Value is IScimResourceTypeDefinition)
                        .SelectMany(rtd => ((IScimResourceTypeDefinition) rtd.Value).SchemaExtensions)
                        .Select(e => e.Schema)));
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            _SchemaBindingRules = new Lazy<IList<SchemaBindingRule>>(() => ResourceTypeDefinitions.Select(rtd => new SchemaBindingRule(rtd.SchemaBindingRule, rtd.DefinitionType)).ToList());
            _Features = CreateDefaultFeatures();

            RequireSsl = true;
        }

        public static explicit operator ServiceProviderConfiguration(ScimServerConfiguration scimConfig)
        {
            // TODO: (DG) move this to a service and set meta version
            var config = new ServiceProviderConfiguration(
                scimConfig.GetFeature(ScimFeatureType.Patch),
                scimConfig.GetFeature<ScimFeatureBulk>(ScimFeatureType.Bulk),
                scimConfig.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                scimConfig.GetFeature(ScimFeatureType.ChangePassword),
                scimConfig.GetFeature(ScimFeatureType.Sort),
                scimConfig.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag),
                scimConfig.AuthenticationSchemes);

            return config;
        }

        public bool RequireSsl { get; set; }

        public HttpConfiguration HttpConfiguration { get; internal set; }

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }

        internal IEnumerable<KeyValuePair<ScimFeatureType, ScimFeature>> Features
        {
            get { return _Features; }
        }

        internal IList<SchemaBindingRule> SchemaBindingRules
        {
            get { return _SchemaBindingRules.Value; }
        }

        internal IEnumerable<IScimResourceTypeDefinition> ResourceTypeDefinitions
        {
            get { return _TypeDefinitionCache.Values.OfType<IScimResourceTypeDefinition>(); }
        }

        internal IEnumerable<IScimSchemaTypeDefinition> SchemaTypeDefinitions
        {
            get
            {
                return _TypeDefinitionCache.Values.OfType<IScimSchemaTypeDefinition>();
            }
        }

        internal ISet<string> ResourceExtensionSchemas
        {
            get { return _ResourceExtensionSchemas.Value; }
        }

        internal IDictionary<Type, Type> TypeDefinitionRegistry
        {
            get { return _TypeDefinitionRegistry; }
        }

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

        public bool ResourceExtensionExists(string extensionSchemaIdentifier)
        {
            return ResourceExtensionSchemas.Contains(extensionSchemaIdentifier);
        }

        public Type GetResourceExtensionType(Type resourceType, string extensionSchemaIdentifier)
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

        public string GetSchemaIdentifierForResourceType(Type resourceType)
        {
            IScimTypeDefinition td;
            if (_TypeDefinitionCache.TryGetValue(resourceType, out td))
                return ((IScimResourceTypeDefinition) td).Schema;

            throw new InvalidOperationException(
                string.Format("Type '{0}' is not a valid resource.", resourceType.Name));
        }

        public ScimServerConfiguration AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            // Enforce only one primary
            if (authenticationScheme.Primary)
                foreach (var scheme in _AuthenticationSchemes)
                    scheme.Primary = false;

            _AuthenticationSchemes.Add(authenticationScheme);
            return this;
        }

        public ScimServerConfiguration ConfigurePatch(bool supported = true)
        {
            _Features[ScimFeatureType.Patch] = new ScimFeature(supported);
            return this;
        }

        public ScimServerConfiguration ConfigureBulk(bool supported = true, int maxOperations = ScimConstants.Defaults.BulkMaxOperations, int maxPayloadSizeInBytes = ScimConstants.Defaults.BulkMaxPayload)
        {
            _Features[ScimFeatureType.Bulk] = supported
                ? ScimFeatureBulk.Create(maxOperations, maxPayloadSizeInBytes)
                : ScimFeatureBulk.CreateUnsupported();

            return this;
        }

        public ScimServerConfiguration ConfigureFilter(bool supported = true, int maxResults = ScimConstants.Defaults.FilterMaxResults)
        {
            _Features[ScimFeatureType.Filter] = supported
                ? ScimFeatureFilter.Create(maxResults)
                : ScimFeatureFilter.CreateUnsupported();

            return this;
        }

        public ScimServerConfiguration ConfigureChangePassword(bool supported = true)
        {
            _Features[ScimFeatureType.ChangePassword] = new ScimFeature(supported);
            return this;
        }

        public ScimServerConfiguration ConfigureSort(bool supported = true)
        {
            _Features[ScimFeatureType.Sort] = new ScimFeature(supported);
            return this;
        }

        public ScimServerConfiguration ConfigureETag(bool supported = true, bool isWeak = true)
        {
            _Features[ScimFeatureType.ETag] = new ScimFeatureETag(supported, isWeak);
            return this;
        }

        public ScimFeature GetFeature(ScimFeatureType feature)
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return _Features[feature];
        }

        public TScimFeature GetFeature<TScimFeature>(ScimFeatureType feature)
            where TScimFeature : ScimFeature
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return (TScimFeature) _Features[feature];
        }

        public bool IsFeatureSupported(ScimFeatureType feature)
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return _Features[feature].Supported;
        }
        
        public ScimServerConfiguration ModifyResourceType<T>(Action<ScimResourceTypeDefinitionBuilder<T>> builder) where T : Resource
        {
            IScimTypeDefinition td;
            if (!_TypeDefinitionCache.TryGetValue(typeof(T), out td))
                throw new Exception(string.Format("There is no resource type defined for type '{0}'.", typeof(T).FullName));

            builder((ScimResourceTypeDefinitionBuilder<T>)td);

            return this;
        }

        public ScimServerConfiguration RemoveResourceType<T>() where T : Resource
        {
            IScimTypeDefinition td;
            _TypeDefinitionCache.TryRemove(typeof (T), out td);

            return this;
        }

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