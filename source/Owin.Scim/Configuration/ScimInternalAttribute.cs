namespace Owin.Scim.Configuration
{
    using System;

    /// <summary>
    /// Applying this attribute to resource members will instruct Owin.Scim that the member is 
    /// not a defined SCIM attribute, but is internal for development usage.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ScimInternalAttribute : Attribute
    {
    }
}