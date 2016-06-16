namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;

    using Machine.Specifications;
    
    using v2.Model;

    public class with_an_enterprise_user : when_creating_a_user
    {
        Establish context = () =>
        {
            UserDto = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName()
            };
            
            UserDto.Extension<EnterpriseUser2Extension>().Department = "Sales";
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_the_user = () => CreatedUser.Id.ShouldNotBeEmpty();
    }
}