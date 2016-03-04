namespace Owin.Scim.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    using Configuration;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public abstract class Resource
    {
        private readonly ResourceExtensions _Extensions = new ResourceExtensions();
        
        [JsonProperty(Order = -10, PropertyName = "schemas")]
        public ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>(
                    new[] { SchemaIdentifier }
                        .Concat(_Extensions.Schemas)
                        .ToList());
            }
        }

        [Description(@"A unique identifier for a SCIM resource as defined by the service provider.")]
        [JsonProperty(Order = -5, PropertyName = "id")]
        public string Id { get; set; }
        
        [Description(@"An identifier for the resource as defined by the provisioning client.")]
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }

        [JsonProperty(Order = 9999, PropertyName = "meta")]
        public ResourceMetadata Meta { get; set; }

        [JsonExtensionData]
        private IDictionary<string, JToken> ExtensionSerialization { get; set; }

        [JsonIgnore]
        public abstract string SchemaIdentifier { get; }

        [JsonIgnore]
        internal ResourceExtensions Extensions
        {
            get { return _Extensions; }
        }

        public virtual int CalculateVersion()
        {
            return new
            {
                Id,
                ExternalId,
                Extensions = _Extensions.CalculateVersion()
            }.GetHashCode();
        }

        public T Extension<T>() where T : ResourceExtension, new()
        {
            return _Extensions.GetOrCreate<T>();
        }

        public virtual bool ShouldSerializeId()
        {
            return true;
        }

        public virtual bool ShouldSerializeExternalId()
        {
            return true;
        }

        protected void AddExtension(Type extensionType)
        {
            _Extensions.Add(extensionType);
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            ExtensionSerialization = _Extensions.ToJsonDictionary();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            if (ExtensionSerialization == null) return;

            foreach (var kvp in ExtensionSerialization)
            {
                var extensionType = ScimServerConfiguration.GetResourceExtensionType(GetType(), kvp.Key);
                if (extensionType == null)
                    continue; // This is either a resource schema or an unsupported extension

                _Extensions.Add(kvp.Key, (ResourceExtension)kvp.Value.ToObject(extensionType));
            }
        }
    }
}