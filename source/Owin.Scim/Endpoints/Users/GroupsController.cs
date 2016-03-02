namespace Owin.Scim.Endpoints.Users
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;
    using Extensions;
    using Model.Groups;
    using Services;

    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(
            ScimServerConfiguration scimServerConfiguration,
            IGroupService groupService) 
            : base(scimServerConfiguration)
        {
            _groupService = groupService;
        }

        [Route("groups", Name = "CreateGroup")]
        public async Task<HttpResponseMessage> Post(Group group)
        {
            return (await _groupService.CreateGroup(group))
                .ToHttpResponseMessage(Request, (groupDto, response) =>
                {
                    response.StatusCode = HttpStatusCode.Created;

                    SetLocationHeader(response, groupDto, "RetrieveGroup", new { groupId = groupDto.Id });
                    SetETagHeader(response, groupDto);
                });
        }

        [Route("groups/{groupId}", Name = "RetrieveGroup")]
        public async Task<HttpResponseMessage> Get(string groupId)
        {
            return (await _groupService.RetrieveGroup(groupId))
                .ToHttpResponseMessage(Request, (groupDto, response) =>
                {
                    SetLocationHeader(response, groupDto, "RetrieveGroup", new { groupId = groupDto.Id });
                    SetETagHeader(response, groupDto);
                });
        }
    }
}