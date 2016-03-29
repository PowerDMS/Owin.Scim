namespace Owin.Scim.Configuration
{
    using System.Security.Cryptography;

    using Model;

    using NContext.Security.Cryptography;

    public class DefaultResourceVersionProvider : IResourceVersionProvider
    {
        private readonly IProvideHashing _HashProvider;

        public DefaultResourceVersionProvider(IProvideHashing hashProvider)
        {
            _HashProvider = hashProvider;
        }

        protected IProvideHashing HashProvider
        {
            get { return _HashProvider; }
        }

        public virtual string GenerateVersion(Resource resource)
        {
            return HashProvider.CreateHash<SHA1Cng>(resource.CalculateVersion().ToString(), 0);
        }
    }
}