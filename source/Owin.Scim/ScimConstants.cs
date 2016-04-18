namespace Owin.Scim
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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

            public const string Schema = @"urn:ietf:params:scim:schemas:core:2.0:Schema";
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

            public const string Schema = @"Schema";

            public const string User = @"User";

            public const string Group = @"Group";
        }

        public static class Defaults
        {
            public const int BulkMaxOperations = 1000;

            public const int BulkMaxPayload = 1048576;

            public const int FilterMaxResults = 200;
        }

        public static class Endpoints
        {
            public const string Users = @"users";

            public const string Groups = @"groups";
        }

        public static class DataTypes
        {
            public const string String = @"string";

            public const string Boolean = @"boolean";

            public const string Decimal = @"decimal";

            public const string Integer = @"integer";

            public const string DateTime = @"datetime";

            public const string Reference = @"reference";

            public const string Binary = @"binary";

            public const string Complex = @"complex";
        }

        public static class ReferenceTypes
        {
            public const string External = @"external";

            public const string Uri = @"uri";
        }

        public static class CanonicalValues
        {
            public static IEnumerable<string> AddressTypes = new List<string>
            {
                @"work",
                @"home",
                @"other"
            };

            public static IEnumerable<string> EmailAddressTypes = new List<string>
            {
                @"work",
                @"home",
                @"other"
            };

            public static IEnumerable<string> InstantMessagingProviders = new List<string>
            {
                @"aim",
                @"gtalk",
                @"icq",
                @"xmpp",
                @"msn",
                @"skype",
                @"qq",
                @"yahoo"
            };

            public static IEnumerable<string> PhoneNumberTypes = new List<string>
            {
                @"work",
                @"home",
                @"mobile",
                @"fax",
                @"pager",
                @"other"
            };

            public static IEnumerable<string> PhotoTypes = new List<string>
            {
                @"photo",
                @"thumbnail"
            }; 

            public static IEnumerable<string> UserGroupTypes = new List<string>
            {
                @"direct",
                @"indirect"
            }; 
        }

        public static class Maps
        {
            public static readonly IReadOnlyDictionary<string, string> EndpointToTypeDictionary =
                new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
                {
                    {Endpoints.Users, ResourceTypes.User},
                    {Endpoints.Groups, ResourceTypes.Group}
                });
        }
    }
}