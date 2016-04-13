namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface IScimResourceTypeDefinition : IScimTypeDefinition
    {
        string Endpoint { get; }

        string Schema { get; }

        Type ValidatorType { get; }

        Predicate<ISet<string>> SchemaBindingRule { get; }

        IEnumerable<ScimResourceTypeExtension> SchemaExtensions { get; }

        ScimResourceTypeExtension GetExtension(string schemaIdentifier);
    }
}