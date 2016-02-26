namespace Owin.Scim.Extensions
{
    using System;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Results;

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

            ValidationContext<T> validationContext = new ValidationContext<T>(instance, new PropertyChain(), selector);
            return validator.ValidateAsync(validationContext);
        }
    }
}