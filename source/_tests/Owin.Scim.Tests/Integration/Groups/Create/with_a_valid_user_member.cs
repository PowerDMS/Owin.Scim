using System.Linq;

namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_a_valid_user_member : when_creating_a_group
    {
        Establish context = () =>
        {
            TestStartTime = DateTime.Now;

            Response = Server
                .HttpClient
                .PostAsync("users",
                    new ObjectContent<User>(
                        new User {UserName = Users.UserNameUtility.GenerateUserName()},
                        new ScimJsonMediaTypeFormatter())).Result;

            ExistingUser = Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result;

            GroupDto = new Group
            {
                DisplayName = "hello",
                ExternalId = "hello",
                Members = new []{new Member {Value = ExistingUser.Id, Type = "user"}, }
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_contain_id = () => CreatedGroup.Id.ShouldNotBeEmpty();

        It should_contain_meta = () =>
        {
            CreatedGroup.Meta.ShouldNotBeNull();
            CreatedGroup.Meta.Created.ShouldBeGreaterThan(TestStartTime);
            CreatedGroup.Meta.LastModified.ShouldEqual(CreatedGroup.Meta.Created);
            CreatedGroup.Meta.Location.ShouldNotBeNull();
            CreatedGroup.Meta.Location.ShouldEqual(Response.Headers.Location);
            CreatedGroup.Meta.Version.ShouldNotBeNull();
            CreatedGroup.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
        };

        It should_contain_new_member = () => CreatedGroup.Members.First().Value.ShouldEqual(ExistingUser.Id);

        It should_canonize_member_type = () => CreatedGroup.Members.First().Type.ShouldEqual("User");

        private static User ExistingUser;
        private static DateTime TestStartTime;
    }
}