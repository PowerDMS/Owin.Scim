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

        protected IReadOnlyDictionary<string, ScimSchema> Schemas
        {
            get { return _Schemas.Value; }
        }

        public IScimResponse<ScimSchema> GetSchema(string schemaId)
        {
            ScimSchema schema;
            if (!Schemas.TryGetValue(schemaId, out schema))
                return new ScimErrorResponse<ScimSchema>(
                    new ScimError(HttpStatusCode.NotFound));

            return new ScimDataResponse<ScimSchema>(schema);
        }

        public IScimResponse<IEnumerable<ScimSchema>> GetSchemas()
        {
            return new ScimDataResponse<IEnumerable<ScimSchema>>(Schemas.Values);
        }

        private IReadOnlyDictionary<string, ScimSchema> CreateSchemas()
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