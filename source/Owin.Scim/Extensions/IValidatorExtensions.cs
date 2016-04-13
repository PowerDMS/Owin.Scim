namespace Owin.Scim.Extensions
{
    using System;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Internal;

    using Validation;

    using ValidationResult = FluentValidation.Results.ValidationResult;

    public static class IValidatorExtensions
    {
        public static Task<ValidationResult> ValidateCreateAsync<T>(
            this IValidator validator, 
            T instance)
        {
            var ruleSet = RuleSetConstants.Create;
            var selector = new RulesetValidatorSelector(ruleSet.Split(',', ';'));
            
            if (typeof(T) == instance.GetType())
            {
                return validator.ValidateAsync(
                    new ValidationContext<T>(instance, new PropertyChain(), selector));
            }

            // we are dealing with a polymorphic type
            var resourceType = instance.GetType();
            var validationContext = (ValidationContext)Activator.CreateInstance(
                typeof(ValidationContext<>).MakeGenericType(resourceType),
                instance,
                new PropertyChain(),
                selector);

            return validator.ValidateAsync(validationContext);
        }

        public static Task<ValidationResult> ValidateUpdateAsync<T>(
            this IValidator validator,
            T instance,
            T existingRecordInstance)
        {
            var ruleSet = RuleSetConstants.Update;
            var selector = new RulesetValidatorSelector(ruleSet.Split(',', ';'));

            if (typeof(T) == instance.GetType())
            {
                return validator.ValidateAsync(
                    new ScimValidationContext<T>(instance, existingRecordInstance, new PropertyChain(), selector));
            }

            // we are dealing with a polymorphic type
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