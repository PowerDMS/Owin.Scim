namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_list_of_address_with_invalid_country_codes : when_validating_a_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(true);

            User = new User
            {
                UserName = "daniel",
                Addresses = new List<Address>
                {
                    new Address { Country = "USA" },
                    new Address { Country = "invalid" }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}