namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Canonicalization;

    public interface IScimTypeAttributeDefinition
    {
        PropertyDescriptor AttributeDescriptor { get; }

        bool MultiValued { get; }

        string Description { get; }

        Mutability Mutability { get; }

        bool Required { get; }

        Returned Returned { get; }

        Uniqueness Uniqueness { get; }

        bool CaseExact { get; }

        IScimTypeDefinition DeclaringTypeDefinition { get; }

        IEnumerable<ICanonicalizationRule> GetCanonicalizationRules();
    }
}