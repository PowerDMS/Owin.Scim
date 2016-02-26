namespace Owin.Scim.Validation
{
    using FluentValidation;
    using FluentValidation.Internal;

    public class ScimValidationContext<T> : ValidationContext<T>, IScimValidationContext
    {
        public ScimValidationContext(
            T instanceToValidate, 
            T existingRecordInstance, 
            PropertyChain propertyChain, 
            IValidatorSelector validatorSelector)
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
            ExistingResourceRecord = existingRecordInstance;
        }

        private ScimValidationContext(T instanceToValidate) 
            : base(instanceToValidate)
        {
        }

        private ScimValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) 
            : base(instanceToValidate, propertyChain, validatorSelector)
        {
        }

        public T ExistingResourceRecord { get; private set; }

        public object GetExistingRecord()
        {
            return ExistingResourceRecord;
        }
    }
}