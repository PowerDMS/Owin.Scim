namespace Owin.Scim.Configuration
{
    using System;

    using Extensions;

    using Model;

    using Newtonsoft.Json;

    public class ScimResourceTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>, IScimResourceTypeDefinition
        where T : Resource
    {
        private readonly string _Endpoint;

        private readonly string _Name;

        private readonly string _Schema;

        public ScimResourceTypeDefinitionBuilder(
            ScimServerConfiguration configuration, 
            string name, 
            string schema, 
            string endpoint) 
            : base(configuration)
        {
            _Name = name;
            _Schema = schema;

            if (!endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Insert(0, "/");
            }

            _Endpoint = endpoint;
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

        public ScimTypeDefinitionBuilder<T> AddSchemaExtension<TDerivative, TExtension>(
            string schemaIdentifier,
            bool required = false,
            Action<ScimTypeDefinitionBuilder<TExtension>> extensionBuilder = null)
            where TDerivative : Resource, T
            where TExtension : class
        {
            if (!typeof(TDerivative).ContainsSchemaExtension<TExtension>(schemaIdentifier))
            {
                throw new InvalidOperationException(
                    string.Format(
                        @"To use type '{0}' as a schema extension, it must have a single property 
                        of type '{1}' with a JsonPropertyAttribute whose PropertyName is equal to '{2}'.".RemoveMultipleSpaces(),
                        typeof(TDerivative).Name,
                        typeof(TExtension).Name,
                        schemaIdentifier));
            }

            var extensionDefinition = new ScimTypeDefinitionBuilder<TExtension>(ScimServerConfiguration);

            AddExtension(extensionDefinition);

            extensionBuilder?.Invoke(extensionDefinition);

            return this;
        }
    }
}