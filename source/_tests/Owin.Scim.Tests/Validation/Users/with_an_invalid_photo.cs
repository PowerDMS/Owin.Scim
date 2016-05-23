namespace Owin.Scim.Tests.Validation.Users
{
    using System;
    using System.Collections.Generic;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_invalid_photo : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel",
                Photos = new List<Photo>
                {
                    new Photo { Value = new Uri("invalidRelativeUri", UriKind.Relative) }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}