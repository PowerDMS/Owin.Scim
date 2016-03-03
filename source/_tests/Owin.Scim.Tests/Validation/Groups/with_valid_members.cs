namespace Owin.Scim.Tests.Validation.Groups
{
    using System;
    using System.Linq;

    using Machine.Specifications;

    using Model.Groups;

    /// <summary>
    /// members must provide $ref or a value/type combo
    /// </summary>
    public class with_valid_members : when_creating_a_group
    {
        Establish ctx = () =>
        {
            Group = new Group
            {
                DisplayName = "blue man",
                Members = new []
                {
                    new Member {Ref = new Uri(@"http://local/Scim/V2/users/" + ValidUserId) },
                    new Member {Value = ValidGroupId, Type = "group" }
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}