namespace Owin.Scim.v1.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Scim.Model;

    using Serialization;

    public class ScimSchema1 : ScimSchema
    {
        public ScimSchema1(
            string schemaId, 
            string name, 
            string description, 
            IEnumerable<ScimAttributeSchema> attributes)
        {
            Id = schemaId;
            Name = name;
            Description = description;
            Attributes = attributes == null ? null : attributes.ToList();
        }

        [JsonProperty("schema")]
        [JsonConverter(typeof(SingleElementStringConverter))]
        public override ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>(
                    new[] { SchemaIdentifier }
                        .Concat(Extensions.Schemas)
                        .ToList());
            }
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Schemas.Schema; }
        }
    }
}