namespace Owin.Scim
{
    using System.Collections.Generic;

    using Model;

    using NContext.Common;

    public interface IScimResponse<T> : IEither<IEnumerable<ScimError>, T>
    {
    }
}