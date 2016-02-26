namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Canonicalization;

    using Extensions;

    using FluentValidation;

    using Model;

    public abstract class ScimTypeAttributeDefinitionBuilder<T, TAttribute> : IScimTypeAttributeDefinition
    {
        private readonly ScimTypeDefinitionBuilder<T> _ScimTypeDefinitionBuilder;

        private readonly PropertyDescriptor _PropertyDescriptor;

        private readonly IList<ICanonicalizationRule> _CanonicalizationRules; 

        protected ScimTypeAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor propertyDescriptor)
        {
            _ScimTypeDefinitionBuilder = scimTypeDefinitionBuilder;
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

        public static implicit operator ScimServerConfiguration(ScimTypeAttributeDefinitionBuilder<T, TAttribute> builder)
        {
            return builder._ScimTypeDefinitionBuilder.ScimServerConfiguration;
        }

        public string Description { get; protected set; }

        public Mutability Mutability { get; protected set; }

        public bool Required { get; protected set; }

        public Returned Returned { get; protected set; }

        public Uniqueness Uniqueness { get; protected set; }

        public bool CaseExact { get; protected set; }

        public IEnumerable<ICanonicalizationRule> GetCanonicalizationRules()
        {
            return _CanonicalizationRules;
        }

        public bool MultiValued { get; protected set; }
        
        public PropertyDescriptor AttributeDescriptor
        {
            get { return _PropertyDescriptor; }
        }

        public virtual IScimTypeDefinition TypeDefinition { get { return _ScimTypeDefinitionBuilder; } }

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

        public ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute> For<TOtherAttribute>(
            Expression<Func<T, TOtherAttribute>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof (T)).Find(memberExpression.Member.Name, true);
            return (ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute>)_ScimTypeDefinitionBuilder.AttributeDefinitions[propertyDescriptor];
        }

        public ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute> For<TOtherAttribute>(
            Expression<Func<T, IEnumerable<TOtherAttribute>>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);
            return (ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute>)_ScimTypeDefinitionBuilder.AttributeDefinitions[propertyDescriptor];
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

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> AddSchemaExtension<TResourceDerivative, TValidator, TExtension>(
            string schemaIdentifier, 
            bool required = false,
            Action<ScimTypeDefinitionBuilder<TExtension>> extensionBuilder = null)
            where TResourceDerivative : Resource, T
            where TExtension : class
            where TValidator : IValidator<TResourceDerivative>
        {
            if (!typeof (Resource).IsAssignableFrom(typeof (T)))
                throw new InvalidOperationException("You cannot add schema extensions to non-resource types.");

            if (!typeof (TResourceDerivative).ContainsSchemaExtension<TExtension>(schemaIdentifier))
                throw new InvalidOperationException(
                    string.Format(
                        @"To use type '{0}' as a schema extension, it must have a single property 
                        of type '{1}' with a JsonPropertyAttribute whose PropertyName is equal to '{2}'."
                            .RemoveMultipleSpaces(),
                        typeof (TResourceDerivative).Name,
                        typeof (TExtension).Name,
                        schemaIdentifier));

            var extensionDefinition = new ScimTypeDefinitionBuilder<TExtension>(_ScimTypeDefinitionBuilder.ScimServerConfiguration);

            ((IScimResourceTypeDefinition)_ScimTypeDefinitionBuilder)
                .AddExtension(
                    new ScimResourceTypeExtension(
                    schemaIdentifier,
                    required,
                    extensionDefinition,
                    typeof(TResourceDerivative),
                    typeof(TValidator)));

            extensionBuilder?.Invoke(extensionDefinition);

            return this;
        }
    }
}