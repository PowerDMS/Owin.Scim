namespace Owin.Scim.Endpoints.Users
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Extensions;

    using Model;
    using Model.Users;

    using Newtonsoft.Json.Serialization;

    using Patching;
    using Patching.Exceptions;

    using Services;

    public class UsersController : ApiController
    {
        private readonly IUserService _UserService;

        public UsersController(IUserService userService)
        {
            _UserService = userService;
        }

        [Route("users", Name = "CreateUser")]
        public async Task<HttpResponseMessage> Post(User user)
        {
            return (await _UserService.CreateUser(user))
                .ToHttpResponseMessage(Request, HttpStatusCode.Created);
        }

        [Route("users/{userId}", Name = "RetrieveUser")]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            return (await _UserService.RetrieveUser(userId))
                .ToHttpResponseMessage(Request);
        }
        
        [Route("users/{userId}", Name = "UpdateUser")]
        public async Task<HttpResponseMessage> Patch(string userId, PatchRequest<User> patchRequest)
        {
            if (patchRequest == null || patchRequest.Operations == null)
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
                .ToHttpResponseMessage(Request);
        }

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("users/{userId}", Name = "ReplaceUser")]
        public async Task<HttpResponseMessage> Put(string userId, User user)
        {
            if (String.IsNullOrWhiteSpace(userId) ||
                user == null ||
                string.IsNullOrWhiteSpace(user.Id) ||
                !user.Id.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                return new ScimErrorResponse<User>(
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        detail: "The request path 'userId' MUST match the user.id in the request body."))
                    .ToHttpResponseMessage(Request);
            }

            return (await _UserService.UpdateUser(user))
                .ToHttpResponseMessage(Request);
        }

        [Route("users/{userId}", Name = "DeleteUser")]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            return (await _UserService.DeleteUser(userId))
                .ToHttpResponseMessage(Request, HttpStatusCode.NoContent);
        }
    }
}