namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Model;
    using ErrorHandling;

    public class DefaultSchemaTypeFactory : ISchemaTypeFactory
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        public DefaultSchemaTypeFactory(ScimServerConfiguration serverConfiguration)
        {
            _ServerConfiguration = serverConfiguration;
        }

        public virtual Type GetSchemaType(ISet<string> schemaIdentifiers)
        {
            foreach (var schemaBindingRule in _ServerConfiguration.SchemaBindingRules)
            {
                if (schemaBindingRule.Predicate(schemaIdentifiers))
                    return schemaBindingRule.Target;
            }

            throw new ScimException(
                HttpStatusCode.BadRequest, 
                "Unsupported schema.",
                ScimErrorType.InvalidValue);
        }
    }
}