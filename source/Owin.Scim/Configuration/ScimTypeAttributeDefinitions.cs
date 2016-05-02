namespace Owin.Scim.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ScimTypeAttributeDefinitions : IDictionary<PropertyInfo, IScimTypeAttributeDefinition>
    {
        private readonly IDictionary<string, KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>> _Definitions;

        public ScimTypeAttributeDefinitions()
        {
            _Definitions = new Dictionary<string, KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>>();
        }

        public ScimTypeAttributeDefinitions(IDictionary<PropertyInfo, IScimTypeAttributeDefinition> definitions)
        {
            _Definitions = definitions
                .ToDictionary(kvp => CreateKey(kvp.Key), kvp => kvp);
        }

        public IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> Definitions
        {
            get
            {
                return _Definitions
                    .ToDictionary(
                        kvp => kvp.Value.Key,
                        kvp => kvp.Value.Value);
            }
        }

        public IEnumerator<KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>> GetEnumerator()
        {
            return Definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition> item)
        {
            _Definitions.Add(CreateKey(item.Key), new KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>(item.Key, item.Value));
        }

        public void Clear()
        {
            _Definitions.Clear();
        }

        public bool Contains(KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition> item)
        {
            return _Definitions.Values.Contains(item);
        }

        public void CopyTo(KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>[] array, int arrayIndex)
        {
            _Definitions.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition> item)
        {
            return _Definitions.Remove(CreateKey(item.Key));
        }

        public int Count
        {
            get { return _Definitions.Count; }
        }

        public bool IsReadOnly
        {
            get { return _Definitions.IsReadOnly; }
        }

        public bool ContainsKey(PropertyInfo key)
        {
            return _Definitions.ContainsKey(CreateKey(key));
        }

        public void Add(PropertyInfo key, IScimTypeAttributeDefinition value)
        {
            _Definitions.Add(CreateKey(key), new KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>(key, value));
        }

        public bool Remove(PropertyInfo key)
        {
            return _Definitions.Remove(CreateKey(key));
        }

        public bool TryGetValue(PropertyInfo key, out IScimTypeAttributeDefinition value)
        {
            KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition> kvp;
            if (_Definitions.TryGetValue(CreateKey(key), out kvp))
            {
                value = kvp.Value;
                return true;
            }

            value = null;
            return false;
        }

        public IScimTypeAttributeDefinition this[PropertyInfo key]
        {
            get { return _Definitions[CreateKey(key)].Value; }
            set { _Definitions[CreateKey(key)] = new KeyValuePair<PropertyInfo, IScimTypeAttributeDefinition>(key, value); }
        }

        public ICollection<PropertyInfo> Keys
        {
            get { return _Definitions.Values.Select(kvp => kvp.Key).ToList(); }
        }

        public ICollection<IScimTypeAttributeDefinition> Values
        {
            get { return _Definitions.Values.Select(kvp => kvp.Value).ToList(); }
        }

        private string CreateKey(PropertyInfo property)
        {
            return property.DeclaringType.FullName + "." + property.Name;
        }
    }
}