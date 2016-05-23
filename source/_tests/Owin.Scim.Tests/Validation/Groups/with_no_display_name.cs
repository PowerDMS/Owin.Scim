namespace Owin.Scim.Tests.Validation.Groups
{
    using System.Linq;

    using Machine.Specifications;

    using Model.Groups;

    public class with_no_display_name : when_creating_a_group
    {
        Establish ctx = () =>
        {
            Group = new ScimGroup
            {
                ExternalId = "some Id",
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);

        It should_indicate_invalid_value =
            () => Result.Errors.First().ScimType.ShouldEqual(Model.ScimErrorType.InvalidValue);

        It should_specify_field_name = () => Result.Errors.First().Detail.ShouldContain("displayName");
    }
}