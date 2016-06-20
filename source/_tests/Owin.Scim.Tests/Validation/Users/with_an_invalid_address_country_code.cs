namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_an_invalid_address_country_code : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser2
            {
                UserName = "daniel",
                Addresses = new List<MailingAddress>
                {
                    new MailingAddress { Country = "invalid" }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}