namespace Owin.Scim.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    public class ResourceExtensionValidators
    {
        private readonly IDictionary<string, IResourceExtensionValidator> _Validators;

        public ResourceExtensionValidators(IEnumerable<IResourceExtensionValidator> validators)
        {
            if (validators != null)
            {
                _Validators = validators
                    .ToDictionary(
                        v => v.ExtensionSchema,
                        v => v);
            }
        }

        public IValidator this[string schemaIdentifier]
        {
            get { return _Validators[schemaIdentifier]; }
        }
    }
}