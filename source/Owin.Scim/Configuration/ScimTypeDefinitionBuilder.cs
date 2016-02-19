namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;

    using Extensions;

    public class ScimTypeDefinitionBuilder<T> : IScimTypeDefinitionBuilder
    {
        private readonly ScimServerConfiguration _ScimServerConfiguration;

        private readonly IDictionary<PropertyDescriptor, IScimTypeAttributeDefinitionBuilder> _AttributeDefinitions;

        public ScimTypeDefinitionBuilder(ScimServerConfiguration configuration)
        {
            _ScimServerConfiguration = configuration;
            _AttributeDefinitions = BuildDefaultTypeDefinitions();
            
            var descriptionAttr = TypeDescriptor
                .GetAttributes(typeof(T))
                .Cast<Attribute>()
                .SingleOrDefault(attr => attr is DescriptionAttribute) as DescriptionAttribute;

            if (descriptionAttr != null)
            {
                Description = descriptionAttr.Description.RemoveMultipleSpaces();
            }
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

        protected internal IDictionary<PropertyDescriptor, IScimTypeAttributeDefinitionBuilder> AttributeDefinitions
        {
            get { return _AttributeDefinitions; }
        }

        public ScimTypeDefinitionBuilder<T> SetDescription(string description)
        {
            Description = description;
            return this;
        }
        
        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> For<TAttribute>(
            Expression<Func<T, TAttribute>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[propertyDescriptor];
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> For<TAttribute>(
            Expression<Func<T, IEnumerable<TAttribute>>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            var propertyDescriptor = TypeDescriptor.GetProperties(typeof(T)).Find(memberExpression.Member.Name, true);
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[propertyDescriptor];
        }


        private IDictionary<PropertyDescriptor, IScimTypeAttributeDefinitionBuilder> BuildDefaultTypeDefinitions()
        {
            return TypeDescriptor.GetProperties(typeof(T))
                .OfType<PropertyDescriptor>()
                .ToDictionary(
                    d => d,
                    d => CreateTypeMemberDefinitionBuilder(d));
        }

        private IScimTypeAttributeDefinitionBuilder CreateTypeMemberDefinitionBuilder(PropertyDescriptor descriptor)
        {
            Type builder;
            IScimTypeAttributeDefinitionBuilder instance;

            // scalar attribute
            if (descriptor.PropertyType.IsTerminalObject())
            {
                builder = typeof(Uri).IsAssignableFrom(descriptor.PropertyType)
                    ? typeof(ScimTypeUriAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType)
                    : typeof(ScimTypeScalarAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
                instance = (IScimTypeAttributeDefinitionBuilder)Activator.CreateInstance(builder, this, descriptor);

                return instance;
            }

            // multiValued complex attribute
            if (descriptor.PropertyType.IsNonStringEnumerable())
            {
                builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>)
                    .MakeGenericType(typeof(T), descriptor.PropertyType.GetGenericArguments()[0]);
                instance = (IScimTypeAttributeDefinitionBuilder)Activator.CreateInstance(builder, this, descriptor, true);

                return instance;
            }

            // complex attribute
            builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
            instance = (IScimTypeAttributeDefinitionBuilder)Activator.CreateInstance(builder, this, descriptor, false);

            return instance;
        }
    }
}