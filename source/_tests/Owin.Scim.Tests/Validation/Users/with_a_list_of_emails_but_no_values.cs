namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_a_list_of_emails_but_no_values : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new User
            {
                UserName = "daniel",
                Emails = new List<Email>
                {
                    new Email(),
                    new Email()
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}