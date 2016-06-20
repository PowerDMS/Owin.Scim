namespace Owin.Scim.v2.Services
{
    using System.Collections.Generic;

    using Configuration;

    using Extensions;

    using Model;

    using Scim.Model;
    using Scim.Services;

    public class ScimSchema2Service : SchemaService
    {
        public ScimSchema2Service(ScimServerConfiguration serverConfiguration, IResourceVersionProvider versionProvider) : base(serverConfiguration, versionProvider)
        {
        }

        protected override IReadOnlyDictionary<string, ScimSchema> CreateSchemas()
        {
            var schemas = new Dictionary<string, ScimSchema>();
            foreach (var std in ServerConfiguration.GetSchemaTypeDefinitions(ScimVersion.Two))
            {
                var attributeDefinitions = new List<ScimAttributeSchema>();
                foreach (var ad in std.AttributeDefinitions.Values)
                {
                    attributeDefinitions.Add(ad.ToScimAttributeSchema());
                }

                schemas.Add(
                    std.Schema,
                    SetResourceVersion(
                        new ScimSchema2(
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
                                new ScimSchema2(
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