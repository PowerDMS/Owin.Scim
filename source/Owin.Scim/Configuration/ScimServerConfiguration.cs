namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using Model;
    using Model.Users;
    using Model.Groups;

    using NContext.Common;
    using NContext.Extensions;

    using Validation.Users;

    public class ScimServerConfiguration
    {
        private static readonly ConcurrentDictionary<Type, IScimTypeDefinition> _TypeDefinitionCache =
            new ConcurrentDictionary<Type, IScimTypeDefinition>();
        
        private static readonly Lazy<ISet<string>> _ResourceExtensions = 
            new Lazy<ISet<string>>(
                () => 
                new HashSet<string>(_TypeDefinitionCache
                    .Where(td => td.Value is IScimResourceTypeDefinition)
                    .SelectMany(rtd => ((IScimResourceTypeDefinition)rtd.Value).SchemaExtensions)
                    .Select(e => e.Schema)));

        private readonly object _SyncLock = new object();

        private readonly IDictionary<ScimFeatureType, ScimFeature> _Features;

        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        private readonly IList<SchemaBindingRule> _SchemaBindingRules;

        private readonly ISet<Predicate<FileInfo>> _CompositionFileInfoConstraints;

        private readonly Lazy<IEnumerable<ResourceType>> _ResourceTypes = 
            new Lazy<IEnumerable<ResourceType>>(() =>
            {
                return _TypeDefinitionCache
                    .Where(td => td.Value is IScimResourceTypeDefinition)
                    .Select(td =>
                    {
                        var rtd = (IScimResourceTypeDefinition) td.Value;
                        return new ResourceType
                        {
                            Description = rtd.Description,
                            Schema = rtd.Schema,
                            Name = rtd.Name,
                            Endpoint = rtd.Endpoint
                        };
                    });
            });

        public ScimServerConfiguration()
        {
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            _CompositionFileInfoConstraints = new HashSet<Predicate<FileInfo>>();
            _SchemaBindingRules = new List<SchemaBindingRule>();

            _Features = CreateDefaultFeatures();

            DefineCoreResourceTypes();

            RequireSsl = true;
        }

        public static explicit operator ServiceProviderConfig(ScimServerConfiguration scimConfig)
        {
            // TODO: (DG) move this to a service and set meta version
            var config = new ServiceProviderConfig(
                scimConfig.GetFeature(ScimFeatureType.Patch),
                scimConfig.GetFeature<ScimFeatureBulk>(ScimFeatureType.Bulk),
                scimConfig.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                scimConfig.GetFeature(ScimFeatureType.ChangePassword),
                scimConfig.GetFeature(ScimFeatureType.Sort),
                scimConfig.GetFeature(ScimFeatureType.ETag),
                scimConfig.AuthenticationSchemes);

            return config;
        }

        public bool RequireSsl { get; set; }

        public string PublicOrigin { get; set; } // TODO: (CY) should we make this Uri instead of string?

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }

        public IEnumerable<Predicate<FileInfo>> CompositionFileInfoConstraints
        {
            get { return _CompositionFileInfoConstraints; }
        }

        public IEnumerable<ResourceType> ResourceTypes
        {
            get { return _ResourceTypes.Value; }
        }

        internal IEnumerable<KeyValuePair<ScimFeatureType, ScimFeature>> Features
        {
            get { return _Features; }
        }

        internal IList<SchemaBindingRule> SchemaBindingRules
        {
            get { return _SchemaBindingRules; }
        }

        internal IEnumerable<IScimResourceTypeDefinition> ResourceTypeDefinitions
        {
            get { return _TypeDefinitionCache.Values.OfType<IScimResourceTypeDefinition>(); }
        }

        public static IScimTypeDefinition GetScimTypeDefinition(Type type)
        {
            return _TypeDefinitionCache.GetOrAdd(
                type,
                t => TypeDescriptor.GetAttributes(t)
                    .OfType<ScimTypeDefinitionAttribute>()
                    .MaybeSingle()
                    .Bind(a => ((IScimTypeDefinition) Activator.CreateInstance(a.DefinitionType)).ToMaybe())
                    .FromMaybe(null));
        }

        public static bool ResourceExtensionExists(string extensionSchemaIdentifier)
        {
            return _ResourceExtensions.Value.Contains(extensionSchemaIdentifier);
        }

        public static Type GetResourceExtensionType(Type resourceType, string extensionSchemaIdentifier)
        {
            IScimTypeDefinition rtd;
            if (!_TypeDefinitionCache.TryGetValue(resourceType, out rtd)) return null;

            return (rtd as IScimResourceTypeDefinition)?.GetExtension(extensionSchemaIdentifier)?.ExtensionType;
        }

        public static string GetSchemaIdentifierForResourceType(Type resourceType)
        {
            IScimTypeDefinition td;
            if (_TypeDefinitionCache.TryGetValue(resourceType, out td))
            {
                return ((IScimResourceTypeDefinition) td).Schema;
            }

            throw new InvalidOperationException(
                string.Format("Type '{0}' is not a valid resource.", resourceType.Name));
        }

        public static string GetSchemaIdentifierForResourceExtensionType(Type extensionType)
        {
            foreach (var rtd in _TypeDefinitionCache.Values.OfType<IScimResourceTypeDefinition>())
            {
                var ext = rtd.SchemaExtensions.SingleOrDefault(e => e.ExtensionType == extensionType);
                if (ext != null)
                    return ext.Schema;
            }

            throw new InvalidOperationException(
                string.Format("Type '{0}' is not a valid resource extension.", extensionType.Name));
        }

        public ScimServerConfiguration AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            // Enforce only one primary
            if (authenticationScheme.Primary)
            {
                foreach (var scheme in _AuthenticationSchemes)
                {
                    scheme.Primary = false;
                }
            }

            _AuthenticationSchemes.Add(authenticationScheme);
            return this;
        }

        public ScimServerConfiguration AddCompositionConditions(params Predicate<FileInfo>[] fileInfoConstraints)
        {
            _CompositionFileInfoConstraints.AddRange(fileInfoConstraints);
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

        public ScimServerConfiguration AddResourceType<T>(Predicate<ISet<string>> schemaBindingRule) where T : Resource
        {
            if (_TypeDefinitionCache.ContainsKey(typeof(T)))
                throw new InvalidOperationException(
                    string.Format("Scim server already contains a resource type for type '{0}'.", typeof(T).Name));

            var typeDefinition = TypeDescriptor.GetAttributes(typeof (T))
                .OfType<ScimTypeDefinitionAttribute>()
                .MaybeSingle()
                .Bind(a =>
                {
                    if (!typeof(ScimResourceTypeDefinitionBuilder<T>).IsAssignableFrom(a.DefinitionType))
                        throw new InvalidOperationException(
                            string.Format(
                                "Type definition '{0}' must inherit from ScimResourceTypeDefinitionBuilder<{1}>.", 
                                a.DefinitionType.Name,
                                typeof(T).Name));

                    return ((ScimResourceTypeDefinitionBuilder<T>) Activator.CreateInstance(a.DefinitionType)).ToMaybe();
                })
                .FromMaybe(null);

            if (typeDefinition == null)
                throw new InvalidOperationException(string.Format("Resource type '{0}' must have a ScimTypeDefinitionAttribute attribute defined.", typeof(T).Name));

            if (!_TypeDefinitionCache.TryAdd(typeDefinition.DefinitionType, typeDefinition))
                throw new ApplicationException("Could not add resource type definition to cache.");

            _SchemaBindingRules.Insert(0, new SchemaBindingRule(schemaBindingRule, typeof (T)));

            return this;
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
            IScimTypeDefinition td;

            return _TypeDefinitionCache.TryGetValue(resourceType, out td) 
                ? (td as IScimResourceTypeDefinition)?.ValidatorType 
                : null;
        }
        
        private IDictionary<ScimFeatureType, ScimFeature> CreateDefaultFeatures()
        {
            var features = new Dictionary<ScimFeatureType, ScimFeature>();
            features.Add(ScimFeatureType.Patch, new ScimFeature(true));
            features.Add(ScimFeatureType.Bulk, ScimFeatureBulk.Create(ScimConstants.Defaults.BulkMaxOperations, ScimConstants.Defaults.BulkMaxPayload));
            features.Add(ScimFeatureType.Filter, ScimFeatureFilter.Create(ScimConstants.Defaults.FilterMaxResults));
            features.Add(ScimFeatureType.ChangePassword, new ScimFeature(true));
            features.Add(ScimFeatureType.Sort, new ScimFeature(true));
            features.Add(ScimFeatureType.ETag, new ScimFeatureETag(true, true));

            return features;
        }

        private void DefineCoreResourceTypes()
        {
            // this is mainly for unit tests to not try and keep creating resource types as they are static
            if (!_TypeDefinitionCache.Any())
            {
                lock (_SyncLock)
                {
                    if (!_TypeDefinitionCache.Any())
                    {
                        AddResourceType<User>(schemaIdentifiers => schemaIdentifiers.Contains(ScimConstants.Schemas.User));
                        AddResourceType<Group>(schemaIdentifiers => schemaIdentifiers.Contains(ScimConstants.Schemas.Group));
                    }
                }
            }
        }
    }
}