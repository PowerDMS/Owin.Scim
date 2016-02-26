namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_and_full_complex_attribute : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
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
                            ""op"": ""add"",
                            ""path"": ""name"",
                            ""value"":{
                                ""honorificPrefix"":""Dr"",
                                ""givenName"":""Daniel""
                            }
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_replace_complex_attribute = () =>
        {
            UpdatedUser.Name.HonorificPrefix.ShouldEqual("Dr");
            UpdatedUser.Name.GivenName.ShouldEqual("Daniel");
            UpdatedUser.Name.FamilyName.ShouldBeNull();
        };
    }
}