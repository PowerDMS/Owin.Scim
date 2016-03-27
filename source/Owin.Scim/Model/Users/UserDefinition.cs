namespace Owin.Scim.Model.Users
{
    using Canonicalization;

    using Configuration;

    using PhoneNumbers;

    using Validation.Users;

    public class UserDefinition : ScimResourceTypeDefinitionBuilder<User>
    {
        public UserDefinition()
            : base(
                ScimConstants.ResourceTypes.User,
                ScimConstants.Schemas.User,
                ScimConstants.Endpoints.Users,
                typeof(UserValidator))
        {
            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(u => u.Id)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always)
                .SetUniqueness(Uniqueness.Server)
                .SetCaseExact(true);

            For(u => u.UserName)
                .SetRequired(true)
                .SetUniqueness(Uniqueness.Server);

            For(u => u.Locale)
                .AddCanonicalizationRule(locale => !string.IsNullOrWhiteSpace(locale) ? locale.Replace('_', '-') : locale);

            For(u => u.ProfileUrl)
                .SetReferenceTypes("external");

            For(u => u.Password)
                .SetMutability(Mutability.WriteOnly)
                .SetReturned(Returned.Never);

            For(u => u.Meta)
                .SetMutability(Mutability.ReadOnly);

            For(u => u.Emails)
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
                .AddCanonicalizationRule((Email attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            For(u => u.PhoneNumbers)
                .AddCanonicalizationRule(phone => phone.Canonicalize(p => p.Value, p => p.Display, PhoneNumberUtil.Normalize))
                .AddCanonicalizationRule((PhoneNumber phone, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(phone, ref state));

            For(u => u.Groups)
                .AddCanonicalizationRule((UserGroup group, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(group, ref state))
                .SetMutability(Mutability.ReadOnly);

            For(u => u.Addresses)
                .AddCanonicalizationRule((Address address, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(address, ref state));

            For(u => u.Roles)
                .AddCanonicalizationRule((Role role, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(role, ref state));

            For(u => u.Entitlements)
                .AddCanonicalizationRule((Entitlement entitlement, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(entitlement, ref state));

            For(u => u.Ims)
                .AddCanonicalizationRule((InstantMessagingAddress im, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(im, ref state));

            For(u => u.Photos)
                .AddCanonicalizationRule((Photo photo, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(photo, ref state));

            For(u => u.X509Certificates)
                .AddCanonicalizationRule((X509Certificate certificate, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(certificate, ref state));
        }
    }
}