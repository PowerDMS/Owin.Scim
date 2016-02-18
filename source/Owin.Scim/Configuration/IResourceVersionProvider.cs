namespace Owin.Scim.Configuration
{
    using Model;

    public interface IResourceVersionProvider
    {
        string GenerateVersion(Resource resource);
    }
}