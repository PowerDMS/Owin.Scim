namespace Owin.Scim.Tests.Integration.Groups
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class using_existing_user_and_group : using_a_scim_server
    {
        protected static User CreateUser(User user)
        {
            var response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(user, new ScimJsonMediaTypeFormatter())).Result;

            return (response.StatusCode == HttpStatusCode.Created)
                ? response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result
                : null;
        }

        protected static Group CreateGroup(Group group)
        {
            var response = Server
                .HttpClient
                .PostAsync("groups", new ObjectContent<Group>(group, new ScimJsonMediaTypeFormatter())).Result;

            return response.StatusCode == HttpStatusCode.Created
                ? response.Content.ReadAsAsync<Group>(ScimJsonMediaTypeFormatter.AsArray()).Result
                : null;
        }
    }
}