namespace Owin.Scim
{
    using Model;

    using NContext.Common;

    public interface IScimResponse<out T> : IEither<ScimError, T>
    {
    }
}