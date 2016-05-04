namespace Owin.Scim.Endpoints
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Model;
    using Model.Groups;

    using NContext.Extensions;

    using Newtonsoft.Json.Serialization;

    using Patching;

    using Querying;

    using Services;

    [RoutePrefix(ScimConstants.Endpoints.Groups)]
    public class GroupsController : ScimControllerBase
    {
        public const string RetrieveGroupRouteName = @"RetrieveGroup";

        private readonly IGroupService _GroupService;

        public GroupsController(
            ScimServerConfiguration serverConfiguration,
            IGroupService groupService) 
            : base(serverConfiguration)
        {
            _GroupService = groupService;
        }

        [Route(Name = "CreateGroup")]
        public async Task<HttpResponseMessage> Post(Group groupDto)
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
                .Let(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id }))
                .ToHttpResponseMessage(Request, (group, response) =>
                {
                    SetContentLocationHeader(response, RetrieveGroupRouteName, new { groupId = group.Id });
                    SetETagHeader(response, group);
                });
        }


        [AcceptVerbs("GET")]
        [Route(Name = "GetQueryGroups")]
        public Task<HttpResponseMessage> GetQuery(ScimQueryOptions queryOptions)
        {
            return Query(queryOptions);
        }

        [AcceptVerbs("POST")]
        [Route(".search", Name = "PostQueryGroups")]
        public Task<HttpResponseMessage> PostQuery(ScimQueryOptions queryOptions)
        {
            return Query(queryOptions);
        }

        [NonAction]
        private async Task<HttpResponseMessage> Query(ScimQueryOptions options)
        {
            return (await _GroupService.QueryGroups(options))
                .Let(groups => groups.ForEach(group => SetMetaLocation(group, RetrieveGroupRouteName, new { groupId = group.Id })))
                .Bind(
                    groups =>
                    new ScimDataResponse<ScimListResponse>(
                        new ScimListResponse(groups)
                        {
                            StartIndex = options.StartIndex,
                            ItemsPerPage = options.Count
                        }))
                .ToHttpResponseMessage(Request);
        }

        [Route("{groupId}", Name = "UpdateGroup")]
        public async Task<HttpResponseMessage> Patch(string groupId, PatchRequest<Group> patchRequest)
        {
            if (patchRequest?.Operations == null ||
                patchRequest.Operations.Operations.Any(a => a.OperationType == Patching.Operations.OperationType.Invalid))
            {
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidSyntax,
                        "The patch request body is un-parsable, syntactically incorrect, or violates schema."))
                    .ToHttpResponseMessage(Request);
            }

            return (await (await _GroupService.RetrieveGroup(groupId))
                .Bind(group =>
                {
                    try
                    {
                        // TODO: (DG) Finish patch support
                        var result = patchRequest.Operations.ApplyTo(
                            @group, 
                            new ScimObjectAdapter<Group>(ServerConfiguration, new CamelCasePropertyNamesContractResolver()));
                        
                        return (IScimResponse<Group>)new ScimDataResponse<Group>(@group);
                    }
                    catch (Patching.Exceptions.ScimPatchException ex)
                    {
                        return (IScimResponse<Group>)new ScimErrorResponse<Group>(ex.ToScimError());
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
        [Route("{groupId}", Name = "ReplaceGroup")]
        public async Task<HttpResponseMessage> Put(string groupId, Group groupDto)
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

        [Route("{groupId}", Name = "DeleteGroup")]
        public async Task<HttpResponseMessage> Delete(string groupId)
        {
            return (await _GroupService.DeleteGroup(groupId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}