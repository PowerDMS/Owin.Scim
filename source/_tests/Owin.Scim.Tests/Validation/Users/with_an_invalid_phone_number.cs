namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;
    
    using Machine.Specifications;

    using Model.Users;

    public class with_an_invalid_phone_number : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber { Value = "tel:+64-3-331-" }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}