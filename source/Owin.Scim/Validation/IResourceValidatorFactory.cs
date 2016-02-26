namespace Owin.Scim.Validation
{
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;

    public interface IResourceValidatorFactory
    {
        Task<IValidator> CreateValidator<TResource>(TResource resource) 
            where TResource : Resource;
    }
}