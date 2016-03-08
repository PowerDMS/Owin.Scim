namespace Owin.Scim.Endpoints.Users
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;
    using Extensions;
    using Model;
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

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("groups/{groupId}", Name = "ReplaceGroup")]
        public async Task<HttpResponseMessage> Put(string groupId, Group group)
        {
            if (String.IsNullOrWhiteSpace(groupId) ||
                group == null ||
                string.IsNullOrWhiteSpace(group.Id) ||
                !group.Id.Equals(groupId, StringComparison.OrdinalIgnoreCase))
            {
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidSyntax,
                        detail: "The request path 'groupId' MUST match the group.id in the request body."))
                    .ToHttpResponseMessage(Request);
            }

            return (await _groupService.UpdateGroup(group))
                .ToHttpResponseMessage(Request, (groupDto, response) =>
                {
                    SetLocationHeader(response, groupDto, "RetrieveGroup", new {groupId = groupDto.Id});
                    SetETagHeader(response, groupDto);
                });
        }

        [Route("groups/{groupId}", Name = "DeleteGroup")]
        public async Task<HttpResponseMessage> Delete(string groupId)
        {
            return (await _groupService.DeleteGroup(groupId)).ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}