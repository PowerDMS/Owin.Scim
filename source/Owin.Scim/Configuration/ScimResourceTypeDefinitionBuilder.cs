namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    using FluentValidation;

    using Model;

    using Newtonsoft.Json;

    public class ScimResourceTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>, IScimResourceTypeDefinition
        where T : Resource
    {
        private readonly string _Endpoint;

        private readonly string _Name;

        private readonly string _Schema;

        private readonly Type _ValidatorType;

        private readonly IDictionary<string, ScimResourceTypeExtension> _SchemaExtensions;
        
        public ScimResourceTypeDefinitionBuilder(
            string name, 
            string schema, 
            string endpoint,
            Type validatorType)
        {
            _SchemaExtensions = new Dictionary<string, ScimResourceTypeExtension>();
            _Name = name;
            _Schema = schema;

            if (endpoint != null && !endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Insert(0, "/");
            }

            _Endpoint = endpoint;
            _ValidatorType = validatorType;

        }

        [JsonProperty("endpoint")]
        public string Endpoint
        {
            get { return _Endpoint; }
        }

        [JsonProperty("name")]
        public string Name
        {
            get { return _Name; }
        }

        [JsonProperty("schema")]
        public string Schema
        {
            get { return _Schema; }
        }

        [JsonIgnore]
        public Type ValidatorType
        {
            get { return _ValidatorType; }
        }
        
        [JsonProperty("schemaExtensions")]
        public IEnumerable<ScimResourceTypeExtension> SchemaExtensions
        {
            get { return _SchemaExtensions.Values; }
        }

        public ScimTypeDefinitionBuilder<T> AddSchemaExtension<TExtension, TValidator>(
            string schemaIdentifier,
            bool required = false,
            Action<ScimTypeDefinitionBuilder<TExtension>> extensionBuilder = null)
            where TExtension : ResourceExtension, new()
            where TValidator : IValidator<TExtension>
        {
            var extensionDefinition = new ScimTypeDefinitionBuilder<TExtension>();

            ((IScimResourceTypeDefinition)this)
                .AddExtension(
                    new ScimResourceTypeExtension(
                        schemaIdentifier,
                        required,
                        extensionDefinition,
                        typeof(TExtension),
                        typeof(TValidator)));
            
            extensionBuilder?.Invoke(extensionDefinition);

            return this;
        }

        void IScimResourceTypeDefinition.AddExtension(ScimResourceTypeExtension extension)
        {
            _SchemaExtensions.Add(extension.Schema, extension);
        }

        ScimResourceTypeExtension IScimResourceTypeDefinition.GetExtension(string schemaIdentifier)
        {
            ScimResourceTypeExtension extension;
            if (_SchemaExtensions.TryGetValue(schemaIdentifier, out extension)) return extension;

            return null;
        }
    }
}