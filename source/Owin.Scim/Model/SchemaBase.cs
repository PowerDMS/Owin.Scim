namespace Owin.Scim.Model
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public abstract class SchemaBase
    {
        private readonly ISet<string> _Schemas;

        protected SchemaBase()
        {
            _Schemas = new HashSet<string>(StringComparer.Ordinal);
        }

        [JsonProperty(Order = -10)]
        public IEnumerable<string> Schemas
        {
            get { return _Schemas; }
        }

        protected void AddSchema(string schema)
        {
            if (_Schemas.Contains(schema)) return;

            _Schemas.Add(schema);
        }
    }
}