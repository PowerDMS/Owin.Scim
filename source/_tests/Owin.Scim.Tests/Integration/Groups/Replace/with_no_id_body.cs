namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    public class with_no_id_in_body : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingGroup = CreateGroup(new Group {DisplayName = "existing group"});

            GroupId = ExistingGroup.Id;

            GroupDto = new Group
            {
                DisplayName = "new group",
                ExternalId = "hello",
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_syntax = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidSyntax);

        It should_indicate_missing_attribute = () => Error.Detail.ShouldContain("id");
 
        private static Group ExistingGroup;
    }
}