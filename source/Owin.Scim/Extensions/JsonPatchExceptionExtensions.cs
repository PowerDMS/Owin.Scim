namespace Owin.Scim.Extensions
{
    using System;
    using System.Net;
    
    using Model;

    using Patching.Exceptions;
    using Patching.Operations;

    public static class JsonPatchExceptionExtensions
    {
        public static ScimError ToScimError(this JsonPatchException exception)
        {
            return new ScimError(
                GetStatusCode(exception),
                GetScimType(exception),
                GetDetail(exception));
        }

        private static HttpStatusCode GetStatusCode(JsonPatchException exception)
        {
            return HttpStatusCode.BadRequest;
        }

        private static ScimType GetScimType(JsonPatchException exception)
        {
            return ScimType.InvalidPath;

            switch (exception.FailedOperation.OperationType)
            {
                case OperationType.Add:
                    break;
                case OperationType.Remove:
                    break;
                case OperationType.Replace:
                    break;
                case OperationType.Move:
                    break;
                case OperationType.Copy:
                    break;
                case OperationType.Test:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetDetail(JsonPatchException exception)
        {
            return null;
        }
    }
}