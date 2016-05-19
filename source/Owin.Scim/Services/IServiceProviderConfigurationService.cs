namespace Owin.Scim.Services
{
    using System.Threading.Tasks;

    using Model;

    public interface IServiceProviderConfigurationService
    {
        Task<IScimResponse<ServiceProviderConfiguration>>  GetServiceProviderConfiguration();
    }
}