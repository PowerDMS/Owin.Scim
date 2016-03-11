namespace Owin.Scim.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ScimInternalAttribute : Attribute
    {
        
    }
}