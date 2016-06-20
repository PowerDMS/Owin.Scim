namespace Owin.Scim.v2.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Scim.Model;

    public class ScimSchema2 : ScimSchema
    {
        public ScimSchema2(
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

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Schemas.Schema; }
        }
    }
}