namespace Owin.Scim
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Model;

    using NContext.Common;

    /// <summary>
    /// Defines a data-transfer-object used for functional composition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "ServiceResponseOf{0}")]
    [KnownType(typeof(ScimErrorResponse<>))]
    [KnownType(typeof(ScimDataResponse<>))]
    public abstract class ScimResponse<T> : Either<IEnumerable<ScimError>, T>, IScimResponse<T>
    {
    }
}