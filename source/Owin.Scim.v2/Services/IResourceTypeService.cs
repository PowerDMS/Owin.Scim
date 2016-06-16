namespace Owin.Scim.v2.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model;

    public interface IResourceTypeService
    {
        Task<IScimResponse<IEnumerable<ResourceType>>> GetResourceTypes();

        Task<IScimResponse<ResourceType>> GetResourceType(string name);
    }
}