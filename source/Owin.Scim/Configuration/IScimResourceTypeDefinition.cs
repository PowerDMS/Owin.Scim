namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IScimResourceTypeDefinition : IScimSchemaTypeDefinition
    {
        string Endpoint { get; }

        Type ValidatorType { get; }

        Predicate<ISet<string>> SchemaBindingRule { get; }

        IEnumerable<ScimResourceTypeExtension> SchemaExtensions { get; }

        ScimResourceTypeExtension GetExtension(string schemaIdentifier);
    }
}