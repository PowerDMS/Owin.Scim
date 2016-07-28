namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Groups;
    using Model.Users;

    using Ploeh.AutoFixture;

    using v2.Model;

    public class when_retrieving_user_with_group : using_a_scim_server
    {
        Because of = () =>
        {
            var autoFixture = new Fixture();

            // Insert the first user so there's one already in-memory.
            var createUserResponse = Server
                .HttpClient
                .PostAsync("v2/users",
                    new ScimObjectContent<ScimUser>(
                        new ScimUser2 {UserName = autoFixture.Create("UserName")}))
                .Result;

            if (createUserResponse.StatusCode != HttpStatusCode.Created)
                return;

            CreatedUser = createUserResponse.Content.ScimReadAsAsync<ScimUser2>().Result;

            // create a group with this user
            var createGroupResponse = Server
                .HttpClient
                .PostAsync("v2/groups",
                    new ScimObjectContent<ScimGroup>(
                        new ScimGroup2
                        {
                            DisplayName = autoFixture.Create("DisplayName"),
                            Members = new[] {new Member {Value = CreatedUser.Id, Type = CreatedUser.Meta.ResourceType}}
                        }))
                .Result;

            if (createGroupResponse.StatusCode != HttpStatusCode.Created)
                return;

            CreatedGroup = createGroupResponse.Content.ScimReadAsAsync<ScimGroup2>().Result;

            Response = Server
                .HttpClient
                .GetAsync("v2/users/" + CreatedUser.Id)
                .Result;
        };

        It should_return_canonized_member = 
            () =>
        {
            Response.ShouldNotBeNull();
            Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            RetrievedUser = Response.Content.ScimReadAsAsync<ScimUser2>().Result;

            RetrievedUser.Groups.ShouldNotBeNull();

            var groups = RetrievedUser.Groups.ToList();
            groups.Count.ShouldEqual(1);
            groups[0].Display.ShouldEqual(CreatedGroup.DisplayName);
            groups[0].Value.ShouldEqual(CreatedGroup.Id);
            groups[0].Type.ShouldEqual("direct");
            groups[0].Ref.ShouldEqual(CreatedGroup.Meta.Location);
        };

        protected static HttpResponseMessage Response;

        protected static ScimUser CreatedUser;

        protected static ScimGroup CreatedGroup;
        
        protected static ScimUser RetrievedUser;
    }
}