namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using Model;

    public class ScimTypeMemberDefinitionBuilder<T, TMember> : IScimTypeMemberDefinitionBuilder
    {
        private readonly ScimTypeDefinitionBuilder<T> _ScimTypeDefinitionBuilder;

        private readonly PropertyDescriptor _Descriptor;

        protected ScimTypeMemberDefinitionBuilder(
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
        }

        public static implicit operator ScimServerConfiguration(ScimTypeMemberDefinitionBuilder<T, TMember> builder)
        {
            return builder._ScimTypeDefinitionBuilder.ScimServerConfiguration;
        }

        internal string Description { get; set; }

        internal Mutable Mutability { get; set; }

        internal Return Returned { get; set; }

        internal Unique Uniqueness { get; set; }

        internal bool CaseExact { get; set; }

        protected internal ScimTypeDefinitionBuilder<T> ScimTypeDefinitionBuilder
        {
            get { return _ScimTypeDefinitionBuilder; }
        }

        public PropertyDescriptor Member
        {
            get { return _Descriptor; }
        }
        
        public ScimTypeScalarMemberDefinitionBuilder<T, TOtherMember> For<TOtherMember>(
            Expression<Func<T, TOtherMember>> memberExp)
            where TOtherMember : IConvertible
        {
            if (memberExp == null) throw new ArgumentNullException("memberExp");

            var memberExpression = memberExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("memberExp must be of type MemberExpression.");
            }
            
            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            return (ScimTypeScalarMemberDefinitionBuilder<T, TOtherMember>)ScimTypeDefinitionBuilder.MemberDefinitions[propertyDescriptor];
        }

        public ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute> For<TMultiValuedAttribute>(
            Expression<Func<T, IEnumerable<TMultiValuedAttribute>>> memberExp)
            where TMultiValuedAttribute : MultiValuedAttribute
        {
            if (memberExp == null) throw new ArgumentNullException("memberExp");

            var memberExpression = memberExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("memberExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            return (ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute>)ScimTypeDefinitionBuilder.MemberDefinitions[propertyDescriptor];
        }
    }
}