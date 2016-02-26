namespace Owin.Scim.Configuration
{
    using System;

    using Newtonsoft.Json;

    public class ScimResourceTypeExtension
    {
        public ScimResourceTypeExtension(
            string schema,
            bool required,
            IScimTypeDefinition extensionDefinition, 
            Type resourceType, 
            Type resourceValidatorType)
        {
            Schema = schema;
            Required = required;
            ResourceType = resourceType;
            ResourceValidatorType = resourceValidatorType;
            ExtensionDefinition = extensionDefinition;
        }

        [JsonProperty("schema")]
        public string Schema { get; private set; }

        [JsonProperty("required")]
        public bool Required { get; private set; }

        [JsonIgnore]
        public Type ResourceType { get; private set; }

        [JsonIgnore]
        public Type ResourceValidatorType { get; private set; }

        [JsonIgnore]
        public IScimTypeDefinition ExtensionDefinition { get; private set; }
    }
}