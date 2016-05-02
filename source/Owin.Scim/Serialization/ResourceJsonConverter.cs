namespace Owin.Scim.Serialization
{
    using System;
    using System.Collections.Generic;

    using Configuration;

    using Extensions;

    using Model;

    using NContext.Common;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ResourceJsonConverter : JsonConverter
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly JsonSerializer _Serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceJsonConverter" /> class.
        /// </summary>
        /// <param name="serverConfiguration">The server configuration.</param>
        /// <param name="serializer">The serializer to use.</param>
        public ResourceJsonConverter(ScimServerConfiguration serverConfiguration, JsonSerializer serializer)
        {
            _ServerConfiguration = serverConfiguration;
            _Serializer = serializer;

            // Remove any instance of ResourceJsonConverter from this instances serializer.
            // It shouldn't have any since this is instaniated before it gets added from the calling function,
            // but just a precaution to prevent circular references which would cause a stackoverflow if json.net
            // didn't catch it.
            for (var index = 0; index < _Serializer.Converters.Count; index++)
            {
                var converter = _Serializer.Converters[index];
                if (converter is ResourceJsonConverter)
                    _Serializer.Converters.RemoveAt(index);
            }
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof (Resource).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var resource = value as Resource;
            if (resource != null)
            {
                // ExtensionSerialization is used to serialize all resource extensions
                // It's only set prior to serialization and used during de-serialization
                // to create the extension collection.
                resource.ExtensionSerialization = resource.Extensions.ToJsonDictionary();
            }

            // JToken.FromObject must have a reference to the serializer in order to 
            // use the serializer's custom ContractResolver. e.g. ScimContractResolver.
            var token = JToken.FromObject(value, _Serializer);
            token.WriteTo(writer);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="System.Exception">
        /// In order for Owin.Scim to support resource extensions, the serialization 
        /// process is uniquely designed. Therefore, your Resource type objects must 
        /// contain at mimimum, a default empty constructor (which may be private).
        /// </exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object instance = null;
            var jsonReader = reader;
            var contract = serializer.ContractResolver.ResolveContract(objectType);
            if (contract.DefaultCreator == null)
            {
                // Let's try to dynamically determine the type to instantiate based upon the schemas collection 
                // of the resource. This adds support for polymorphism if objectType == typeof(Resource), which is abstract.
                var jObject = JObject.Load(reader);
                var schemasKey = jObject.FindKeyCaseInsensitive(ScimConstants.Schemas.Key);
                if (schemasKey != null)
                {
                    var schemasValue = jObject[schemasKey];
                    if (schemasValue != null)
                    {
                        var schemaIdentifiers = ((JArray) schemasValue).ToObject<ISet<string>>();
                        foreach (var schemaBindingRule in _ServerConfiguration.SchemaBindingRules)
                        {
                            if (schemaBindingRule.Predicate(schemaIdentifiers))
                            {
                                instance = schemaBindingRule.Target.CreateInstance();
                                jsonReader = jObject.CreateReader(); // create a new reader from the token
                                break;
                            }
                        }

                    }
                }
            }
            else
            {
                instance = contract.DefaultCreator();
            }

            if (instance == null)
                throw new Exception(
                    @"In order for Owin.Scim to support resource extensions, the serialization 
                      process is uniquely designed. Therefore, your Resource type objects must 
                      contain at mimimum, a default empty constructor (which may be private).".RemoveMultipleSpaces());

            try
            {
                serializer.Populate(jsonReader, instance);
            }
            catch (FormatException)
            {
                // TODO: (DG) implement exception handling
                throw new Exception("Invalid json. Could not deserialize to objectType...");
            }
            
            var resource = instance as Resource;
            if (resource?.ExtensionSerialization != null)
            {
                foreach (var kvp in resource.ExtensionSerialization)
                {
                    var extensionType = _ServerConfiguration.GetResourceExtensionType(objectType, kvp.Key);
                    if (extensionType == null)
                        continue; // This is either a readOnly attribute or an unknown/unsupported extension

                    resource.AddExtension((ResourceExtension)kvp.Value.ToObject(extensionType));
                }
            }

            return resource;
        }
    }
}