namespace Owin.Scim.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Configuration;

    using Extensions;

    using Model;
    
    public class SchemaService : ServiceBase, ISchemaService
    {
        private readonly Lazy<IReadOnlyDictionary<string, ScimSchema>> _Schemas;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SchemaService(ScimServerConfiguration configuration) : base(configuration)
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
        public IScimResponse<ScimSchema> GetSchema(string schemaId)
        {
            ScimSchema schema;
            if (!Schemas.TryGetValue(schemaId, out schema))
                return new ScimErrorResponse<ScimSchema>(
                    new ScimError(HttpStatusCode.NotFound));

            return new ScimDataResponse<ScimSchema>(schema);
        }

        /// <summary>
        /// Gets all defined <see cref="ScimSchema" />s.
        /// </summary>
        /// <returns>IScimResponse&lt;IEnumerable&lt;ScimSchema&gt;&gt;.</returns>
        public IScimResponse<IEnumerable<ScimSchema>> GetSchemas()
        {
            return new ScimDataResponse<IEnumerable<ScimSchema>>(Schemas.Values);
        }

        /// <summary>
        /// Creates the schemas dictionary.
        /// </summary>
        /// <returns>IReadOnlyDictionary&lt;System.String, ScimSchema&gt;.</returns>
        protected virtual IReadOnlyDictionary<string, ScimSchema> CreateSchemas()
        {
            var schemas = new Dictionary<string, ScimSchema>();
            foreach (var std in ScimServerConfiguration.SchemaTypeDefinitions)
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