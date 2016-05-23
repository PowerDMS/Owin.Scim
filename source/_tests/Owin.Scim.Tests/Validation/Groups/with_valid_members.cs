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
            Group = new ScimGroup
            {
                DisplayName = "blue man",
                Members = new []
                {
                    new Member {Value = ValidUserId, Type = "User" },
                    new Member {Value = ValidGroupId, Type = "Group" }
                }
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}