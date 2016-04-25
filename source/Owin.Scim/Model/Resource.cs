namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Configuration;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
    public abstract class Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        protected Resource()
        {
            Extensions = new ResourceExtensions();
        }

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        /// <value>The schemas.</value>
        [JsonProperty(Order = -10, PropertyName = "schemas")]
        public ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>(
                    new[] { SchemaIdentifier }
                        .Concat(Extensions.Schemas)
                        .ToList());
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier for a SCIM resource as defined by the service provider.
        /// </summary>
        /// <value>The identifier.</value>
        [Description(@"A unique identifier for a SCIM resource as defined by the service provider.")]
        [JsonProperty(Order = -5, PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the resource as defined by the provisioning client.
        /// </summary>
        /// <value>The external identifier.</value>
        [Description(@"An identifier for the resource as defined by the provisioning client.")]
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the metadata which describes the resource instance.
        /// </summary>
        /// <value>The meta.</value>
        [JsonProperty(Order = 9999, PropertyName = "meta")]
        public ResourceMetadata Meta { get; set; }

        /// <summary>
        /// Gets the schema identifier for the resource instance.
        /// </summary>
        /// <value>The schema identifier.</value>
        [JsonIgnore]
        [ScimInternal]
        public abstract string SchemaIdentifier { get; }

        /// <summary>
        /// Gets the extension collection for the resource instance.
        /// </summary>
        /// <value>The extensions.</value>
        [JsonIgnore]
        [ScimInternal]
        public ResourceExtensions Extensions { get; private set; }

        /// <summary>
        /// This is only used for internal Owin.Scim serialization purposes.
        /// </summary>
        /// <value>The extension serialization.</value>
        [JsonExtensionData]
        [ScimInternal]
        internal IDictionary<string, JToken> ExtensionSerialization { get; set; }

        /// <summary>
        /// Gets the resource extension instance for the type <typeparamref name="T"/> specified. 
        /// If no extension exists, it initializes an instance and returns the created instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T Extension<T>() where T : ResourceExtension, new()
        {
            return Extensions.GetOrCreate<T>();
        }

        /// <summary>
        /// Adds the specified <paramref name="extension"/> to the resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extension">The extension.</param>
        public void AddExtension<T>(T extension) where T : ResourceExtension
        {
            Extensions.Add(extension);
        }

        public virtual int CalculateVersion()
        {
            return new
            {
                Id,
                ExternalId,
                Extensions = Extensions.CalculateVersion()
            }.GetHashCode();
        }
    }
}