namespace Owin.Scim.Model.Users
{
    using Canonicalization;

    using Configuration;

    using PhoneNumbers;

    using Validation.Users;

    public class UserDefinition : ScimResourceTypeDefinitionBuilder<ScimUser>
    {
        public UserDefinition(ScimServerConfiguration serverConfiguration)
            : base(
                serverConfiguration,
                ScimConstants.ResourceTypes.User,
                ScimConstants.Schemas.User,
                ScimConstants.Endpoints.Users,
                typeof(UserValidator),
                schemaIdentifiers => schemaIdentifiers.Contains(ScimConstants.Schemas.User))
        {
            SetName(ScimConstants.ResourceTypes.User);
            SetDescription("User resource.");

            AddSchemaExtension<EnterpriseUserExtension, EnterpriseUserExtensionValidator>(ScimConstants.Schemas.UserEnterprise, false);

            For(u => u.Active)
                .SetDescription(@"A Boolean value indicating the User's administrative status.");

            For(u => u.Addresses)
                .SetDescription(@"A physical mailing address for this User.")
                .AddCanonicalizationRule((MailingAddress address, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(address, ref state));

            For(u => u.DisplayName)
                .SetDescription(@"
                    The name of the User, suitable for display to end-users.The name SHOULD 
                    be the full name of the User being described, if known.");

            For(u => u.Emails)
                .SetDescription(@"
                    Email addresses for the user.  The value SHOULD be canonicalized by the service 
                    provider, e.g., 'bjensen@example.com' instead of 'bjensen@EXAMPLE.COM'.")
                .AddCanonicalizationRule(email =>
                    email.Canonicalize(
                        e => e.Value,
                        e => e.Display,
                        value =>
                        {
                            if (string.IsNullOrWhiteSpace(value)) return null;

                            var atIndex = value.IndexOf('@') + 1;
                            if (atIndex == 0) return null; // IndexOf returned -1, invalid email

                            return value.Substring(0, atIndex) + value.Substring(atIndex).ToLower();
                        }))
                .AddCanonicalizationRule((Email email, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(email, ref state));

            For(u => u.Entitlements)
                .SetDescription(@"A list of entitlements for the User that represent a thing the User has.")
                .AddCanonicalizationRule((Entitlement entitlement, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(entitlement, ref state));

            For(u => u.Groups)
                .SetDescription(@"
                    A list of groups to which the user belongs, either through direct membership, through 
                    nested groups, or dynamically calculated.")
                .SetMutability(Mutability.ReadOnly);

            For(u => u.Id)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always)
                .SetUniqueness(Uniqueness.Server)
                .SetCaseExact(true);
            
            For(u => u.Ims)
                .SetDescription(@"Instant messaging addresses for the User.")
                .AddCanonicalizationRule((InstantMessagingAddress im, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(im, ref state));

            For(u => u.Locale)
                .SetDescription(@"
                    Used to indicate the User's default location for purposes of localizing 
                    items such as currency, date time format, or numerical representations.")
                .AddCanonicalizationRule(locale => !string.IsNullOrWhiteSpace(locale) ? locale.Replace('_', '-') : locale);

            For(u => u.NickName)
                .SetDescription(@"
                    The casual way to address the user in real life, e.g., 'Bob' or 'Bobby' 
                    instead of 'Robert'.  This attribute SHOULD NOT be used to represent a 
                    User's username (e.g., 'bjensen' or 'mpepperidge').");
            
            For(u => u.Meta)
                .SetMutability(Mutability.ReadOnly);

            For(u => u.Name)
                .SetDescription(@"
                    The components of the user's real name. Providers MAY return just the full 
                    name as a single string in the formatted sub-attribute, or they MAY return 
                    just the individual component attributes using the other sub-attributes, or 
                    they MAY return both.If both variants are returned, they SHOULD be describing 
                    the same name, with the formatted name indicating how the component attributes 
                    should be combined.");

            For(u => u.Password)
                .SetDescription(@"
                    The User's cleartext password. This attribute is intended to be used as a means 
                    to specify an initial password when creating a new User or to reset an existing 
                    User's password.")
                .SetMutability(Mutability.WriteOnly)
                .SetReturned(Returned.Never);

            For(u => u.PhoneNumbers)
                .SetDescription(@"
                    Phone numbers for the User.  The value SHOULD be canonicalized by the service 
                    provider according to the format specified in RFC 3966, e.g., 'tel:+1-201-555-0123'.")
                .AddCanonicalizationRule(phone => phone.Canonicalize(p => p.Value, p => p.Display, PhoneNumberUtil.Normalize))
                .AddCanonicalizationRule((PhoneNumber phone, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(phone, ref state));
            
            For(u => u.Photos)
                .SetDescription(@"URLs of photos of the User.")
                .AddCanonicalizationRule((Photo photo, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(photo, ref state));

            For(u => u.PreferredLanguage)
                .SetDescription(@"
                    Indicates the User's preferred written or spoken language.Generally used 
                    for selecting a localized user interface; e.g., 'en-US' specifies the 
                    language English and country US.");
            
            For(u => u.ProfileUrl)
                .SetDescription(@"A fully qualified URL pointing to a page representing the User's online profile.")
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));

            For(u => u.Roles)
                .SetDescription(@"A list of roles for the User that collectively represent who the User is, e.g., 'Student', 'Faculty'.")
                .AddCanonicalizationRule((Role role, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(role, ref state));

            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(u => u.Timezone)
                .SetDescription(@"The User's time zone in the 'Olson' time zone database format, e.g., 'America/Los_Angeles'.");

            For(u => u.Title)
                .SetDescription(@"The user's title, such as 'Vice President'.");

            For(u => u.UserName)
                .SetDescription(@"
                    Unique identifier for the User, typically used by the user to directly 
                    authenticate to the service provider. Each User MUST include a non-empty 
                    userName value.This identifier MUST be unique across the service 
                    provider's entire set of Users. REQUIRED.")
                .SetRequired(true)
                .SetUniqueness(Uniqueness.Server);

            For(u => u.UserType)
                .SetDescription(@"
                    Used to identify the relationship between the organization and the user. 
                    Typical values used might be 'Contractor', 'Employee', 'Intern', 'Temp', 
                    'External', and 'Unknown', but any value may be used.");

            For(u => u.X509Certificates)
                .SetDescription(@"A list of certificates issued to the User.")
                .AddCanonicalizationRule((X509Certificate certificate, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(certificate, ref state));
        }
    }
}