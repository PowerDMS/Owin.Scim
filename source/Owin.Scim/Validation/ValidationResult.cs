namespace Owin.Scim.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValidationResult
    {
        private readonly IEnumerable<string> _ErrorMessages;

        private readonly bool _IsValid;

        public ValidationResult(int httpStatusCode = 400, IEnumerable<string> errorMessages = null)
        {
            _IsValid = errorMessages == null || !errorMessages.Any();
            _ErrorMessages = errorMessages;
        }

        public static implicit operator bool(ValidationResult result)
        {
            return result._IsValid;
        }

        public IEnumerable<string> ErrorMessages
        {
            get { return _ErrorMessages; }
        }
    }
}