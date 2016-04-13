namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using FluentValidation;

    using Model;

    using NContext.Common;
    
    public class ScimResourceTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>, IScimResourceTypeDefinition
        where T : Resource
    {
        private readonly IDictionary<string, ScimResourceTypeExtension> _SchemaExtensions;

        private readonly string _Endpoint;

        private readonly string _Schema;

        private readonly Predicate<ISet<string>> _SchemaBindingRule;

        private Type _ValidatorType;

        public ScimResourceTypeDefinitionBuilder(
            string name, 
            string schema, 
            string endpoint,
            Type validatorType,
            Predicate<ISet<string>> schemaBindingRule)
        {
            _SchemaExtensions = new Dictionary<string, ScimResourceTypeExtension>();
            _Schema = schema;

            SetName(name);

            if (endpoint != null && !endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Insert(0, "/");
            }

            _Endpoint = endpoint;
            _ValidatorType = validatorType;
            _SchemaBindingRule = schemaBindingRule;
        }
        
        public string Endpoint
        {
            get { return _Endpoint; }
        }
        
        public string Schema
        {
            get { return _Schema; }
        }
        
        public Type ValidatorType
        {
            get { return _ValidatorType; }
        }
        
        public IEnumerable<ScimResourceTypeExtension> SchemaExtensions
        {
            get { return _SchemaExtensions.Values; }
        }

        public Predicate<ISet<string>> SchemaBindingRule
        {
            get { return _SchemaBindingRule; }
        }

        public ScimTypeDefinitionBuilder<T> AddSchemaExtension<TExtension, TValidator>(
            string schemaIdentifier,
            bool required = false)
            where TExtension : ResourceExtension, new()
            where TValidator : IValidator<TExtension>
        {
            var typeDefinition = TypeDescriptor.GetAttributes(typeof(TExtension))
                .OfType<ScimTypeDefinitionAttribute>()
                .MaybeSingle()
                .Bind(a =>
                {
                    if (!typeof(ScimTypeDefinitionBuilder<TExtension>).IsAssignableFrom(a.DefinitionType))
                        throw new InvalidOperationException(
                            string.Format(
                                "Type definition '{0}' must inherit from ScimTypeDefinitionBuilder<{1}>.",
                                a.DefinitionType.Name,
                                typeof(TExtension).Name));

                    return ((ScimTypeDefinitionBuilder<TExtension>)Activator.CreateInstance(a.DefinitionType)).ToMaybe();
                })
                .FromMaybe(new ScimTypeDefinitionBuilder<TExtension>());

            var extension = new ScimResourceTypeExtension(
                schemaIdentifier,
                required,
                typeDefinition,
                typeof (TExtension),
                typeof (TValidator));

            _SchemaExtensions.Add(extension.Schema, extension);

            return this;
        }

        public ScimTypeDefinitionBuilder<T> SetValidator<TValidator>()
            where TValidator : IValidator<T>
        {
            _ValidatorType = typeof (TValidator);
            return this;
        }

        ScimResourceTypeExtension IScimResourceTypeDefinition.GetExtension(string schemaIdentifier)
        {
            ScimResourceTypeExtension extension;
            if (_SchemaExtensions.TryGetValue(schemaIdentifier, out extension)) return extension;

            return null;
        }
    }
}