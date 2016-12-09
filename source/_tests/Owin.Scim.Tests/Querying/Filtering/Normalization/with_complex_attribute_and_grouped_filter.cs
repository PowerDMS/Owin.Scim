namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_complex_attribute_and_grouped_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "name[givenName eq \"Daniel\"]";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("name", "givenName eq \"Daniel\""));

        It should_equal = () => ScimFilter.NormalizedFilterExpression.ShouldEqual("name[givenName eq \"Daniel\"]");
    }
}