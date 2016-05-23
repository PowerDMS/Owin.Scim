namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_an_empty_list_of_addresses : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel",
                Addresses = new List<MailingAddress>
                {
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}