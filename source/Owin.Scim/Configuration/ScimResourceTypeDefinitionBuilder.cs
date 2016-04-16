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

        private readonly Predicate<ISet<string>> _SchemaBindingRule;

        private Type _ValidatorType;

        public ScimResourceTypeDefinitionBuilder(
            string name, 
            string schema, 
            string endpoint,
            Type validatorType,
            Predicate<ISet<string>> schemaBindingRule)
            : base(schema)
        {
            _SchemaExtensions = new Dictionary<string, ScimResourceTypeExtension>();

            SetName(name);

            if (endpoint != null && !endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Insert(0, "/");
            }

            _Endpoint = endpoint;
            _ValidatorType = validatorType;
            _SchemaBindingRule = schemaBindingRule;
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

        public Predicate<ISet<string>> SchemaBindingRule
        {
            get { return _SchemaBindingRule; }
        }

        public ScimTypeDefinitionBuilder<T> AddSchemaExtension<TExtension, TValidator>(
            string schemaIdentifier,
            bool required = false)
            where TExtension : ResourceExtension, new()
            where TValidator : IValidator<TExtension>
        {
            var typeDefinition = ScimServerConfiguration.GetScimTypeDefinition(typeof (TExtension));
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