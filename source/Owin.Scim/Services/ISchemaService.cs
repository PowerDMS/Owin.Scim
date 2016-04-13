namespace Owin.Scim.Services
{
    using System.Collections.Generic;

    using Model;

    public interface ISchemaService
    {
        IScimResponse<ScimSchema> GetSchema(string schemaId);

        IScimResponse<IEnumerable<ScimSchema>> GetSchemas();
    }
}