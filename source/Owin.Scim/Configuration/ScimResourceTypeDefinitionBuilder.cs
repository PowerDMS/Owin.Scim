namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    using Model;

    public class ScimResourceTypeDefinitionBuilder<T> : ScimSchemaTypeDefinitionBuilder<T>, IScimResourceTypeDefinition
        where T : Resource
    {
        private readonly IDictionary<string, ScimResourceTypeExtension> _SchemaExtensions;

        private readonly string _Endpoint;

        private readonly SchemaBindingPredicate _SchemaBindingPredicate;

        private Type _ValidatorType;

        public ScimResourceTypeDefinitionBuilder(
            ScimServerConfiguration serverConfiguration,
            string name, 
            string schema, 
            string endpoint,
            Type validatorType,
            SchemaBindingPredicate schemaBindingPredicate)
            : base(serverConfiguration, schema)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentNullException("schema");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException("endpoint");

            if (validatorType == null)
                throw new ArgumentNullException("validatorType");

            if (schemaBindingPredicate == null)
                throw new ArgumentNullException("schemaBindingPredicate");

            if (!schema.StartsWith(ScimConstants.Defaults.URNPrefix, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Resource types define a schema identifier which starts with \"" + ScimConstants.Defaults.URNPrefix + "\" as per RFC2141.");

            _SchemaExtensions = new Dictionary<string, ScimResourceTypeExtension>();

            if (!endpoint.StartsWith("/"))
                endpoint = endpoint.Insert(0, "/");

            _Endpoint = endpoint;
            _ValidatorType = validatorType;
            _SchemaBindingPredicate = schemaBindingPredicate;

            SetName(name);
        }
        
        public string Endpoint
        {
            get { return _Endpoint; }
        }
        
        public Type ValidatorType
        {
            get { return _ValidatorType; }
        }
        
        public IEnumerable<ScimResourceTypeExtension> SchemaExtensions
        {
            get { return _SchemaExtensions.Values; }
        }

        public SchemaBindingPredicate SchemaBindingPredicate
        {
            get { return _SchemaBindingPredicate; }
        }

        public ScimTypeDefinitionBuilder<T> AddSchemaExtension<TExtension, TValidator>(
            string schemaIdentifier,
            bool required = false)
            where TExtension : ResourceExtension, new()
            where TValidator : IValidator<TExtension>
        {
            var typeDefinition = ServerConfiguration.GetScimTypeDefinition(typeof (TExtension));
            var extension = new ScimResourceTypeExtension(
                schemaIdentifier,
                required,
                typeDefinition,
                typeof (TExtension),
                typeof (TValidator));

            _SchemaExtensions.Add(extension.Schema, extension);

            return this;
        }

        public ScimTypeDefinitionBuilder<T> RemoveSchemaExtension<TExtension>()
        {
            var extension = SchemaExtensions.SingleOrDefault(e => e.ExtensionType == typeof (TExtension));
            if (extension == null) return this;

            _SchemaExtensions.Remove(extension.Schema);

            return this;
        } 

        public ScimTypeDefinitionBuilder<T> SetValidator<TValidator>()
            where TValidator : IValidator<T>
        {
            _ValidatorType = typeof (TValidator);
            return this;
        }

        ScimResourceTypeExtension IScimResourceTypeDefinition.GetExtension(string schemaIdentifier)
        {
            ScimResourceTypeExtension extension;
            if (_SchemaExtensions.TryGetValue(schemaIdentifier, out extension)) return extension;

            return null;
        }
    }
}