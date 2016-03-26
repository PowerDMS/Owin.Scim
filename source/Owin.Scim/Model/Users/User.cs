namespace Owin.Scim.Model.Users
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Extensions;

    using Newtonsoft.Json;

    [Description("User accounts")]
    [ScimTypeDefinition(typeof(UserDefinition))]
    public class User : Resource
    {
        public User() : this(null)
        {
        }

        public User(params Type[] withExtensionTypes)
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.User);

            if (withExtensionTypes != null)
            {
                foreach (var extensionType in withExtensionTypes)
                {
                    AddExtension(extensionType);
                }
            }
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.User; }
        }

        [Description(@"
        Unique identifier for the User, typically used by the user to directly 
        authenticate to the service provider. Each User MUST include a non-empty 
        userName value.This identifier MUST be unique across the service 
        provider's entire set of Users. REQUIRED.")]
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [Description(@"
        The components of the user's real name. Providers MAY return just the full 
        name as a single string in the formatted sub-attribute, or they MAY return 
        just the individual component attributes using the other sub-attributes, or 
        they MAY return both.If both variants are returned, they SHOULD be describing 
        the same name, with the formatted name indicating how the component attributes 
        should be combined.")]
        [JsonProperty(PropertyName = "name")]
        public Name Name { get; set; }

        [Description(@"
        The name of the User, suitable for display to end-users.The name SHOULD 
        be the full name of the User being described, if known.")]
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [Description(@"
        The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' 
        instead of 'Robert'.  This attribute SHOULD NOT be used to represent a 
        User's username (e.g., 'bjensen' or 'mpepperidge').")]
        [JsonProperty(PropertyName = "nickName")]
        public string NickName { get; set; }

        [Description(@"A fully qualified URL pointing to a page representing the User's online profile.")]
        [JsonProperty(PropertyName = "profileUrl")]
        public Uri ProfileUrl { get; set; }

        [Description(@"The user's title, such as 'Vice President'.")]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [Description(@"
        Used to identify the relationship between the organization and the user. 
        Typical values used might be 'Contractor', 'Employee', 'Intern', 'Temp', 
        'External', and 'Unknown', but any value may be used.")]
        [JsonProperty(PropertyName = "userType")]
        public string UserType { get; set; }

        [Description(@"
        Indicates the User's preferred written or spoken language.Generally used 
        for selecting a localized user interface; e.g., 'en-US' specifies the 
        language English and country US.")]
        [JsonProperty(PropertyName = "preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [Description(@"
        Used to indicate the User's default location for purposes of localizing 
        items such as currency, date time format, or numerical representations.")]
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        [Description(@"The User's time zone in the 'Olson' time zone database format, e.g., 'America/Los_Angeles'.")]
        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }

        [Description(@"A Boolean value indicating the User's administrative status.")]
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }

        [Description(@"
        The User's cleartext password. This attribute is intended to be used as a means 
        to specify an initial password when creating a new User or to reset an existing 
        User's password.")]
        [JsonIgnore]
        public string Password { get; set; }

        [Description(@"
        Email addresses for the user.  The value SHOULD be canonicalized by the service 
        provider, e.g., 'bjensen@example.com' instead of 'bjensen@EXAMPLE.COM'. Canonical 
        type values of 'work', 'home', and 'other'.")]
        [JsonProperty(PropertyName = "emails")]
        public IEnumerable<Email> Emails { get; set; }

        [Description(@"
        Phone numbers for the User.  The value SHOULD be canonicalized by the service 
        provider according to the format specified in RFC 3966, e.g., 'tel:+1-201-555-0123'.
        Canonical type values of 'work', 'home', 'mobile', 'fax', 'pager', and 'other'.")]
        [JsonProperty(PropertyName = "phoneNumbers")]
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }

        [Description(@"Instant messaging addresses for the User.")]
        [JsonProperty(PropertyName = "ims")]
        public IEnumerable<InstantMessagingAddress> Ims { get; set; }

        [Description(@"URLs of photos of the User.")]
        [JsonProperty(PropertyName = "photos")]
        public IEnumerable<Photo> Photos { get; set; }

        [Description(@"A physical mailing address for this User. Canonical type values of 'work', 'home', and 'other'.")]
        [JsonProperty(PropertyName = "addresses")]
        public IEnumerable<Address> Addresses { get; set; }

        [Description(@"
        A list of groups to which the user belongs, either through direct membership, through 
        nested groups, or dynamically calculated.")]
        [JsonProperty(PropertyName = "groups")]
        public IEnumerable<UserGroup> Groups { get; set; }

        [Description(@"A list of entitlements for the User that represent a thing the User has.")]
        [JsonProperty(PropertyName = "entitlements")]
        public IEnumerable<Entitlement> Entitlements { get; set; }

        [Description(@"A list of roles for the User that collectively represent who the User is, e.g., 'Student', 'Faculty'.")]
        [JsonProperty(PropertyName = "roles")]
        public IEnumerable<Role> Roles { get; set; }

        [Description(@"A list of certificates issued to the User.")]
        [JsonProperty(PropertyName = "x509Certificates")]
        public IEnumerable<X509Certificate> X509Certificates { get; set; }
        
        public override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                Active,
                Locale,
                Name = Name?.CalculateVersion(),
                NickName,
                DisplayName,
                Password,
                PreferredLanguage,
                ProfileUrl,
                Timezone,
                Title,
                UserName,
                UserType,
                Addresses = Addresses.GetMultiValuedAttributeCollectionVersion(),
                Emails = Emails.GetMultiValuedAttributeCollectionVersion(),
                Entitlements = Entitlements.GetMultiValuedAttributeCollectionVersion(),
                Groups = Groups.GetMultiValuedAttributeCollectionVersion(),
                Ims = Ims.GetMultiValuedAttributeCollectionVersion(),
                PhoneNumbers = PhoneNumbers.GetMultiValuedAttributeCollectionVersion(),
                Photos = Photos.GetMultiValuedAttributeCollectionVersion(),
                Roles = Roles.GetMultiValuedAttributeCollectionVersion(),
                X509Certificates = X509Certificates.GetMultiValuedAttributeCollectionVersion()
            }.GetHashCode();
        }
    }
}