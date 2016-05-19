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
            Attributes = attributes == null ? null : attributes.ToList();
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

        public override int CalculateVersion()
        {
            return new
            {
                Name,
                Description,
                Attributes = Attributes == null
                    ? 0
                    : Attributes.Aggregate(
                        0,
                        (hashSeed, attributeSchema) =>
                        {
                            if (attributeSchema == null) return 0;

                            return hashSeed * 31 + new
                            {
                                ReferenceTypes = attributeSchema.ReferenceTypes,    // TODO: (DG) versioning should iterate collection
                                CanonicalValues = attributeSchema.CanonicalValues,  // TODO: (DG) versioning should iterate collection
                                CaseExact = attributeSchema.CaseExact,
                                Description = attributeSchema.Description,
                                MultiValued = attributeSchema.MultiValued,
                                Mutability = attributeSchema.Mutability,
                                Name = attributeSchema.Name,
                                Required = attributeSchema.Required,
                                Returned = attributeSchema.Returned,
                                SubAttributes = attributeSchema.SubAttributes == null
                                    ? 0
                                    : attributeSchema.SubAttributes
                                        .Aggregate(
                                            0,
                                            (subAttributeHashSeed, subAttribute) =>
                                            {
                                                if (subAttribute == null) return 0;

                                                return subAttributeHashSeed * 31 + new
                                                {
                                                    ReferenceTypes = subAttribute.ReferenceTypes,   // TODO: (DG) versioning should iterate collection
                                                    CanonicalValues = subAttribute.CanonicalValues, // TODO: (DG) versioning should iterate collection
                                                    CaseExact = subAttribute.CaseExact,
                                                    Description = subAttribute.Description,
                                                    MultiValued = subAttribute.MultiValued,
                                                    Mutability = subAttribute.Mutability,
                                                    Name = subAttribute.Name,
                                                    Required = subAttribute.Required,
                                                    Returned = subAttribute.Returned
                                                }.GetHashCode();
                                            })
                            }.GetHashCode();
                        })
            }.GetHashCode();
        }
    }
}