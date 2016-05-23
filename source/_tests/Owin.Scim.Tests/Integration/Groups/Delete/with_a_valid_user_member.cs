namespace Owin.Scim.Tests.Integration.Groups.Delete
{
    using System.Net;
    using Machine.Specifications;
    using Model.Groups;

    public class with_a_valid_user_member : when_deleting_a_group
    {
        /// <summary>
        /// Must have a pre-existing group with members in it
        /// </summary>
        private Establish context = () =>
        {
            GroupDto = new ScimGroup
            {
                DisplayName = Users.UserNameUtility.GenerateUserName(),
            };

            ExistingGroup = CreateGroup(new ScimGroup {DisplayName = Users.UserNameUtility.GenerateUserName()});

            GroupId = ExistingGroup.Id;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);

        private static ScimGroup ExistingGroup;
    }
}