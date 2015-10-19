namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_valid_emails : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new User
            {
                UserName = "daniel",
                Emails = new List<Email>
                {
                    new Email { Value = "valid@gmail.com" },
                    new Email { Value = "anothervalid@gmail.co.uk" }
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}