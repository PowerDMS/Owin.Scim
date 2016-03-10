namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_terminal_attribute_and_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "displayName eq \"Daniel\"";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression(null, "displayName eq \"Daniel\""));
    }
}