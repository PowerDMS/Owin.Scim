using Machine.Specifications;
using Owin.Scim.Querying;

namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    public class with_resource_extension_attribute_exp_at_the_begining_and_complex_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\" and meta.resourceType eq \"User\"";

        private It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", null),
            new PathFilterExpression(null, "employeeNumber eq \"123\" and meta[resourceType eq \"User\"]"));

        It should_equal = () =>
            ScimFilter.NormalizedFilterExpression.ShouldEqual("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\" and meta[resourceType eq \"User\"]");
    }
}