namespace Owin.Scim.Endpoints.Users
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Model.Users;

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
            var result = await _UserService.CreateUser(user);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return Request.CreateResponse(HttpStatusCode.Created, result);
        }

        [Route("users/{userId}", Name = "RetrieveUser")]
        public async Task<HttpResponseMessage> Get(string userId)
        {
            var user = await _UserService.RetrieveUser(userId);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        [Route("users/{userId}", Name = "UpdateUser")]
        public async Task<HttpResponseMessage> Patch(string userId)//, PatchRequest<User> patch)
        {
            throw new NotImplementedException();
        }

        [Route("users/{userId}", Name = "ReplaceUser")]
        public async Task<HttpResponseMessage> Put(string userId, User user)
        {
            if (String.IsNullOrWhiteSpace(userId) || 
                user == null ||
                string.IsNullOrWhiteSpace(user.Id) ||
                !user.Id.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var result = await _UserService.UpdateUser(user);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("users/{userId}", Name = "DeleteUser")]
        public async Task<HttpResponseMessage> Delete(string userId)
        {
            var result = await _UserService.DeleteUser(userId);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}