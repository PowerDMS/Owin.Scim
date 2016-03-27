namespace Owin.Scim.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using NContext.Common;

    using Newtonsoft.Json.Linq;

    public class ResourceExtensions : IEnumerable<KeyValuePair<string, SometimesLazy<ResourceExtension>>>
    {
        private readonly IDictionary<string, SometimesLazy<ResourceExtension>> _Extensions;

        public ResourceExtensions()
        {
            _Extensions = new Dictionary<string, SometimesLazy<ResourceExtension>>();
        }

        public ISet<string> Schemas { get { return new HashSet<string>(_Extensions.Keys); } } 

        public void Add(Type extensionType)
        {
            if (!typeof(ResourceExtension).IsAssignableFrom(extensionType))
                throw new ArgumentException("Resource extension must inherit from ResourceExtension.", "extension");
            
            _Extensions.Add(
                ScimServerConfiguration.GetSchemaIdentifierForResourceExtensionType(extensionType), 
                new SometimesLazy<ResourceExtension>(() => (ResourceExtension)extensionType.CreateInstance()));
        }

        internal void Add(string schemaIdentifier, ResourceExtension extensionInstance)
        {
            _Extensions.Add(schemaIdentifier, new SometimesLazy<ResourceExtension>(extensionInstance));
        }

        public bool Contains(Type extensionType)
        {
            return _Extensions.ContainsKey(ScimServerConfiguration.GetSchemaIdentifierForResourceExtensionType(extensionType));
        }

        public T GetOrCreate<T>() where T : ResourceExtension, new()
        {
            if (!Contains(typeof (T)))
            {
                Add(typeof (T));
            }

            return _Extensions[ScimServerConfiguration.GetSchemaIdentifierForResourceExtensionType(typeof(T))].Value as T;
        }

        internal object GetOrCreate(Type extensionType)
        {
            if (!typeof(ResourceExtension).IsAssignableFrom(extensionType))
                throw new ArgumentException("Extension must be a type assignable from ResourceExtension.", "extensionType");

            if (!Contains(extensionType))
            {
                Add(extensionType);
            }

            return _Extensions[ScimServerConfiguration.GetSchemaIdentifierForResourceExtensionType(extensionType)].Value;
        }

        public IDictionary<string, JToken> ToJsonDictionary()
        {
            return _Extensions
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.IsValueCreated
                        ? (JToken)JObject.FromObject(kvp.Value.Value)
                        : (JToken)null);
        }

        public int CalculateVersion()
        {
            var hash = 0;
            foreach (var extension in _Extensions.Values.Where(ext => ext.IsValueCreated))
            {
                hash = hash * 31 + extension.Value.CalculateVersion();
            }

            return hash;
        }

        public IEnumerator<KeyValuePair<string, SometimesLazy<ResourceExtension>>> GetEnumerator()
        {
            return _Extensions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}