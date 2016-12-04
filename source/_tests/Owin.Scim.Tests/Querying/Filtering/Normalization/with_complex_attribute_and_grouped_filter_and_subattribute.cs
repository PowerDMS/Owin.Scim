namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_complex_attribute_and_grouped_filter_and_subattribute : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "emails[type eq \"work\"].value";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("emails", "type eq \"work\""),
            new PathFilterExpression("value", null));

        It should_equal = () => ScimFilter.NormalizedFilterExpression.ShouldEqual("emails[type eq \"work\"].value");
    }
}