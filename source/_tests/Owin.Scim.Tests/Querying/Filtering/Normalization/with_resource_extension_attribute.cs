namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    using Machine.Specifications;

    using Scim.Querying;

    [Ignore("not supported yet")]
    public class with_resource_extension_attribute : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("urn:ietf:params:scim:schemas:extension:enterprise:2.0", null),
            new PathFilterExpression("employeeNumber", null));
    }
}