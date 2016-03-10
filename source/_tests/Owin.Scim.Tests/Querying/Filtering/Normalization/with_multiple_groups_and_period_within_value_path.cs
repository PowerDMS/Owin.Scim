namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class with_multiple_groups_and_period_within_value_path : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "userType eq \"Employee\" and (emails co \"example.com\" or emails.value co \"example.org\")";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression(null, "userType eq \"Employee\" and (emails co \"example.com\" or emails[value co \"example.org\"])"));
    }
}