namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Canonicalization;

    using Extensions;

    using Newtonsoft.Json;

    public abstract class ScimTypeAttributeDefinitionBuilder<T, TAttribute> : IScimTypeAttributeDefinition
    {
        private readonly IScimTypeDefinition _DeclaringTypeDefinition;

        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly IList<ICanonicalizationRule> _CanonicalizationRules; 

        protected ScimTypeAttributeDefinitionBuilder(
            IScimTypeDefinition typeDefinition,
            PropertyDescriptor propertyDescriptor,
            bool multiValued)
        {
            _DeclaringTypeDefinition = typeDefinition;
            _PropertyDescriptor = propertyDescriptor;
            _CanonicalizationRules = new List<ICanonicalizationRule>();

            // Initialize defaults
            CaseExact = false;
            MultiValued = multiValued;
            Mutability = Mutability.ReadWrite;
            Required = false;
            Returned = Returned.Default;
            Uniqueness = Uniqueness.None;

            var descriptionAttr = propertyDescriptor
                .Attributes
                .Cast<Attribute>()
                .SingleOrDefault(attr => attr is DescriptionAttribute) as DescriptionAttribute;

            if (descriptionAttr != null)
            {
                Description = descriptionAttr.Description.RemoveMultipleSpaces();
            }
        }

        public string Name
        {
            get
            {
                var jsonPropertyAttribute = _PropertyDescriptor.Attributes.OfType<JsonPropertyAttribute>().SingleOrDefault();
                if (jsonPropertyAttribute != null) return jsonPropertyAttribute.PropertyName;

                return _PropertyDescriptor.Name.LowercaseFirstCharacter();
            }
        }

        public ISet<object> CanonicalValues { get; protected set; }

        public IEqualityComparer CanonicalValueComparer { get; protected set; }

        public bool CaseExact { get; protected set; }

        public string Description { get; protected set; }

        public bool MultiValued { get; protected set; }

        public Mutability Mutability { get; protected set; }

        public IEnumerable<string> ReferenceTypes { get; protected set; }

        public bool Required { get; protected set; }

        public Returned Returned { get; protected set; }

        public Uniqueness Uniqueness { get; protected set; }

        public PropertyDescriptor AttributeDescriptor
        {
            get { return _PropertyDescriptor; }
        }

        public IScimTypeDefinition DeclaringTypeDefinition
        {
            get { return _DeclaringTypeDefinition; }
        }

        public IEnumerable<ICanonicalizationRule> GetCanonicalizationRules()
        {
            return _CanonicalizationRules;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetDescription(string description)
        {
            if (description == null)
                return this;

            Description = description.RemoveMultipleSpaces();
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetMutability(Mutability mutability)
        {
            Mutability = mutability;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetRequired(bool required)
        {
            Required = required;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetReturned(Returned returned)
        {
            Returned = returned;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetUniqueness(Uniqueness uniqueness)
        {
            Uniqueness = uniqueness;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(CanonicalizationAction<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new CanonicalizationRule<TAttribute>(_PropertyDescriptor, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(AttributeCanonicalizationAction<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new AttributeCanonicalizationRule<TAttribute>(_PropertyDescriptor, this, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulCanonicalizationAction<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new StatefulCanonicalizationRule<TAttribute>(_PropertyDescriptor, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulAttributeCanonicalizationAction<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new StatefulAttributeCanonicalizationRule<TAttribute>(_PropertyDescriptor, this, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(CanonicalizationFunc<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new CanonicalizationRule<TAttribute>(_PropertyDescriptor, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(AttributeCanonicalizationFunc<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new AttributeCanonicalizationRule<TAttribute>(_PropertyDescriptor, this, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulCanonicalizationFunc<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new StatefulCanonicalizationRule<TAttribute>(_PropertyDescriptor, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulAttributeCanonicalizationFunc<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new StatefulAttributeCanonicalizationRule<TAttribute>(_PropertyDescriptor, this, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> ClearCanonicalizationRules()
        {
            _CanonicalizationRules.Clear();
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCanonicalValues(IEnumerable<TAttribute> canonicalValues)
        {
            return SetCanonicalValues(canonicalValues, EqualityComparer<TAttribute>.Default);
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCanonicalValues<TEqualityComparer>(
            IEnumerable<TAttribute> canonicalValues,
            TEqualityComparer comparer)
            where TEqualityComparer : class, IEqualityComparer<TAttribute>, IEqualityComparer
        {
            IEqualityComparer<TAttribute> equalityComparer = comparer;
            if (comparer == null)
                equalityComparer = EqualityComparer<TAttribute>.Default;
            
            CanonicalValues = new HashSet<object>(canonicalValues.Distinct(equalityComparer).Cast<object>());
            CanonicalValueComparer = (IEqualityComparer)equalityComparer;

            return this;
        }
    }
}