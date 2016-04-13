namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Canonicalization;

    public interface IScimTypeAttributeDefinition
    {
        PropertyDescriptor AttributeDescriptor { get; }

        ISet<object> CanonicalValues { get; }

        bool CaseExact { get; }

        IScimTypeDefinition DeclaringTypeDefinition { get; }

        string Description { get; }

        bool MultiValued { get; }

        Mutability Mutability { get; }

        string Name { get; }

        IEnumerable<string> ReferenceTypes { get; }

        bool Required { get; }

        Returned Returned { get; }

        Uniqueness Uniqueness { get; }

        IEnumerable<ICanonicalizationRule> GetCanonicalizationRules();
    }
}