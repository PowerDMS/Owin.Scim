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

    public class UsersController : ScimControllerBase
    {
        private readonly IUserService _UserService;

        public UsersController(
            ScimServerConfiguration scimServerConfiguration,
            IUserService userService)
            : base(scimServerConfiguration)
        {
            _UserService = userService;
        }

        [Route("users", Name = "CreateUser")]
        public async Task<HttpResponseMessage> Post(User user)
        {
            return (await _UserService.CreateUser(user))
                .ToHttpResponseMessage(Request, (userDto, response) =>
                {
                    response.StatusCode = HttpStatusCode.Created;

                    SetLocationHeader(response, userDto, "RetrieveUser", new { userId = userDto.Id });
                    SetETagHeader(response, userDto);
                });
        }
        
        [Route("users/{userId}", Name = "RetrieveUser")]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            return (await _UserService.RetrieveUser(userId))
                .ToHttpResponseMessage(Request, (userDto, response) =>
                {
                    SetLocationHeader(response, userDto, "RetrieveUser", new { userId = userDto.Id });
                    SetETagHeader(response, userDto);
                });
        }
        
        [Route("users/{userId}", Name = "UpdateUser")]
        public async Task<HttpResponseMessage> Patch(string userId, PatchRequest<User> patchRequest)
        {
            if (patchRequest?.Operations == null || 
                patchRequest.Operations.Operations.Any(a => a.OperationType == Patching.Operations.OperationType.Invalid))
            {
                return new ScimErrorResponse<User>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidSyntax,
                        "The patch request body is unparsable, syntactically incorrect, or violates schema."))
                    .ToHttpResponseMessage(Request);
            }

            return (await (await _UserService.RetrieveUser(userId))
                .Bind<User, User>(user =>
                {
                    try
                    {
                        // TODO: (DG) Finish patch support
                        var result = patchRequest.Operations.ApplyTo(
                            user,
                            new ScimObjectAdapter<User>(
                                new CamelCasePropertyNamesContractResolver()));

                        return new ScimDataResponse<User>(user);
                    }
                    catch (ScimPatchException ex)
                    {
                        return new ScimErrorResponse<User>(ex.ToScimError());
                    }
                })
                .BindAsync(user => _UserService.UpdateUser(user)))
                .ToHttpResponseMessage(Request, (userDto, response) =>
                {
                    SetLocationHeader(response, userDto, "RetrieveUser", new { userId = userDto.Id });
                    SetETagHeader(response, userDto);
                });
        }

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("users/{userId}", Name = "ReplaceUser")]
        public async Task<HttpResponseMessage> Put(string userId, User user)
        {
            user.Id = userId;

            return (await _UserService.UpdateUser(user))
                .ToHttpResponseMessage(Request, (userDto, response) =>
                {
                    SetLocationHeader(response, userDto, "RetrieveUser", new { userId = userDto.Id });
                    SetETagHeader(response, userDto);
                });
        }

        [Route("users/{userId}", Name = "DeleteUser")]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            return (await _UserService.DeleteUser(userId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}