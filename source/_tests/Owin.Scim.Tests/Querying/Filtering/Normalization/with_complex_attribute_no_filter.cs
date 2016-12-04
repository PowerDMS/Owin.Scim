namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_complex_attribute_no_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "name.familyName";

        It show_throw_exception = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("name", null),
            new PathFilterExpression("familyName", null));

        It should_equal = () => ScimFilter.NormalizedFilterExpression.ShouldEqual("name.familyName");
    }
}