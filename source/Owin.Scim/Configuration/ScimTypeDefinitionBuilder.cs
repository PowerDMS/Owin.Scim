namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Extensions;
   
    public class ScimTypeDefinitionBuilder<T> : IScimTypeDefinition
    {
        private readonly IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> _AttributeDefinitions;
        
        public ScimTypeDefinitionBuilder()
        {
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
        
        public Type DefinitionType
        {
            get { return typeof(T); }
        }

        public string Description { get; private set; }
        
        public IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> AttributeDefinitions
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
            
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[(PropertyInfo)memberExpression.Member];
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> For<TAttribute>(
            Expression<Func<T, ISet<TAttribute>>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[(PropertyInfo)memberExpression.Member];
        }

        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> For<TAttribute>(
            Expression<Func<T, ICollection<TAttribute>>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[(PropertyInfo)memberExpression.Member];
        }
        
        public ScimTypeAttributeDefinitionBuilder<T, TAttribute> For<TAttribute>(
            Expression<Func<T, IList<TAttribute>>> attrExp)
        {
            if (attrExp == null) throw new ArgumentNullException("attrExp");

            var memberExpression = attrExp.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException("attrExp must be of type MemberExpression.");
            }

            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[(PropertyInfo)memberExpression.Member];
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
            
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[(PropertyInfo)memberExpression.Member];
        }

        private IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> BuildDefaultTypeDefinitions()
        {
            return TypeDescriptor.GetProperties(typeof(T))
                .Cast<PropertyDescriptor>()
                .Where(d => !d.Attributes.Contains(new ScimInternalAttribute()))
                .ToDictionary(
                    d => d.ComponentType.GetProperty(d.Name),
                    CreateTypeMemberDefinitionBuilder);
        }

        private IScimTypeAttributeDefinition CreateTypeMemberDefinitionBuilder(PropertyDescriptor descriptor)
        {
            Type builder;
            IScimTypeAttributeDefinition instance;

            // scalar attribute
            if (descriptor.PropertyType.IsTerminalObject())
            {
                builder = typeof(Uri).IsAssignableFrom(descriptor.PropertyType)
                    ? typeof(ScimTypeUriAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType)
                    : typeof(ScimTypeScalarAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
                instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor);

                return instance;
            }

            if (descriptor.PropertyType.IsNonStringEnumerable())
            {
                var itemType = descriptor.PropertyType.IsArray
                    ? descriptor.PropertyType.GetElementType()
                    : descriptor.PropertyType.GetGenericArguments()[0];

                if (itemType.IsTerminalObject())
                {
                    builder = typeof(Uri).IsAssignableFrom(descriptor.PropertyType)
                        ? typeof(ScimTypeUriAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType)
                        : typeof(ScimTypeScalarAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType);
                    instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor);

                    return instance;
                }

                // multiValued complex attribute
                builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType);
                instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor, true);

                return instance;
            }

            // complex attribute
            builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
            instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor, false);

            return instance;
        }
    }
}