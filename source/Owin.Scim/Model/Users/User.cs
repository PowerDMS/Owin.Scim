namespace Owin.Scim.Model.Users
{
    using System;
    using System.Collections.Generic;

    public class User : Resource
    {
        public override ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>
                {
                    ScimConstants.Schemas.User
                };
            }
        }

        /// <summary>
        /// Unique identifier for the User, typically used by the user to directly 
        /// authenticate to the service provider. Each User MUST include a non-empty 
        /// userName value.This identifier MUST be unique across the service 
        /// provider's entire set of Users. REQUIRED.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The components of the user's real name. Providers MAY return just the full 
        /// name as a single string in the formatted sub-attribute, or they MAY return 
        /// just the individual component attributes using the other sub-attributes, or 
        /// they MAY return both.If both variants are returned, they SHOULD be describing 
        /// the same name, with the formatted name indicating how the component attributes 
        /// should be combined.
        /// </summary>
        public Name Name { get; set; }

        /// <summary>
        /// The name of the User, suitable for display to end-users.The name SHOULD 
        /// be the full name of the User being described, if known.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' 
        /// instead of 'Robert'.  This attribute SHOULD NOT be used to represent a 
        /// User's username (e.g., 'bjensen' or 'mpepperidge').
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// A fully qualified URL pointing to a page representing the User's online profile.
        /// </summary>
        public Uri ProfileUrl { get; set; }

        /// <summary>
        /// The user's title, such as "Vice President".
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Used to identify the relationship between the organization and the user. 
        /// Typical values used might be 'Contractor', 'Employee', 'Intern', 'Temp', 
        /// 'External', and 'Unknown', but any value may be used.
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Indicates the User's preferred written or spoken language.Generally used 
        /// for selecting a localized user interface; e.g., 'en-US' specifies the 
        /// language English and country US.
        /// </summary>
        public string PreferredLanguage { get; set; }

        /// <summary>
        /// Used to indicate the User's default location for purposes of localizing 
        /// items such as currency, date time format, or numerical representations.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The User's time zone in the 'Olson' time zone database format, e.g., 'America/Los_Angeles'.
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// A Boolean value indicating the User's administrative status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The User's cleartext password. This attribute is intended to be used as a means 
        /// to specify an initial password when creating a new User or to reset an existing 
        /// User's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Email addresses for the user.  The value SHOULD be canonicalized by the service 
        /// provider, e.g., 'bjensen@example.com' instead of 'bjensen@EXAMPLE.COM'. Canonical 
        /// type values of 'work', 'home', and 'other'.
        /// </summary>
        public IEnumerable<Email> Emails { get; set; }

        /// <summary>
        /// Phone numbers for the User.  The value SHOULD be canonicalized by the service 
        /// provider according to the format specified in RFC 3966, e.g., 'tel:+1-201-555-0123'.
        /// Canonical type values of 'work', 'home', 'mobile', 'fax', 'pager', and 'other'.
        /// </summary>
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// Instant messaging addresses for the User.
        /// </summary>
        public IEnumerable<InstantMessagingAddress> Ims { get; set; }

        /// <summary>
        /// URLs of photos of the User.
        /// </summary>
        public IEnumerable<Photo> Photos { get; set; }

        /// <summary>
        /// A physical mailing address for this User. Canonical type values of 'work', 'home', and 'other'.
        /// </summary>
        public IEnumerable<Address> Addresses { get; set; }

        /// <summary>
        /// A list of groups to which the user belongs, either through direct membership, through 
        /// nested groups, or dynamically calculated.
        /// </summary>
        public IEnumerable<UserGroup> Groups { get; set; }

        /// <summary>
        /// A list of entitlements for the User that represent a thing the User has.
        /// </summary>
        public IEnumerable<Entitlement> Entitlements { get; set; }

        /// <summary>
        /// A list of roles for the User that collectively represent who the User is, e.g., 'Student', 'Faculty'.
        /// </summary>
        public IEnumerable<Role> Roles { get; set; }

        /// <summary>
        /// A list of certificates issued to the User.
        /// </summary>
        public IEnumerable<X509Certificate> X509Certificates { get; set; }
    }
}