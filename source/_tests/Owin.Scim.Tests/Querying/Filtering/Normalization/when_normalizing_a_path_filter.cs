namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    public class when_normalizing_a_path_filter
    {
        Because of = () =>
        {
            ScimFilter = new ScimFilter(PathFilter);
        };

        protected static ScimFilter ScimFilter;

        protected static string PathFilter;
    }
}