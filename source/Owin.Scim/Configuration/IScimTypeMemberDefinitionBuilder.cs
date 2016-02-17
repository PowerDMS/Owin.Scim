namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public interface IScimTypeMemberDefinitionBuilder
    {
        PropertyDescriptor Member { get; }
    }
}