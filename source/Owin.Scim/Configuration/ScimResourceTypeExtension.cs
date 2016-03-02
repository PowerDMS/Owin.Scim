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
            Type extensionType, 
            Type extensionValidatorType)
        {
            Schema = schema;
            Required = required;
            ExtensionType = extensionType;
            ExtensionValidatorType = extensionValidatorType;
            ExtensionDefinition = extensionDefinition;
        }

        [JsonProperty("schema")]
        public string Schema { get; private set; }

        [JsonProperty("required")]
        public bool Required { get; private set; }

        [JsonIgnore]
        public Type ExtensionType { get; private set; }

        [JsonIgnore]
        public Type ExtensionValidatorType { get; private set; }

        [JsonIgnore]
        public IScimTypeDefinition ExtensionDefinition { get; private set; }
    }
}