namespace Owin.Scim.Model
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using NContext.Common;

    using Newtonsoft.Json.Linq;

    public class ResourceExtensions : IEnumerable<KeyValuePair<string, ResourceExtension>>
    {
        private readonly ConcurrentDictionary<string, ResourceExtension> _Extensions;

        public ResourceExtensions()
        {
            _Extensions = new ConcurrentDictionary<string, ResourceExtension>();
        }

        public ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>(
                    _Extensions.Values
                        .Where(ext => ext != null)
                        .Select(ext => ext.SchemaIdentifier));
            }
        }

        public bool Contains(Type extensionType)
        {
            return _Extensions.ContainsKey(extensionType.FullName);
        }

        public T GetOrCreate<T>() where T : ResourceExtension, new()
        {
            return _Extensions.GetOrAdd(
                typeof (T).FullName,
                type => (ResourceExtension)typeof(T).CreateInstance()) as T;
        }

        internal void Add(ResourceExtension extensionInstance)
        {
            _Extensions.TryAdd(extensionInstance.GetType().FullName, extensionInstance);
        }

        internal object GetOrCreate(Type extensionType)
        {
            if (!typeof(ResourceExtension).IsAssignableFrom(extensionType))
                throw new ArgumentException("Extension must be a type assignable from ResourceExtension.", "extensionType");

            return _Extensions.GetOrAdd(
                extensionType.FullName,
                type => (ResourceExtension)extensionType.CreateInstance());
        }

        public IDictionary<string, JToken> ToJsonDictionary()
        {
            return _Extensions
                .ToDictionary(
                    kvp => kvp.Value.SchemaIdentifier,
                    kvp => kvp.Value == null
                        ? (JToken)null
                        : (JToken)JObject.FromObject(kvp.Value));
        }

        public int CalculateVersion()
        {
            var hash = 0;
            foreach (var extension in _Extensions.Values.Where(ext => ext != null))
            {
                hash = hash * 31 + extension.CalculateVersion();
            }

            return hash;
        }

        public IEnumerator<KeyValuePair<string, ResourceExtension>> GetEnumerator()
        {
            return _Extensions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}