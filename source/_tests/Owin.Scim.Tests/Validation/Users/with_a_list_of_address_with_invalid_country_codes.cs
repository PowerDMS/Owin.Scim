namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_list_of_address_with_invalid_country_codes : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel",
                Addresses = new List<MailingAddress>
                {
                    new MailingAddress { Country = "USA" },
                    new MailingAddress { Country = "invalid" }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}