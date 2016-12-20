using Machine.Specifications;
using Owin.Scim.Querying;

namespace Owin.Scim.Tests.Querying.Filtering.Normalization
{
    public class with_resource_extension_attribute_and_complex_filter : when_normalizing_a_path_filter
    {
        Establish context = () => PathFilter = "(userType eq \"Employee\" or name.givenName eq \"Employee\") and (urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\")";

        private It should_contain = () => ScimFilter.Paths.ShouldContainOnly(
            new PathFilterExpression(null, "(userType eq \"Employee\" or name[givenName eq \"Employee\"]) and (urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\")"));

        It should_equal = () =>
            ScimFilter.NormalizedFilterExpression.ShouldEqual("(userType eq \"Employee\" or name[givenName eq \"Employee\"]) and (urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber eq \"123\")");
    }
}