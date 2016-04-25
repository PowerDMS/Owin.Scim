namespace Owin.Scim.Services
{
    using Configuration;

    public interface ICanonicalizationService
    {
        void Canonicalize(object instance, IScimTypeDefinition typeDefinition);
    }
}