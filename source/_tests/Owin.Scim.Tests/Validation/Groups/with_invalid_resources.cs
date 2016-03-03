namespace Owin.Scim.Tests.Validation.Groups
{
    using System;
    using System.Linq;

    using Machine.Specifications;

    using Model.Groups;

    /// <summary>
    /// members must provide $ref or a value/type combo
    /// </summary>
    public class with_invalid_resources : when_creating_a_group
    {
        Establish ctx = () =>
        {
            Group = new Group
            {
                DisplayName = "blue man",
                Members = new []
                {
                    new Member {Ref = new Uri(@"http://local/Scim/V2/users/invalidOne") },
                    new Member {Value = "invalidOne", Type = "group" }
                }
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);

        It should_return_two_errors =
            () => Result.Errors.Count(e => e.Detail.Contains("resource")).ShouldEqual(2);
    }
}