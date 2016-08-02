namespace Owin.Scim.v2.Endpoints
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Model;

    using NContext.Extensions;

    using Newtonsoft.Json.Serialization;

    using Patching;

    using Querying;

    using Scim.Endpoints;
    using Scim.Model;
    using Scim.Model.Groups;
    using Scim.Services;
    
    [RoutePrefix(ScimConstantsV2.Endpoints.Groups)]
    public class GroupsController : ScimControllerBase
    {
        public const string RetrieveGroupRouteName = @"RetrieveGroup2";

        private readonly IGroupService _GroupService;

        public GroupsController(
            ScimServerConfiguration serverConfiguration,
            IGroupService groupService) 
            : base(serverConfiguration)
        {
            _GroupService = groupService;
        }

        [Route]
        public async Task<HttpResponseMessage> Post(ScimGroup groupDto)
        {
            return (await _GroupService.CreateGroup(groupDto))
                .Let(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id }))
                .ToHttpResponseMessage(Request, (group, response) =>
                {
                    response.StatusCode = HttpStatusCode.Created;

                    SetContentLocationHeader(response, RetrieveGroupRouteName, new { groupId = group.Id });
                    SetETagHeader(response, group);
                });
        }

        [Route("{groupId}", Name = RetrieveGroupRouteName)]
        public async Task<HttpResponseMessage> Get(string groupId)
        {
            return (await _GroupService.RetrieveGroup(groupId))
                .Let(group => SetMetaLocation(group, RetrieveGroupRouteName, new {groupId = group.Id}))
                .Let(group =>
                    group.Members?.ForEach(m =>
                        m.Ref = m.Type == ScimConstants.ResourceTypes.User
                            ? GetUserUri(UsersController.RetrieveUserRouteName, m.Value)
                            : GetGroupUri(RetrieveGroupRouteName, m.Value)))
                .ToHttpResponseMessage(Request, (group, response) =>
                {
                    SetContentLocationHeader(response, RetrieveGroupRouteName, new {groupId = group.Id});
                    SetETagHeader(response, group);
                });
        }

        [AcceptVerbs("GET")]
        [Route]
        public Task<HttpResponseMessage> GetQuery(ScimQueryOptions queryOptions)
        {
            return Query(queryOptions);
        }

        [AcceptVerbs("POST")]
        [Route(".search")]
        public Task<HttpResponseMessage> PostQuery(ScimQueryOptions queryOptions)
        {
            return Query(queryOptions);
        }

        [NonAction]
        private async Task<HttpResponseMessage> Query(ScimQueryOptions options)
        {
            return (await _GroupService.QueryGroups(options))
                .Let(groups => groups.ForEach(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id })))
                .Let(groups => groups.ForEach(group =>
                {
                    // needed to materialize ienumerable, otherwise it did not work
                    var members = group.Members?.ToList();
                    members?.ForEach(m =>
                        m.Ref = m.Type == ScimConstants.ResourceTypes.User
                            ? GetUserUri(UsersController.RetrieveUserRouteName, m.Value)
                            : GetGroupUri(RetrieveGroupRouteName, m.Value));
                    group.Members = members;
                }))
                .Bind(
                    groups =>
                    new ScimDataResponse<ScimListResponse>(
                        new ScimListResponse2(groups)
                        {
                            StartIndex = options.StartIndex,
                            ItemsPerPage = options.Count
                        }))
                .ToHttpResponseMessage(Request);
        }

        [Route("{groupId}")]
        public async Task<HttpResponseMessage> Patch(string groupId, PatchRequest<ScimGroup> patchRequest)
        {
            if (patchRequest == null ||
                patchRequest.Operations == null ||
                patchRequest.Operations.Operations.Any(a => a.OperationType == Patching.Operations.OperationType.Invalid))
            {
                return new ScimErrorResponse<ScimGroup>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidSyntax,
                        "The patch request body is un-parsable, syntactically incorrect, or violates schema."))
                    .ToHttpResponseMessage(Request);
            }

            return (await (await _GroupService.RetrieveGroupForUpdate(groupId))
                .Bind(group =>
                {
                    try
                    {
                        // TODO: (DG) Finish patch support
                        var result = patchRequest.Operations.ApplyTo(
                            @group, 
                            new ScimObjectAdapter<ScimGroup>(ServerConfiguration, new CamelCasePropertyNamesContractResolver()));
                        
                        return (IScimResponse<ScimGroup>)new ScimDataResponse<ScimGroup>(@group);
                    }
                    catch (Patching.Exceptions.ScimPatchException ex)
                    {
                        return (IScimResponse<ScimGroup>)new ScimErrorResponse<ScimGroup>(ex.ToScimError());
                    }
                })
                .BindAsync(group => _GroupService.UpdateGroup(group)))
                .Let(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id }))
                .ToHttpResponseMessage(Request, (group, response) =>
                {
                    SetContentLocationHeader(response, RetrieveGroupRouteName, new { groupId = group.Id });
                    SetETagHeader(response, group);
                });
        }

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("{groupId}")]
        public async Task<HttpResponseMessage> Put(string groupId, ScimGroup groupDto)
        {
            groupDto.Id = groupId;

            return (await _GroupService.UpdateGroup(groupDto))
                .Let(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id }))
                .ToHttpResponseMessage(Request, (group, response) =>
                {
                    SetContentLocationHeader(response, RetrieveGroupRouteName, new { groupId = group.Id });
                    SetETagHeader(response, group);
                });
        }

        [Route("{groupId}")]
        public async Task<HttpResponseMessage> Delete(string groupId)
        {
            return (await _GroupService.DeleteGroup(groupId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}