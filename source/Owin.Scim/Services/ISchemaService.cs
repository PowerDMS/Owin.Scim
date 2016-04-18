namespace Owin.Scim.Services
{
    using System.Collections.Generic;

    using Model;

    /// <summary>
    /// Application service for retrieving schema information.
    /// </summary>
    public interface ISchemaService
    {
        /// <summary>
        /// Gets the <see cref="ScimSchema"/> associated with the specified <paramref name="schemaId"/>.
        /// </summary>
        /// <param name="schemaId">The schema identifier.</param>
        /// <returns>IScimResponse&lt;ScimSchema&gt;.</returns>
        IScimResponse<ScimSchema> GetSchema(string schemaId);

        /// <summary>
        /// Gets all defined <see cref="ScimSchema"/>s.
        /// </summary>
        /// <returns>IScimResponse&lt;IEnumerable&lt;ScimSchema&gt;&gt;.</returns>
        IScimResponse<IEnumerable<ScimSchema>> GetSchemas();
    }
}