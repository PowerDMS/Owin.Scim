namespace Owin.Scim.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;

    using Configuration;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Services;

    public class ScimContractResolver : CamelCasePropertyNamesContractResolver
    {
        private static readonly HttpMethod _Patch = new HttpMethod("patch");

        private static readonly object _TypeContractCacheLock = new object();

        private readonly DefaultContractResolverState _InstanceState = new DefaultContractResolverState();

        static ScimContractResolver()
        {
        }

        public override JsonContract ResolveContract(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var state = _InstanceState;
            var key = new ResolverContractKey(GetType(), type);
            var dictionary1 = state.ContractCache;
            JsonContract contract;
            if (dictionary1 == null || !dictionary1.TryGetValue(key, out contract))
            {
                contract = CreateContract(type);
                object obj = _TypeContractCacheLock;
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(obj, ref lockTaken);
                    var dictionary2 = state.ContractCache;
                    var dictionary3 = dictionary2 != null 
                        ? new Dictionary<ResolverContractKey, JsonContract>(dictionary2) 
                        : new Dictionary<ResolverContractKey, JsonContract>();
                    dictionary3[key] = contract;
                    state.ContractCache = dictionary3;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(obj);
                }
            }

            return contract;
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return base.GetSerializableMembers(objectType)
                .Where(m => m is PropertyInfo && !m.CustomAttributes.Any(a => a.AttributeType == typeof(ScimInternalAttribute)))
                .ToList();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var serializableMembers = GetSerializableMembers(type).Cast<PropertyInfo>();
            if (serializableMembers == null)
                throw new JsonSerializationException("Null collection of seralizable members returned.");

            var typeDefinition = ScimServerConfiguration.GetScimTypeDefinition(type);
            var propertyCollection = new JsonPropertyCollection(type);
            foreach (var member in serializableMembers)
            {
                var property = CreateProperty(member, memberSerialization);
                if (property != null)
                {
                    var state = _InstanceState;
                    var propertyNameTable = state.NameTable;
                    bool lockTaken = false;
                    try
                    {
                        Monitor.Enter(propertyNameTable, ref lockTaken);
                        property.PropertyName = state.NameTable.Add(property.PropertyName);

                        IScimTypeAttributeDefinition attributeDefinition;
                        if (typeDefinition != null && typeDefinition.AttributeDefinitions.TryGetValue(member, out attributeDefinition))
                        {
                            // Internal json.net deserialization logic will hit the ShouldDeserialize delegate before checking property.Writable
                            // Still, we'll set Writable to accurately reflect this
                            property.Writable = attributeDefinition.Mutability != Mutability.ReadOnly;  // deserializable?

                            // setting property.Readable will shortcut the ShouldSerialize delegate
                            // INTERNAL JSON.NET LOGIC
                            // if (!property.Ignored && property.Readable && ShouldSerialize(writer, property, value) && IsSpecified(writer, property, value))
                            property.Readable = attributeDefinition.Mutability != Mutability.WriteOnly; // serializable?

                            // Only attach a conditional deserialization delegate if the attribute is not ReadWrite.
                            if (attributeDefinition.Mutability != Mutability.ReadWrite)
                            {
                                property.ShouldDeserialize = o =>
                                {
                                    if (attributeDefinition.Mutability == Mutability.ReadOnly)
                                        return false;

                                    return true;
                                };
                            }

                            // Only attach a conditional serialization delegate if the attribute is not Always returned.
                            if (attributeDefinition.Returned != Returned.Always)
                            {
                                property.ShouldSerialize = o =>
                                {
                                    if (attributeDefinition.Mutability == Mutability.WriteOnly || attributeDefinition.Returned == Returned.Never)
                                        return false;

                                    var httpMethod = AmbientRequestMessageService.HttpMethod;
                                    if (attributeDefinition.Returned == Returned.Default ||
                                        (attributeDefinition.Returned == Returned.Request && (httpMethod == HttpMethod.Post || httpMethod == _Patch || httpMethod == HttpMethod.Put)))
                                    {
                                        var queryOptions = AmbientRequestMessageService.QueryOptions;
                                        if (queryOptions.Attributes.Any() && !queryOptions.Attributes.Contains(property.PropertyName))
                                            return false;

                                        if (queryOptions.ExcludedAttributes.Any() && queryOptions.ExcludedAttributes.Contains(property.PropertyName))
                                            return false;
                                    }

                                    return true;
                                };
                            }
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                            Monitor.Exit(propertyNameTable);
                    }

                    propertyCollection.AddProperty(property);
                }
            }

            return propertyCollection.OrderBy(p => p.Order ?? -1).ToList();
        }
    }
}