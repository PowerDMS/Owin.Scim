namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_direct_subattribute_no_change : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                Name = new Name
                {
                    FamilyName = "Smith",
                    GivenName = "John"
                }
            };

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""replace"",
                            ""path"": ""name.givenName"",
                            ""value"": ""John""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_change_complex_attribute = () => UpdatedUser.Name.ShouldBeLike(UserToUpdate.Name);

        It should_not_update_version = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_not_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);
    }
}