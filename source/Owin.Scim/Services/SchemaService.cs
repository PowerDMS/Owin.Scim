namespace Owin.Scim.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Configuration;

    using Extensions;

    using Model;
    
    public class SchemaService : ServiceBase, ISchemaService
    {
        private readonly Lazy<IReadOnlyDictionary<string, ScimSchema>> _Schemas;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaService" /> class.
        /// </summary>
        /// <param name="serverConfiguration">The configuration.</param>
        /// <param name="versionProvider">The version provider.</param>
        public SchemaService(ScimServerConfiguration serverConfiguration, IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
            _Schemas = new Lazy<IReadOnlyDictionary<string, ScimSchema>>(CreateSchemas);
        }

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        /// <value>The schemas.</value>
        protected virtual IReadOnlyDictionary<string, ScimSchema> Schemas
        {
            get { return _Schemas.Value; }
        }

        /// <summary>
        /// Gets the <see cref="ScimSchema" /> associated with the specified <paramref name="schemaId" />.
        /// </summary>
        /// <param name="schemaId">The schema identifier.</param>
        /// <returns>IScimResponse&lt;ScimSchema&gt;.</returns>
        public Task<IScimResponse<ScimSchema>> GetSchema(string schemaId)
        {
            ScimSchema schema;
            if (!Schemas.TryGetValue(schemaId, out schema))
                return Task.FromResult<IScimResponse<ScimSchema>>(
                    new ScimErrorResponse<ScimSchema>(
                        new ScimError(HttpStatusCode.NotFound)));

            return Task.FromResult<IScimResponse<ScimSchema>>(
                new ScimDataResponse<ScimSchema>(schema));
        }

        /// <summary>
        /// Gets all defined <see cref="ScimSchema" />s.
        /// </summary>
        /// <returns>IScimResponse&lt;IEnumerable&lt;ScimSchema&gt;&gt;.</returns>
        public Task<IScimResponse<IEnumerable<ScimSchema>>> GetSchemas()
        {
            return Task.FromResult<IScimResponse<IEnumerable<ScimSchema>>>(
                new ScimDataResponse<IEnumerable<ScimSchema>>(Schemas.Values));
        }

        /// <summary>
        /// Creates the schemas dictionary.
        /// </summary>
        /// <returns>IReadOnlyDictionary&lt;System.String, ScimSchema&gt;.</returns>
        protected virtual IReadOnlyDictionary<string, ScimSchema> CreateSchemas()
        {
            var schemas = new Dictionary<string, ScimSchema>();
            foreach (var std in ServerConfiguration.SchemaTypeDefinitions)
            {
                var attributeDefinitions = new List<ScimAttributeSchema>();
                foreach (var ad in std.AttributeDefinitions.Values)
                {
                    attributeDefinitions.Add(ad.ToScimAttributeSchema());
                }

                schemas.Add(std.Schema, new ScimSchema(std.Schema, std.Name, std.Description, attributeDefinitions));

                var rtd = std as IScimResourceTypeDefinition;
                if (rtd != null)
                {
                    foreach (var extension in rtd.SchemaExtensions)
                    {
                        attributeDefinitions = new List<ScimAttributeSchema>();
                        foreach (var ad in extension.ExtensionDefinition.AttributeDefinitions.Values)
                        {
                            attributeDefinitions.Add(ad.ToScimAttributeSchema());
                        }

                        schemas.Add(
                            extension.Schema,
                            new ScimSchema(
                                extension.Schema,
                                extension.ExtensionDefinition.Name,
                                extension.ExtensionDefinition.Description,
                                attributeDefinitions));
                    }
                }
            }

            return schemas;
        }
    }
}