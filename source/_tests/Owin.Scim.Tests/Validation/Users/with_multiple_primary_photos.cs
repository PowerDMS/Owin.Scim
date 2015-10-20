namespace Owin.Scim.Tests.Validation.Users
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    public class with_multiple_primary_photos : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new User
            {
                UserName = "daniel",
                Photos = new List<Photo>
                {
                    new Photo { Value = "http://example.com/scim/2/users/me.jpg", Primary = true },
                    new Photo { Value = "http://example.com/scim/2/users/me2.jpg", Primary = true }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}