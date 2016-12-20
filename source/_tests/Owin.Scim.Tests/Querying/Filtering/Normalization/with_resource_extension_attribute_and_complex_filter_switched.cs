using Machine.Specifications;
using Owin.Scim.Querying;

namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    public class with_resource_extension_attribute_and_complex_filter_switched : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "(urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\") and (userType eq \"Employee\" or name.givenName eq \"Employee\")";

        private It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression(null, "(urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\") and (userType eq \"Employee\" or name[givenName eq \"Employee\"])"));

        It should_equal = () =>
            ScimFilter.NormalizedFilterExpression.ShouldEqual("(urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\") and (userType eq \"Employee\" or name[givenName eq \"Employee\"])");
    }
}