namespace Owin.Scim
{
    using Model;

    using NContext.Common;

    public interface IScimResponse<T> : IEither<ScimError, T>
    {
    }
}