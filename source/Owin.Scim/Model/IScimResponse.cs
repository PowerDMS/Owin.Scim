namespace Owin.Scim.Model
{
    using System.Collections.Generic;

    using NContext.Common;

    public interface IScimResponse<T> : IEither<IEnumerable<ScimError>, T>
    {
    }
}