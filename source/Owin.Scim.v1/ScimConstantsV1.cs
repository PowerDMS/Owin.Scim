namespace Owin.Scim.v1
{
    public class ScimConstantsV1 : ScimConstants
    {
        public static class Schemas
        {
            public const string User = @"urn:scim:schemas:core:1.0";

            public const string UserEnterprise = @"urn:scim:schemas:extension:enterprise:1.0";

            public const string Group = @"urn:scim:schemas:core:1.0";

            public const string ServiceProviderConfig = @"urn:scim:schemas:core:1.0";

            public const string Schema = @"urn:scim:schemas:core:1.0";
        }

        public static class Messages
        {
            public const string Error = @"urn:ietf:params:scim:api:messages:2.0:Error";

            public const string ListResponse = @"urn:scim:schemas:core:1.0";

            public const string SearchRequest = @"urn:ietf:params:scim:api:messages:2.0:SearchRequest";
        }

        public static class Defaults
        {
            public const string URNPrefix = @"urn:";

            public const int BulkMaxOperations = 1000;

            public const int BulkMaxPayload = 1048576;

            public const int FilterMaxResults = 200;
        }

        public static class Endpoints
        {
            public const string Users = @"v1/users";

            public const string Groups = @"v1/groups";

            public const string ServiceProviderConfig = @"v1/serviceproviderconfigs";

            public const string Schemas = @"v1/schemas";
        }
    }
}