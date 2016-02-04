namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface ISchemaTypeFactory
    {
        Type GetSchemaType(ISet<string> schemaIdentifiers);
    }
}