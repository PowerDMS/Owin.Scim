namespace Owin.Scim.Extensions
{
    using System.Linq;

    using Model;

    using Validation;

    public static class ValidationResultExtensions
    {
        public static ValidationResult ToScimValidationResult(this FluentValidation.Results.ValidationResult result)
        {
            return new ValidationResult(
                result.Errors.Any()
                    ? result.Errors.Select(e => (ScimError)e.CustomState) 
                    : null);
        }
    }
}