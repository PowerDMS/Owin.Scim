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
            UserToUpdate = new EnterpriseUser
            {
                UserName = UserNameUtility.GenerateUserName(),
                Name = new Name
                {
                    FamilyName = "Smith",
                    GivenName = "John"
                },
                Enterprise = new EnterpriseUserExtension
                {
                    Department = "Hello"
                }
            };

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

        It should_replace_employee_number = () => UpdatedUser.Enterprise.Department.ShouldEqual("1234");
    }
}