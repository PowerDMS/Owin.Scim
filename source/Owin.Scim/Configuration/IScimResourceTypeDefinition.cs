namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IScimResourceTypeDefinition : IScimTypeDefinition
    {
        string Description { get; }

        string Endpoint { get; }

        string Name { get; }

        string Schema { get; }

        Type ValidatorType { get; }

        IEnumerable<ScimResourceTypeExtension> SchemaExtensions { get; }
        
        void AddExtension(ScimResourceTypeExtension extension);

        ScimResourceTypeExtension GetExtension(string schemaIdentifier);
    }
}