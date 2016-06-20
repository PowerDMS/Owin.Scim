namespace Owin.Scim.v1.Endpoints
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Model;

    using NContext.Extensions;

    using Querying;

    using Scim.Endpoints;
    using Scim.Services;

    using v1;

    [RoutePrefix(ScimConstantsV1.Endpoints.Users)]
    public class UsersController : ScimControllerBase
    {
        public const string RetrieveUserRouteName = @"RetrieveUser1";

        private readonly IUserService _UserService;

        public UsersController(
            ScimServerConfiguration serverConfiguration,
            IUserService userService)
            : base(serverConfiguration)
        {
            _UserService = userService;
        }

        [Route]
        public async Task<HttpResponseMessage> Post(ScimUser1 userDto)
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

        [NonAction]
        private async Task<HttpResponseMessage> Query(ScimQueryOptions options)
        {
            return (await _UserService.QueryUsers(options))
                .Let(users => users.ForEach(user => SetMetaLocation(user, RetrieveUserRouteName, new { userId = user.Id })))
                .Bind(
                    users => 
                    new ScimDataResponse<ScimListResponse>(
                        new ScimListResponse1(users)
                        {
                            StartIndex = options.StartIndex,
                            ItemsPerPage = options.Count
                        }))
                .ToHttpResponseMessage(Request);
        }

        [AcceptVerbs("PUT", "OPTIONS")]
        [Route("{userId}")]
        public async Task<HttpResponseMessage> Put(string userId, ScimUser1 userDto)
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