namespace Owin.Scim.Tests.Integration.Groups.Retrieve
{
    using System.Net;
    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_a_valid_user_member : when_retrieving_a_group
    {
        /// <summary>
        /// Must have a pre-existing group with members in it
        /// </summary>
        private Establish context = () =>
        {
            ExistingUser = CreateUser(new ScimUser {UserName = Users.UserNameUtility.GenerateUserName()});

            GroupDto = new ScimGroup
            {
                DisplayName = Users.UserNameUtility.GenerateUserName(),
                Members = new[] {new Member {Value = ExistingUser.Id, Type = "user"},}
            };

            ExistingGroup = CreateGroup(GroupDto);

            GroupId = ExistingGroup.Id;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_return_group_data = () => RetrievedGroup.ShouldBeLike(ExistingGroup);

        It should_return_header_location = () =>
        {
            RetrievedGroup.Meta.Location.ShouldEqual(Response.Headers.Location);
        };

        It should_return_header_etag = () =>
        {
            RetrievedGroup.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
        };

        private static ScimUser ExistingUser;
        private static ScimGroup ExistingGroup;
    }
}