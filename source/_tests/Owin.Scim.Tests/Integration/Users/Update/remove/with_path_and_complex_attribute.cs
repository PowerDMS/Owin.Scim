namespace Owin.Scim.Tests.Integration.Users.Update.remove
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_path_and_complex_attribute : when_updating_a_user
    {
        static with_path_and_complex_attribute()
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
        }

        Establish context = () =>
        {
            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""remove"",
                            ""path"": ""name.givenName""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_remove_other_attributes = () => UpdatedUser.Name.FamilyName.ShouldEqual("Smith");

        It should_remove_the_attribute_value = () => UpdatedUser.Name.GivenName.ShouldBeNull();
    }
}