namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Extensions;

    using Model;

    public class ScimTypeDefinitionBuilder<T> : IScimTypeDefinitionBuilder
    {
        private readonly ScimServerConfiguration _ScimServerConfiguration;

        private readonly IDictionary<PropertyDescriptor, IScimTypeMemberDefinitionBuilder> _MemberDefinitions;

        public ScimTypeDefinitionBuilder(ScimServerConfiguration configuration)
        {
            _ScimServerConfiguration = configuration;
            _MemberDefinitions = BuildDefaultTypeDefinitions();
        }
        
        public Type ResourceType
        {
            get { return typeof(T); }
        }

        public string Description { get; private set; }

        protected internal ScimServerConfiguration ScimServerConfiguration
        {
            get { return _ScimServerConfiguration; }
        }

        protected internal IDictionary<PropertyDescriptor, IScimTypeMemberDefinitionBuilder> MemberDefinitions
        {
            get { return _MemberDefinitions; }
        }

        public ScimTypeDefinitionBuilder<T> SetDescription(string description)
        {
            Description = description;
            return this;
        }
        
        public ScimTypeScalarMemberDefinitionBuilder<T, TMember> For<TMember>(Expression<Func<T, TMember>> memberExp)
            where TMember : IConvertible
        {
            if (memberExp == null) throw new ArgumentNullException("memberExp");

            var memberExpression = memberExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("memberExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);

            return (ScimTypeScalarMemberDefinitionBuilder<T, TMember>)_MemberDefinitions[propertyDescriptor];
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

            return (ScimTypeMultiValuedAttributeDefinitionBuilder<T, TMultiValuedAttribute>)_MemberDefinitions[propertyDescriptor];
        }

        protected internal void AddMemberDefinition(IScimTypeMemberDefinitionBuilder builder)
        {
            _MemberDefinitions.Add(builder.Member, builder);
        }

        protected internal bool Contains(PropertyDescriptor propertyDescriptor)
        {
            return _MemberDefinitions.Any(kvp => kvp.Value != null && kvp.Value.Member.Equals(propertyDescriptor));
        }

        private IDictionary<PropertyDescriptor, IScimTypeMemberDefinitionBuilder> BuildDefaultTypeDefinitions()
        {
            return TypeDescriptor.GetProperties(typeof(T))
                .OfType<PropertyDescriptor>()
                .ToDictionary(
                    d => d,
                    d => CreateTypeMemberDefinitionBuilder(d));
        }

        private IScimTypeMemberDefinitionBuilder CreateTypeMemberDefinitionBuilder(PropertyDescriptor descriptor)
        {
            if (typeof(IConvertible).IsAssignableFrom(descriptor.PropertyType))
            {
                var builder = typeof(ScimTypeScalarMemberDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
                var instance = Activator.CreateInstance(builder, this, descriptor);

                return (IScimTypeMemberDefinitionBuilder)instance;
            }

            if (descriptor.PropertyType.IsGenericType &&
                descriptor.PropertyType.IsNonStringEnumerable() &&
                typeof(MultiValuedAttribute).IsAssignableFrom(descriptor.PropertyType.GetGenericArguments()[0]))
            {
                var builder = typeof(ScimTypeMultiValuedAttributeDefinitionBuilder<,>)
                    .MakeGenericType(typeof(T), descriptor.PropertyType.GetGenericArguments()[0]);
                var instance = Activator.CreateInstance(builder, this, descriptor);

                return (IScimTypeMemberDefinitionBuilder)instance;
            }

            return null; // TODO: (DG) Support ComplexProperties
        }
    }
}