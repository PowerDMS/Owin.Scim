namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Canonicalization;

    using Extensions;

    using FluentValidation;

    using Model;

    public abstract class ScimTypeAttributeDefinitionBuilder<T, TAttribute> : IScimTypeAttributeDefinition
    {
        private readonly ScimTypeDefinitionBuilder<T> _DeclaringTypeDefinition;

        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly IList<ICanonicalizationRule> _CanonicalizationRules; 

        protected ScimTypeAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor)
        {
            _DeclaringTypeDefinition = typeDefinition;
            _PropertyDescriptor = propertyDescriptor;
            _CanonicalizationRules = new List<ICanonicalizationRule>();

            // Initialize defaults
            CaseExact = false;
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
        
        public bool CaseExact { get; protected set; }

        public string Description { get; protected set; }

        public bool MultiValued { get; protected set; }

        public Mutability Mutability { get; protected set; }

        public bool Required { get; protected set; }

        public Returned Returned { get; protected set; }

        public Uniqueness Uniqueness { get; protected set; }

        public PropertyDescriptor AttributeDescriptor
        {
            get { return _PropertyDescriptor; }
        }

        public virtual IScimTypeDefinition DeclaringTypeDefinition
        {
            get { return _DeclaringTypeDefinition; }
        }

        public IEnumerable<ICanonicalizationRule> GetCanonicalizationRules()
        {
            return _CanonicalizationRules;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetDescription(string description)
        {
            Description = description;
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
            var func = new StatefulCanonicalizationFunc<TAttribute>(
                (TAttribute value, ref object state) =>
                {
                    rule.Invoke(value);
                    return value;
                });

            return AddCanonicalizationRule(func);
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(CanonicalizationFunc<TAttribute> rule)
        {
            var func = new StatefulCanonicalizationFunc<TAttribute>((TAttribute value, ref object state) => rule.Invoke(value));

            return AddCanonicalizationRule(func);
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulCanonicalizationAction<TAttribute> rule)
        {
            var func = new StatefulCanonicalizationFunc<TAttribute>(
                (TAttribute value, ref object state) =>
                {
                    rule.Invoke(value, ref state);
                    return value;
                });

            return AddCanonicalizationRule(func);
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalizationRule(StatefulCanonicalizationFunc<TAttribute> rule)
        {
            _CanonicalizationRules.Add(new CanonicalizationRule<TAttribute>(_PropertyDescriptor, rule));
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> ClearCanonicalizationRules()
        {
            _CanonicalizationRules.Clear();
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddCanonicalValues(
            IEnumerable<TAttribute> acceptableValues,
            IEqualityComparer<TAttribute> comparer = null)
        {
            // TODO: (DG) impl
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddSchemaExtension<TExtension, TValidator>(
            string schemaIdentifier, 
            bool required = false,
            Action<ScimTypeDefinitionBuilder<TExtension>> extensionBuilder = null)
            where TExtension : ResourceExtension, new()
            where TValidator : IValidator<TExtension>
        {
            if (!typeof (Resource).IsAssignableFrom(typeof (T)))
                throw new InvalidOperationException("You cannot add schema extensions to non-resource types.");
            
            var extensionDefinition = new ScimTypeDefinitionBuilder<TExtension>();

            ((IScimResourceTypeDefinition)_DeclaringTypeDefinition)
                .AddExtension(
                    new ScimResourceTypeExtension(
                    schemaIdentifier,
                    required,
                    extensionDefinition,
                    typeof(TExtension),
                    typeof(TValidator)));

            extensionBuilder?.Invoke(extensionDefinition);

            return this;
        }
    }
}