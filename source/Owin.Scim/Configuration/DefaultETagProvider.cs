namespace Owin.Scim.Configuration
{
    using System.Security.Cryptography;

    using Model;

    using NContext.Security.Cryptography;

    public class DefaultETagProvider : IResourceETagProvider
    {
        private readonly IProvideHashing _HashProvider;

        public DefaultETagProvider(IProvideHashing hashProvider)
        {
            _HashProvider = hashProvider;
        }

        public string GenerateETag(Resource resource)
        {
            return _HashProvider.CreateHash<SHA1Cng>(resource.GenerateETagHash(), 0);
        }
    }
}