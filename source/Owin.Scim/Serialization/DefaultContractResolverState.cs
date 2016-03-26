namespace Owin.Scim.Serialization
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Serialization;

    internal class DefaultContractResolverState
    {
        public PropertyNameTable NameTable = new PropertyNameTable();
        public Dictionary<ResolverContractKey, JsonContract> ContractCache;
    }
}