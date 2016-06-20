namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_enterprise_attributes : when_updating_a_user
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

            UserToUpdate.Extension<EnterpriseUser2Extension>().Department = "Hello";

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp"",
                                      ""urn:ietf:params:scim:schemas:extension:enterprise:2.0:User""],
                        ""Operations"": [{
                            ""op"": ""add"",
                            ""path"": ""urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department"",
                            ""value"": ""1234""
                        },
                        {
                            ""op"": ""add"",
                            ""path"": ""urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:manager.value"",
                            ""value"": ""42""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_add_department = () => UpdatedUser.Extension<EnterpriseUser2Extension>().Department.ShouldEqual("1234");

        It should_add_manager_value = () => UpdatedUser.Extension<EnterpriseUser2Extension>().Manager.Value.ShouldEqual("42");
    }
}