namespace Owin.Scim.Configuration
{
    public interface IScimResourceTypeDefinition
    {
        string Description { get; }

        string Endpoint { get; }

        string Name { get; }

        string Schema { get; }
    }
}