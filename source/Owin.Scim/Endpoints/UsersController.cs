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

    using Newtonsoft.Json.Serialization;

    using Patching;
    using Patching.Exceptions;

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
        public async Task<HttpResponseMessage> Post(User userDto)
        {
            return (await _UserService.CreateUser(userDto))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (user, response) =>
                {
                    response.StatusCode = HttpStatusCode.Created;

                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = user.Id });
                    SetETagHeader(response, userDto);
                });
        }
        
        [Route("{userId}", Name = RetrieveUserRouteName)]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            return (await _UserService.RetrieveUser(userId))
                .Let(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id }))
                .ToHttpResponseMessage(Request, (userDto, response) =>
                {
                    SetContentLocationHeader(response, RetrieveUserRouteName, new { userId = userDto.Id });
                    SetETagHeader(response, userDto);
                });
        }
        
        [Route("{userId}", Name = "UpdateUser")]
        public async Task<HttpResponseMessage> Patch(string userId, PatchRequest<User> patchRequest)
        {
            if (patchRequest?.Operations == null || 
                patchRequest.Operations.Operations.Any(a => a.OperationType == Patching.Operations.OperationType.Invalid))
            {
                return new ScimErrorResponse<User>(
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
                            new ScimObjectAdapter<User>(ServerConfiguration, new CamelCasePropertyNamesContractResolver()));

                        return (IScimResponse<User>)new ScimDataResponse<User>(user);
                    }
                    catch (ScimPatchException ex)
                    {
                        return (IScimResponse<User>)new ScimErrorResponse<User>(ex.ToScimError());
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
        public async Task<HttpResponseMessage> Put(string userId, User userDto)
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