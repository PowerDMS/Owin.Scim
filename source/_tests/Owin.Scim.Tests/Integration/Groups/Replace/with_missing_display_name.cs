namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    using v2.Model;

    public class with_missing_display_name : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingGroup = CreateGroup(new ScimGroup2 {DisplayName = "existing group"});

            GroupId = ExistingGroup.Id;

            GroupDto = new ScimGroup2
            {
                Id = GroupId,
                ExternalId = "hello",
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidValue);

        It should_return_indicate_missing_attribute = () => Error.Detail.ShouldContain("displayName");

        private static ScimGroup ExistingGroup;
    }
}