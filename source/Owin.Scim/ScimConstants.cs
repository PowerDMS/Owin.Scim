namespace Owin.Scim
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class ScimConstants
    {
        public static class Owin
        {
            public const string BasePath = @"owin.scim:basePath";

            public const string BaseUri = @"owin.scim:baseUri";

            public const string Host = @"owin.scim:host";

            public const string HttpMethod = @"owin.scim:httpMethod";

            public const string QueryOptions = @"owin.scim:queryOptions";
        }

        public static class Schemas
        {
            public const string Key = @"schemas";

            public const string User = @"urn:ietf:params:scim:schemas:core:2.0:User";

            public const string UserEnterprise = @"urn:ietf:params:scim:schemas:extension:enterprise:2.0:User";

            public const string Group = @"urn:ietf:params:scim:schemas:core:2.0:Group";

            public const string ServiceProviderConfig = @"urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig";

            public const string ResourceType = @"urn:ietf:params:scim:schemas:core:2.0:ResourceType";

            public const string Schema = @"urn:ietf:params:scim:schemas:core:2.0:Schema";

            public const string BulkRequest = @"urn:ietf:params:scim:api:messages:2.0:BulkRequest";

            public const string BulkResponse = @"urn:ietf:params:scim:api:messages:2.0:BulkResponse";
        }

        public static class Messages
        {
            public const string UriPrefix = @"urn:ietf:params:scim:api:";

            public const string BulkRequest = @"urn:ietf:params:scim:api:messages:2.0:BulkRequest";

            public const string BulkResponse = @"urn:ietf:params:scim:api:messages:2.0:BulkResponse";

            public const string Error = @"urn:ietf:params:scim:api:messages:2.0:Error";

            public const string PatchOp = @"urn:ietf:params:scim:api:messages:2.0:PatchOp";

            public const string ListResponse = @"urn:ietf:params:scim:api:messages:2.0:ListResponse";

            public const string SearchRequest = @"urn:ietf:params:scim:api:messages:2.0:SearchRequest";
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
            public const string URNPrefix = @"urn:";

            public const int BulkMaxOperations = 1000;

            public const int BulkMaxPayload = 1048576;

            public const int FilterMaxResults = 200;
        }

        public static class Endpoints
        {
            public const string Users = @"users";

            public const string Groups = @"groups";

            public const string ResourceTypes = @"resourcetypes";

            public const string ServiceProviderConfig = @"serviceproviderconfig";

            public const string Schemas = @"schemas";
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

            public static IEnumerable<string> ScimDataTypes = new List<string>
            {
                @"string",
                @"complex",
                @"boolean",
                @"decimal",
                @"integer",
                @"dateTime",
                @"reference",
                @"binary"
            };

            public static IEnumerable<string> ScimMutabilityOptions = new List<string>
            {
                @"readOnly",
                @"readWrite",
                @"immutable",
                @"writeOnly"
            };

            public static IEnumerable<string> ScimReturnedOptions = new List<string>
            {
                @"always",
                @"never",
                @"default",
                @"request"
            };

            public static IEnumerable<string> ScimUniquenessOptions = new List<string>
            {
                @"none",
                @"server",
                @"global"
            };

            public static IEnumerable<string> ScimReferenceOptions = new List<string>
            {
                @"A SCIM resource type (e.g., ""User"" or ""Group"")",
                @"external",
                @"uri"
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