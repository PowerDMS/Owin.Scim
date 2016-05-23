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
    using Model.Users;

    using NContext.Extensions;

    using Newtonsoft.Json.Serialization;

    using Patching;
    using Patching.Exceptions;

    using Querying;

    using Services;
    
    [RoutePrefix(ScimConstants.Endpoints.Users)]
    public class UsersController : ScimControllerBase
    {
        public const string RetrieveUserRouteName = @"RetrieveUser";

        private readonly IUserService _UserService;

        public UsersController(
            ScimServerConfiguration serverConfiguration,
            IUserService userService)
            : base(serverConfiguration)
        {
            _UserService = userService;
        }

        [Route(Name = "CreateUser")]
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
                });
        }

        [AcceptVerbs("GET")]
        [Route(Name = "GetQueryUsers")]
        public Task<HttpResponseMessage> GetQuery(ScimQueryOptions queryOptions)
        {
            return Query(queryOptions);
        }

        [AcceptVerbs("POST")]
        [Route(".search", Name = "PostQueryUsers")]
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
                        new ScimListResponse(users)
                        {
                            StartIndex = options.StartIndex,
                            ItemsPerPage = options.Count
                        }))
                .ToHttpResponseMessage(Request);
        }

        [Route("{userId}", Name = "UpdateUser")]
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
                            new ScimObjectAdapter<ScimUser>(ServerConfiguration, new CamelCasePropertyNamesContractResolver()));

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
        [Route("{userId}", Name = "ReplaceUser")]
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

        [Route("{userId}", Name = "DeleteUser")]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            return (await _UserService.DeleteUser(userId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}