namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IScimTypeDefinition
    {
        Type ResourceType { get; }

        IDictionary<PropertyDescriptor, IScimTypeAttributeDefinition> AttributeDefinitions { get; }
    }
}