namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public interface IScimTypeAttributeDefinitionBuilder
    {
        PropertyDescriptor AttributeDescriptor { get; }
    }
}