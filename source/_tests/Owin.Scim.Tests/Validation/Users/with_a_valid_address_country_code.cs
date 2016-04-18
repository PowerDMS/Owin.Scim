namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_address_country_code : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new User
            {
                UserName = "daniel",
                Addresses = new List<MailingAddress>
                {
                    new MailingAddress { Country = "US" }
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}