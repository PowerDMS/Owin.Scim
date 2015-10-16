namespace Owin.Scim.Security
{
    using System.Threading.Tasks;

    public interface IVerifyPasswordComplexity
    {
        Task<bool> MeetsRequirements(string password);
    }
}