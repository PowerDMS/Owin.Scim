namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;
    using System.Reflection;

    [InheritedExport]
    public interface IScimTypeDefinition
    {
        string Description { get; }

        string Name { get; }

        Type DefinitionType { get; }

        IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> AttributeDefinitions { get; }

        IScimTypeAttributeDefinition GetAttributeDefinition<TAttribute>(Expression<Func<TAttribute>> attributeExpression);
    }
}