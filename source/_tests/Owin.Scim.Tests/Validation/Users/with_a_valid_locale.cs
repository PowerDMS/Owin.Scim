namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_locale : when_validating_a_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(true);

            User = new User
            {
                UserName = "daniel",
                Locale = "en-US"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
    public class with_an_invalid_phone_number : when_validating_a_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(true);

            User = new User
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