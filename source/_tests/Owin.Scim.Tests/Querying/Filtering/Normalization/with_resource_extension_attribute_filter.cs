using Machine.Specifications;
using Owin.Scim.Querying;

namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    public class with_resource_extension_attribute_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department eq \"Development\"";

        It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User", null),
            new PathFilterExpression(null, "department eq \"Development\""));

        It should_equal = () => ScimFilter.NormalizedFilterExpression.ShouldEqual(
            "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department eq \"Development\"");
    }
}