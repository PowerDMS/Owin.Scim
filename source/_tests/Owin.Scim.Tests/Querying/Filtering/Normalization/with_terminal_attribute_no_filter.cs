namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_terminal_attribute_no_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "displayName";

        It show_throw_exception = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("displayName", null));

        It should_equal = () => ScimFilter.NormalizedFilterExpression.ShouldEqual("displayName");
    }
}