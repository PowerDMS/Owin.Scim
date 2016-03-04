namespace Owin.Scim.Validation
{
    using FluentValidation;

    public interface IResourceExtensionValidator : IValidator
    {
        string ExtensionSchema { get; }
    }
}