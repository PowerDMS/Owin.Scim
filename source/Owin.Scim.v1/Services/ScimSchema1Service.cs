namespace Owin.Scim.v1.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Configuration;

    using Extensions;

    using Model;

    using Scim.Model;
    using Scim.Services;

    public class ScimSchema1Service : SchemaService
    {
        public ScimSchema1Service(ScimServerConfiguration serverConfiguration, IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
        }

        public override Task<IScimResponse<ScimSchema>> GetSchema(string schemaId)
        {
            return Task.FromResult<IScimResponse<ScimSchema>>(
                new ScimErrorResponse<ScimSchema>(
                    new ScimError(HttpStatusCode.BadRequest)));
        }

        protected override IReadOnlyDictionary<string, ScimSchema> CreateSchemas()
        {
            var schemas = new Dictionary<string, ScimSchema>();
            foreach (var std in ServerConfiguration.GetSchemaTypeDefinitions(ScimVersion.One))
            {
                var attributeDefinitions = new List<ScimAttributeSchema>();
                foreach (var ad in std.AttributeDefinitions.Values)
                {
                    attributeDefinitions.Add(ad.ToScimAttributeSchema());
                }

                schemas.Add(
                    string.Format("{0}:{1}", std.Schema, std.Name),
                    SetResourceVersion(
                        new ScimSchema1(
                            std.Schema, 
                            std.Name, 
                            std.Description, 
                            attributeDefinitions)));

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
                            SetResourceVersion(
                                new ScimSchema1(
                                    extension.Schema,
                                    extension.ExtensionDefinition.Name,
                                    extension.ExtensionDefinition.Description,
                                    attributeDefinitions)));
                    }
                }
            }

            return schemas;
        }
    }
}