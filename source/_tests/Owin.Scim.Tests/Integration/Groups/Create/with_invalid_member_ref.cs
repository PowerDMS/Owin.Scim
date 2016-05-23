namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Net;
    using Machine.Specifications;
    using Model.Users;
    using Model.Groups;

    public class with_invalid_member_ref : when_creating_a_group
    {
        Establish context = () =>
        {
            ExistingUser = CreateUser(new ScimUser { UserName = Users.UserNameUtility.GenerateUserName() });

            GroupDto = new ScimGroup
            {
                DisplayName = "hello",
                ExternalId = "hello",
                Members = new []
                {
                    new Member { Ref = new Uri("\\\\hello") }
                }
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_syntax = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidSyntax);

        It should_return_indicate_invalid_attribute = () => Error.Detail.ShouldContain("member.$ref");

        private static ScimUser ExistingUser;
    }
}