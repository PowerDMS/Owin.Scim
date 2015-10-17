namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_empty_list_of_addresses : when_validating_a_user
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
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}