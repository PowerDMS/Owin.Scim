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
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly ScimTypeAttributeDefinitions _AttributeDefinitions;
        
        public ScimTypeDefinitionBuilder(ScimServerConfiguration serverConfiguration)
        {
            _ServerConfiguration = serverConfiguration;
            _AttributeDefinitions = BuildDefaultTypeDefinitions();
            
            var descriptionAttr = TypeDescriptor
                .GetAttributes(typeof(T))
                .Cast<Attribute>()
                .SingleOrDefault(attr => attr is DescriptionAttribute) as DescriptionAttribute;

            if (descriptionAttr != null)
            {
                SetDescription(descriptionAttr.Description.RemoveMultipleSpaces());
            }
        }

        /// <summary>
        /// Gets the type of which the definition corresponds to. This returns the type argument <typeparamref name="T"/>.
        /// </summary>
        /// <value>The type of the definition.</value>
        public Type DefinitionType
        {
            get { return typeof(T); }
        }

        public string Description { get; private set; }

        public string Name { get; private set; }

        public IReadOnlyDictionary<PropertyInfo, IScimTypeAttributeDefinition> AttributeDefinitions
        {
            get { return _AttributeDefinitions.Definitions; }
        }

        public ScimServerConfiguration ServerConfiguration
        {
            get { return _ServerConfiguration; }
        }

        public ScimTypeDefinitionBuilder<T> SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public ScimTypeDefinitionBuilder<T> SetName(string name)
        {
            Name = name;
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
            
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[GetPropertyInfo(memberExpression)];
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
            
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[GetPropertyInfo(memberExpression)];
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

            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[GetPropertyInfo(memberExpression)];
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

            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[GetPropertyInfo(memberExpression)];
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
            
            return (ScimTypeAttributeDefinitionBuilder<T, TAttribute>)_AttributeDefinitions[GetPropertyInfo(memberExpression)];
        }

        private ScimTypeAttributeDefinitions BuildDefaultTypeDefinitions()
        {
            return new ScimTypeAttributeDefinitions(
                TypeDescriptor.GetProperties(typeof(T))
                    .Cast<PropertyDescriptor>()
                    .Where(d => !d.Attributes.Contains(new ScimInternalAttribute()))
                    .ToDictionary(GetPropertyInfo, CreateTypeMemberDefinitionBuilder));
        }

        private IScimTypeAttributeDefinition CreateTypeMemberDefinitionBuilder(PropertyDescriptor descriptor)
        {
            Type builder;
            IScimTypeAttributeDefinition instance;
            IScimTypeDefinition typeDefinition;

            // scalar attribute
            if (descriptor.PropertyType.IsTerminalObject())
            {
                if (typeof (Uri).IsAssignableFrom(descriptor.PropertyType))
                    builder = typeof (ScimTypeUriAttributeDefinitionBuilder<,>).MakeGenericType(typeof (T), descriptor.PropertyType);
                else
                    builder = typeof (ScimTypeScalarAttributeDefinitionBuilder<,>).MakeGenericType(typeof (T), descriptor.PropertyType);

                instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor, false);

                return instance;
            }
            
            // enumerable attribute
            if (descriptor.PropertyType.IsNonStringEnumerable())
            {
                var itemType = descriptor.PropertyType.IsArray
                    ? descriptor.PropertyType.GetElementType()
                    : descriptor.PropertyType.GetGenericArguments()[0];

                // enumerable type is a scalar data-type
                if (itemType.IsTerminalObject())
                {
                    builder = typeof(Uri).IsAssignableFrom(descriptor.PropertyType)
                        ? typeof(ScimTypeUriAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType)
                        : typeof(ScimTypeScalarAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType);
                    instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(builder, this, descriptor, true);

                    return instance;
                }

                // enumerable type is a complex attribute
                typeDefinition = itemType == typeof (T) // circular reference check; e.g. ScimAttributeSchema
                    ? this
                    : ServerConfiguration.GetScimTypeDefinition(itemType);
                builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), itemType);
                instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(
                    builder, 
                    typeDefinition, 
                    descriptor, 
                    true);

                return instance;
            }

            // complex attribute
            typeDefinition = descriptor.PropertyType == typeof(T) // circular reference check
                ? this
                : ServerConfiguration.GetScimTypeDefinition(descriptor.PropertyType);
            builder = typeof(ScimTypeComplexAttributeDefinitionBuilder<,>).MakeGenericType(typeof(T), descriptor.PropertyType);
            instance = (IScimTypeAttributeDefinition)Activator.CreateInstance(
                builder, 
                typeDefinition, 
                descriptor, 
                false);

            return instance;
        }

        private PropertyInfo GetPropertyInfo(MemberExpression expression)
        {
            // A MemberExpression from a LambdaExpression.Body gives us the Member.PropertyInfo without taking into account
            // inheritance. See. http://stackoverflow.com/questions/6658669/lambda-expression-not-returning-expected-memberinfo
            // For(user => user.Schemas) -> expression.Member gives us a PropertyInfo whose DeclaringType is SchemaBase not Resource

            return GetPropertyInfo(
                TypeDescriptor.GetProperties(typeof(T))
                    .Find(expression.Member.Name, false));
        }

        private PropertyInfo GetPropertyInfo(PropertyDescriptor descriptor)
        {
            try
            {
                return descriptor.ComponentType.GetProperty(descriptor.Name, BindingFlags.Instance | BindingFlags.Public);
            }
            catch (AmbiguousMatchException)
            {
                // In some circumstances, we hide base properties using the new operator and we only want the property on the declaring class.
                return descriptor.ComponentType.GetProperty(descriptor.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            }
        }
    }
}