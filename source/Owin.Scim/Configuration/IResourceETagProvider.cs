namespace Owin.Scim.Configuration
{
    using Model;

    public interface IResourceETagProvider
    {
        string GenerateETag(Resource resource);
    }
}