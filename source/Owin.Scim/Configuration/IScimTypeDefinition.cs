namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IScimTypeDefinition
    {
        Type DefinitionType { get; }

        IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> AttributeDefinitions { get; }
    }
}