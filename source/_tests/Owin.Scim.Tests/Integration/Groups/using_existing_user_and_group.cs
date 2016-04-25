namespace Owin.Scim.Tests.Integration.Groups
{
    using System.Net;

    using Model.Users;
    using Model.Groups;

    public class using_existing_user_and_group : using_a_scim_server
    {
        protected static User CreateUser(User user)
        {
            var response = Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<User>(user)).Result;

            return (response.StatusCode == HttpStatusCode.Created)
                ? response.Content.ScimReadAsAsync<User>().Result
                : null;
        }

        protected static Group CreateGroup(Group group)
        {
            var response = Server
                .HttpClient
                .PostAsync("groups", new ScimObjectContent<Group>(group)).Result;

            return response.StatusCode == HttpStatusCode.Created
                ? response.Content.ScimReadAsAsync<Group>().Result
                : null;
        }
    }
}