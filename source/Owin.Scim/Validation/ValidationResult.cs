namespace Owin.Scim.Validation
{
    using System.Collections.Generic;
    using System.Linq;
    
    using Owin.Scim.Model;

    public class ValidationResult
    {
        private readonly bool _IsValid;

        private readonly IEnumerable<ScimError> _Errors;

        public ValidationResult(IEnumerable<ScimError> errors = null)
        {
            _IsValid = errors == null || !errors.Any();
            _Errors = errors == null ? null : errors.ToList();
        }

        public static implicit operator bool(ValidationResult result)
        {
            return result._IsValid;
        }

        public IEnumerable<ScimError> Errors
        {
            get { return _Errors; }
        }
    }
}