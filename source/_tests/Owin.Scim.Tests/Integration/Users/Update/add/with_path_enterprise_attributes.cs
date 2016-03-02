namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_enterprise_attributes : when_updating_enterprise_user
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
            UserToUpdate.Extension<EnterpriseUserExtension>().Department = "Hello";

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp"",
                                      ""urn:ietf:params:scim:schemas:extension:enterprise:2.0:User""],
                        ""Operations"": [{
                            ""op"": ""add"",
                            ""path"": ""urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department"",
                            ""value"": ""1234""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_replace_employee_number = () => UpdatedUser.Extension<EnterpriseUserExtension>().Department.ShouldEqual("1234");
    }
}