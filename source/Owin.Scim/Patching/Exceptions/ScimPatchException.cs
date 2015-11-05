namespace Owin.Scim.Patching.Exceptions
{
    using System;

    using Model;

    using Operations;

    public class ScimPatchException : Exception 
    {
        public ScimErrorType ErrorType { get; private set; }

        public Operation FailedOperation { get; private set; }

        public ScimPatchException(
            ScimErrorType errorType,
            Operation operation)
        {
            ErrorType = errorType;
            FailedOperation = operation;
        }
    }
}