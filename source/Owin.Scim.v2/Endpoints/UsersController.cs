namespace Owin.Scim.v2.Endpoints
{
    using System;
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
    using Patching.Exceptions;

    using Querying;

    using Scim.Endpoints;
    using Scim.Model;
    using Scim.Model.Users;
    using Scim.Services;

    [RoutePrefix(ScimConstantsV2.Endpoints.Users)]
    public class UsersController : ScimControllerBase
    {
        public const string RetrieveUserRouteName = @"RetrieveUser2";

        private readonly IUserService _UserService;

        public UsersController(
            ScimServerConfiguration serverConfiguration,
            IUserService userService)
            : base(serverConfiguration)
        {
            _UserService = userService;
        }

        [Route]
        public async Task<HttpResponseMessage> Post(ScimUser userDto)
        {
            return (await _UserService.CreateUser(userDto))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (user, response) =>
                {
                    response.StatusCode = HttpStatusCode.Created;

                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = user.Id });
                    SetETagHeader(response, user);
                });
        }
        
        [Route("{userId}", Name = RetrieveUserRouteName)]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            return (await _UserService.RetrieveUser(userId))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (user, response) =>
                {
                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = user.Id });
                    SetETagHeader(response, user);

                    if (user.Groups != null)
                    {
                        foreach (var userGroup in user.Groups)
                        {
                            userGroup.Ref = new Uri(
                                Request
                                    .GetUrlHelper()
                                    .Link(GroupsController.RetrieveGroupRouteName, new { groupId = userGroup.Value }));
                        }
                    }
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
            return (await _UserService.QueryUsers(options))
                .Let(users => users.ForEach(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id })))
                .Bind(
                    users => 
                    new ScimDataResponse<ScimListResponse>(
                        new ScimListResponse2(users)
                        {
                            StartIndex = options.StartIndex,
                            ItemsPerPage = options.Count
                        }))
                .ToHttpResponseMessage(Request);
        }

        [Route("{userId}")]
        public async Task<HttpResponseMessage> Patch(string userId, PatchRequest<ScimUser> patchRequest)
        {
            if (patchRequest == null ||
                patchRequest.Operations == null || 
                patchRequest.Operations.Operations.Any(a => a.OperationType == Patching.Operations.OperationType.Invalid))
            {
                return new ScimErrorResponse<ScimUser>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidSyntax,
                        "The patch request body is un-parsable, syntactically incorrect, or violates schema."))
                    .ToHttpResponseMessage(Request);
            }

            return (await (await _UserService.RetrieveUser(userId))
                .Bind(user =>
                {
                    try
                    {
                        // TODO: (DG) Finish patch support
                        var result = patchRequest.Operations.ApplyTo(
                            user, 
                            new ScimObjectAdapter<ScimUser2>(ServerConfiguration, new CamelCasePropertyNamesContractResolver()));

                        return (IScimResponse<ScimUser>)new ScimDataResponse<ScimUser>(user);
                    }
                    catch (ScimPatchException ex)
                    {
                        return (IScimResponse<ScimUser>)new ScimErrorResponse<ScimUser>(ex.ToScimError());
                    }
                })
                .BindAsync(user => _UserService.UpdateUser(user)))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (user, response) =>
                {
                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = user.Id });
                    SetETagHeader(response, user);
                });
        }

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("{userId}")]
        public async Task<HttpResponseMessage> Put(string userId, ScimUser userDto)
        {
            userDto.Id = userId;

            return (await _UserService.UpdateUser(userDto))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (user, response) =>
                {
                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = user.Id });
                    SetETagHeader(response, user);
                });
        }

        [Route("{userId}")]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            return (await _UserService.DeleteUser(userId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}