namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// The "ResourceType" schema specifies the metadata about a resource type.
    /// </summary>
    /// <seealso cref="Owin.Scim.Model.Resource" />
    public class ScimSchema : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScimSchema" /> class.
        /// </summary>
        /// <param name="schemaId">The schema identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="attributes">The attribute definitions.</param>
        public ScimSchema(string schemaId, string name, string description, IEnumerable<ScimAttributeSchema> attributes)
        {
            Id = schemaId;
            Name = name;
            Description = description;
            Attributes = attributes?.ToList();
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Schema);
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.Schema; }
        }

        /// <summary>
        /// Gets the schema's human-readable name. When applicable, service providers MUST specify the name, e.g., "User" or "Group". OPTIONAL.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the schema's human-readable description.  When applicable, service providers MUST specify the description. OPTIONAL.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty("description")]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the schema definitions for a complex type's attributes.
        /// </summary>
        /// <value>The attribute's schema definitions.</value>
        [JsonProperty("attributes")]
        public IEnumerable<ScimAttributeSchema> Attributes { get; private set; }
    }
}