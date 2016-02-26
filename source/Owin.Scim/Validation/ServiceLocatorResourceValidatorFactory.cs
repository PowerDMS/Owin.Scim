namespace Owin.Scim.Validation
{
    using System.Threading.Tasks;

    using Configuration;

    using DryIoc;

    using FluentValidation;

    using Model;

    public class ServiceLocatorResourceValidatorFactory : IResourceValidatorFactory
    {
        private readonly IContainer _Container;

        private readonly ScimServerConfiguration _ServerConfiguration;

        public ServiceLocatorResourceValidatorFactory(
            IContainer container,
            ScimServerConfiguration serverConfiguration)
        {
            _Container = container;
            _ServerConfiguration = serverConfiguration;
        }

        public virtual Task<IValidator> CreateValidator<TResource>(TResource resource) 
            where TResource : Resource
        {
            return Task.FromResult(
                (IValidator)_Container.Resolve(
                    _ServerConfiguration.GetScimResourceValidatorType(resource.GetType())));
        }
    }
}