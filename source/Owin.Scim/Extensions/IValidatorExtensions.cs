namespace Owin.Scim.Extensions
{
    using System;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Internal;

    using Validation;
    using Validation.Users;

    using ValidationResult = FluentValidation.Results.ValidationResult;

    public static class IValidatorExtensions
    {
        public static Task<ValidationResult> ValidateAsync<T>(
            this IValidator validator, 
            T instance, 
            IValidatorSelector selector = null, 
            string ruleSet = null)
        {
            if (selector != null && ruleSet != null)
                throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");

            if (selector == null)
                selector = new DefaultValidatorSelector();

            if (ruleSet != null)
                selector = new RulesetValidatorSelector(ruleSet.Split(new char[2]
                {
                    ',',
                    ';'
                }));
            
            if (typeof(T) == instance.GetType())
            {
                return validator.ValidateAsync(
                    new ValidationContext<T>(instance, new PropertyChain(), selector));
            }

            // we are dealing with a polymorphic type, typically a resource extension
            var resourceType = instance.GetType();
            var validationContext = (ValidationContext)Activator.CreateInstance(
                typeof(ValidationContext<>).MakeGenericType(resourceType),
                instance,
                new PropertyChain(),
                selector);

            return validator.ValidateAsync(validationContext);
        }

        public static Task<ValidationResult> ValidateAsync<T>(
            this IValidator validator,
            T instance,
            T existingRecordInstance,
            IValidatorSelector selector = null,
            string ruleSet = null)
        {
            if (selector != null && ruleSet != null)
                throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");

            if (selector == null)
                selector = new DefaultValidatorSelector();

            if (ruleSet != null)
                selector = new RulesetValidatorSelector(ruleSet.Split(new char[2]
                {
                    ',',
                    ';'
                }));

            if (typeof(T) == instance.GetType())
            {
                return validator.ValidateAsync(
                    new ScimValidationContext<T>(instance, existingRecordInstance, new PropertyChain(), selector));
            }

            // we are dealing with a polymorphic type, typically a resource extension
            var resourceType = instance.GetType();
            var validationContext = (ValidationContext)Activator.CreateInstance(
                typeof (ScimValidationContext<>).MakeGenericType(resourceType),
                instance,
                existingRecordInstance,
                new PropertyChain(),
                selector);

            return validator.ValidateAsync(validationContext);
        }
    }
}