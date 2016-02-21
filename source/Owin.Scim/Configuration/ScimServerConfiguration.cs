namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Canonicalization;

    using Microsoft.FSharp.Data.UnitSystems.SI.UnitNames;

    using Model;
    using Model.Users;

    using NContext.Extensions;

    using PhoneNumbers;

    using Services;

    using PhoneNumber = Model.Users.PhoneNumber;

    public class ScimServerConfiguration
    {
        private readonly IDictionary<ScimFeatureType, ScimFeature> _Features;
         
        private readonly ISet<AuthenticationScheme> _AuthenticationSchemes;

        private readonly IList<SchemaBindingRule> _SchemaBindingRules;

        private readonly ISet<Predicate<FileInfo>> _CompositionFileInfoConstraints;

        private readonly IDictionary<Type, IScimTypeDefinition> _ResourceTypeDefinitions;

        public ScimServerConfiguration()
        {
            _AuthenticationSchemes = new HashSet<AuthenticationScheme>();
            _CompositionFileInfoConstraints = new HashSet<Predicate<FileInfo>>();
            _ResourceTypeDefinitions = new Dictionary<Type, IScimTypeDefinition>();

            _Features = CreateDefaultFeatures();
            _SchemaBindingRules = CreateDefaultBindingRules();

            CreateCoreResourceTypes();

            RequireSsl = true;
        }

        public static explicit operator ServiceProviderConfig(ScimServerConfiguration config)
        {
            return new ServiceProviderConfig(
                config.GetFeature(ScimFeatureType.Patch),
                config.GetFeature<ScimFeatureBulk>(ScimFeatureType.Bulk),
                config.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                config.GetFeature(ScimFeatureType.ChangePassword),
                config.GetFeature(ScimFeatureType.Sort),
                config.GetFeature(ScimFeatureType.ETag),
                config.AuthenticationSchemes);
        }

        public bool RequireSsl { get; set; }

        public string PublicOrigin { get; set; }

        // TODO: should we make this Uri instead of string?

        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }

        public IEnumerable<SchemaBindingRule> SchemaBindingRules
        {
            get { return _SchemaBindingRules; }
        }

        internal IEnumerable<KeyValuePair<ScimFeatureType, ScimFeature>> Features { get { return _Features; } }

        public IEnumerable<Predicate<FileInfo>> CompositionFileInfoConstraints
        {
            get { return _CompositionFileInfoConstraints; }
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

        public ScimServerConfiguration InsertSchemaBindingRule<TBindingTarget>(Predicate<ISet<string>> predicate) 
            where TBindingTarget : SchemaBase, new()
        {
            _SchemaBindingRules.Insert(0, new SchemaBindingRule(predicate, typeof (TBindingTarget)));
            return this;
        }

        public bool IsFeatureSupported(ScimFeatureType feature)
        {
            if (!_Features.ContainsKey(feature)) throw new ArgumentOutOfRangeException();

            return _Features[feature].Supported;
        }

        public ScimServerConfiguration AddResourceType<T>(
            string name, 
            string schema, 
            string endpoint,
            Action<ScimResourceTypeDefinitionBuilder<T>> builder)
            where T : Resource
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(schema)) throw new ArgumentNullException("schema");
            if (string.IsNullOrWhiteSpace(endpoint)) throw new ArgumentNullException("endpoint");
            
            var rtb = new ScimResourceTypeDefinitionBuilder<T>(this, name, schema, endpoint);
            _ResourceTypeDefinitions.Add(rtb.ResourceType, rtb);

            builder(rtb);

            return this;
        }

        public ScimServerConfiguration ModifyResourceType<T>(Action<ScimResourceTypeDefinitionBuilder<T>> builder) where T : Resource
        {
            if (!_ResourceTypeDefinitions.ContainsKey(typeof(T)))
                throw new Exception(string.Format("There is no resource type defined for type '{0}'.", typeof(T).FullName));

            builder((ScimResourceTypeDefinitionBuilder<T>) _ResourceTypeDefinitions[typeof (T)]);

            return this;
        }

        public ScimServerConfiguration RemoveResourceType<T>()
        {
            if (_ResourceTypeDefinitions.ContainsKey(typeof (T)))
            {
                _ResourceTypeDefinitions.Remove(typeof (T));
            }

            return this;
        }

        public IScimTypeDefinition GetScimTypeDefinition(Type type)
        {
            return _ResourceTypeDefinitions.ContainsKey(type)
                ? _ResourceTypeDefinitions[type]
                : null;
        }

        private IList<SchemaBindingRule> CreateDefaultBindingRules()
        {
            var rules = new List<SchemaBindingRule>
            {
                new SchemaBindingRule(
                    schemaIdentifiers =>
                    {
                        if (schemaIdentifiers.Count == 1 &&
                            schemaIdentifiers.Contains(ScimConstants.Schemas.User))
                            return true;

                        return false;
                    },
                    typeof (User)),
                new SchemaBindingRule(
                    schemaIdentifiers =>
                    {
                        if (schemaIdentifiers.Count == 2 &&
                            schemaIdentifiers.Contains(ScimConstants.Schemas.User) &&
                            schemaIdentifiers.Contains(ScimConstants.Schemas.UserEnterprise))
                            return true;

                        return false;
                    },
                    typeof (EnterpriseUser))
            };

            return rules;
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

        private void CreateCoreResourceTypes()
        {
            AddResourceType<User>(
                ScimConstants.ResourceTypes.User, 
                ScimConstants.Schemas.User,
                ScimConstants.Endpoints.Users, 
                DefineUserResourceType);
        }

        private void DefineUserResourceType(ScimResourceTypeDefinitionBuilder<User> builder)
        {
            builder
                .For(u => u.Id)
                    .SetMutability(Mutability.ReadOnly)
                    .SetReturned(Returned.Always)
                    .SetUniqueness(Uniqueness.Server)
                    .SetCaseExact(true)

                .For(u => u.UserName)
                    .SetRequired(true)
                    .SetUniqueness(Uniqueness.Server)

                .For(u => u.Locale)
                    .AddCanonicalizationRule(locale => !string.IsNullOrWhiteSpace(locale) ? locale.Replace('_', '-') : locale)

                .For(u => u.ProfileUrl)
                    .SetReferenceTypes("external")

                .For(u => u.Password)
                    .SetMutability(Mutability.WriteOnly)
                    .SetReturned(Returned.Never)

                .For(u => u.Emails)
                    .AddCanonicalizationRule(email => 
                        email.Canonicalize(
                            e => e.Value, 
                            e => e.Display, 
                            value =>
                        {
                            if (string.IsNullOrWhiteSpace(value)) return null;

                            var atIndex = value.IndexOf('@') + 1;
                            if (atIndex == 0) return null; // IndexOf returned -1, invalid email
                            
                            return value.Substring(0, atIndex) + value.Substring(atIndex).ToLower();
                        }))
                    .AddCanonicalizationRule((Email attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state))
                    .DefineSubAttributes(config => config
                        .For(e => e.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.PhoneNumbers)
                    .AddCanonicalizationRule(phone => phone.Canonicalize(p => p.Value, p => p.Display, PhoneNumberUtil.Normalize))
                    .AddCanonicalizationRule((PhoneNumber phone, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(phone, ref state))
                    .DefineSubAttributes(config => config
                        .For(p => p.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Groups)
                    .AddCanonicalizationRule((UserGroup group, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(group, ref state))
                    .SetMutability(Mutability.ReadOnly)
                    .DefineSubAttributes(config => config
                        .For(g => g.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Addresses)
                    .AddCanonicalizationRule((Address address, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(address, ref state))
                    .DefineSubAttributes(config => config
                        .For(a => a.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Roles)
                    .AddCanonicalizationRule((Role role, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(role, ref state))
                    .DefineSubAttributes(config => config
                        .For(r => r.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Entitlements)
                    .AddCanonicalizationRule((Entitlement entitlement, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(entitlement, ref state))
                    .DefineSubAttributes(config => config
                        .For(e => e.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Ims)
                    .AddCanonicalizationRule((InstantMessagingAddress im, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(im, ref state))
                    .DefineSubAttributes(config => config
                        .For(im => im.Display)
                            .SetMutability(Mutability.ReadOnly))

                .For(u => u.Photos)
                    .AddCanonicalizationRule((Photo photo, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(photo, ref state))
                    .DefineSubAttributes(config => config
                        .For(p => p.Display)
                            .SetMutability(Mutability.ReadOnly)
                        .For(p => p.Value)
                            .AddCanonicalizationRule(value => value.ToLower()))

                .For(u => u.X509Certificates)
                    .AddCanonicalizationRule((X509Certificate certificate, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(certificate, ref state))
                    .DefineSubAttributes(config => config
                        .For(c => c.Display)
                            .SetMutability(Mutability.ReadOnly))


// SUPPORT SCHEMA EXTENSIONS!
//                        .AddOrModifySchemaExtension<EnterpriseUser, EnterpriseUserExtension>(ScimConstants.Schemas.UserEnterprise, true)
//                            .ForMember(eu => eu.EmployeeNumber)
//                                .SetDescription("A string identifier, typically numeric or alphanumeric, assigned to a person, typically based on order of hire or association with an organization.")
;
        }

    }
}