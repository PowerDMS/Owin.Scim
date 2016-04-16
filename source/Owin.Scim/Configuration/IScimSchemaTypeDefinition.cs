namespace Owin.Scim.Configuration
{
    public interface IScimSchemaTypeDefinition : IScimTypeDefinition
    {
        string Schema { get; }
    }
}