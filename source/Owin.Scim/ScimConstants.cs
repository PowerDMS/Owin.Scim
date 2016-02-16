namespace Owin.Scim
{
    public static class ScimConstants
    {
        public static class Schemas
        {
            public const string Key = @"schemas";

            public const string User = @"urn:ietf:params:scim:schemas:core:2.0:User";

            public const string UserEnterprise = @"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User";

            public const string Group = @"urn:ietf:params:scim:schemas:core:2.0:Group";

            public const string ServiceProviderConfig = @"urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig";

            public const string ResourceType = @"urn:ietf:params:scim:schemas:core:2.0:ResourceType";
        }

        public static class Messages
        {
            public const string UriPrefix = @"urn:ietf:params:scim:api:";

            public const string BulkRequest = @"urn:ietf:params:scim:api:messages:2.0:BulkRequest";

            public const string BulkResponse = @"urn:ietf:params:scim:api:messages:2.0:BulkResponse";

            public const string Error = @"urn:ietf:params:scim:api:messages:2.0:Error";

            public const string PatchOp = @"urn:ietf:params:scim:api:messages:2.0:PatchOp";
        }

        public static class ResourceTypes
        {
            public const string ServiceProviderConfig = @"ServiceProviderConfig";

            public const string ResourceType = @"ResourceType";

            public const string User = @"User";

            public const string Group = @"Group";
        }

        public static class Defaults
        {
            public const int BulkMaxOperations = 1000;

            public const int BulkMaxPayload = 1048576;

            public const int FilterMaxResults = 200;
        }
    }
}