namespace Owin.Scim.Model
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SchemaIdentifierAttribute : Attribute
    {
        public SchemaIdentifierAttribute(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; private set; }
    }
}