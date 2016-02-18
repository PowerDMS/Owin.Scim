namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Extensions;

    public class ScimTypeAttributeDefinitionBuilder<T, TAttribute> : IScimTypeAttributeDefinitionBuilder
    {
        private readonly ScimTypeDefinitionBuilder<T> _ScimTypeDefinitionBuilder;

        private readonly PropertyDescriptor _Descriptor;

        protected ScimTypeAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor)
        {
            _ScimTypeDefinitionBuilder = scimTypeDefinitionBuilder;
            _Descriptor = descriptor;

            // Initialize defaults
            Mutability = Mutable.ReadWrite;
            Returned = Return.Default;
            Uniqueness = Unique.None;
            CaseExact = false;

            var descriptionAttr = descriptor
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

        internal string Description { get; set; }

        internal Mutable Mutability { get; set; }

        internal Return Returned { get; set; }

        internal Unique Uniqueness { get; set; }

        internal bool CaseExact { get; set; }

        internal bool MultiValued { get; set; }

        protected internal ScimTypeDefinitionBuilder<T> ScimTypeDefinitionBuilder
        {
            get { return _ScimTypeDefinitionBuilder; }
        }

        public PropertyDescriptor AttributeDescriptor
        {
            get { return _Descriptor; }
        }


        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetMutability(Mutable mutability)
        {
            Mutability = mutability;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetReturned(Return returned)
        {
            Returned = returned;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetUniqueness(Unique uniqueness)
        {
            Uniqueness = uniqueness;
            return this;
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCaseExact(bool caseExact)
        {
            CaseExact = caseExact;
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
            return (ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute>)ScimTypeDefinitionBuilder.MemberDefinitions[propertyDescriptor];
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
            return (ScimTypeAttributeDefinitionBuilder<T, TOtherAttribute>)ScimTypeDefinitionBuilder.MemberDefinitions[propertyDescriptor];
        }
    }
}