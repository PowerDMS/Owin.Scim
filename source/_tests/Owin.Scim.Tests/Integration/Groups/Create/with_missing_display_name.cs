namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    public class with_missing_display_name : when_creating_a_group
    {
        Establish context = () =>
        {
            GroupDto = new Group
            {
                ExternalId = "hello",
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidValue);

        It should_return_indicate_missing_attribute = () => Error.Detail.ShouldContain("displayName");
    }
}