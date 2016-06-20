namespace Owin.Scim.v2.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;

    using Configuration;

    using ErrorHandling;

    using Extensions;

    using FluentValidation;

    using Model;

    using Repository;

    using Scim.Model;
    using Scim.Model.Users;
    using Scim.Validation;

    using Security;

    public class ScimUser2Validator : ResourceValidatorBase<ScimUser2>
    {
        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;
        
        public ScimUser2Validator(
            ScimServerConfiguration serverConfiguration,
            ResourceExtensionValidators extensionValidators,
            IUserRepository userRepository,
            IManagePasswords passwordManager)
            : base(serverConfiguration, extensionValidators)
        {
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
        }

        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(u => u.UserName)
                       .NotEmpty()
                       .WithState(u =>
                           new ScimError(
                               HttpStatusCode.BadRequest,
                               ScimErrorType.InvalidValue,
                               ScimErrorDetail.AttributeRequired("userName")));

            When(user => !string.IsNullOrWhiteSpace(user.PreferredLanguage),
                () =>
                {
                    RuleFor(user => user.PreferredLanguage)
                        .Must(ValidatePreferredLanguage)
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                "The attribute 'preferredLanguage' is formatted the same " +
                                "as the HTTP Accept-Language header field. (e.g., da, en-gb;q=0.8, en;q=0.7)"));
                });
            When(user => user.ProfileUrl != null,
                () =>
                {
                    RuleFor(user => user.ProfileUrl)
                        .Must(uri => uri.IsAbsoluteUri)
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                "The attribute 'profileUrl' must be a valid absolute URI."));
                });
            When(user => !string.IsNullOrWhiteSpace(user.Locale),
                () =>
                {
                    RuleFor(user => user.Locale)
                        .Must(locale =>
                        {
                            try
                            {
                                CultureInfo.GetCultureInfo(locale);
                                return true;
                            }
                            catch (Exception)
                            {
                            }

                            return false;
                        })
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                "The attribute 'locale' MUST be a valid language tag as defined in [RFC5646]."));
                });
            When(user => user.Emails != null && user.Emails.Any(),
                () =>
                {
                    RuleFor(user => user.Emails)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Email>
                            {
                                    {
                                        email => email.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("email.value")))
                                            .EmailAddress()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    "The attribute 'email.value' must be a valid email as defined in [RFC5321]."))
                                    }
                            });
                });
            When(user => user.Ims != null && user.Ims.Any(),
                () =>
                {
                    RuleFor(user => user.Ims)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<InstantMessagingAddress>
                            {
                                    {
                                        im => im.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("im.value")))
                                    }
                            });
                });
            When(user => user.PhoneNumbers != null && user.PhoneNumbers.Any(),
                () =>
                {
                        // The value SHOULD be specified according to the format defined 
                        // in [RFC3966], e.g., 'tel:+1-201-555-0123'.
                        RuleFor(user => user.PhoneNumbers)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<PhoneNumber>
                            {
                                    {
                                        pn => pn.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("phoneNumber.value")))
                                            .Must(PhoneNumbers.PhoneNumberUtil.IsViablePhoneNumber)
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    "The attribute 'phoneNumber.value' must be a valid phone number as defined in [RFC3966]."))
                                    }
                            });
                });
            When(user => user.Photos != null && user.Photos.Any(),
                () =>
                {
                    RuleFor(user => user.Photos)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Photo>
                            {
                                    {
                                        photo => photo.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("photo.value")))
                                            .Must(uri => uri.IsAbsoluteUri)
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    "The attribute 'photo.value' must be a valid absolute URI."))
                                    }
                            });
                });
            When(user => user.Addresses != null && user.Addresses.Any(),
                () =>
                {
                    RuleFor(user => user.Addresses)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<MailingAddress>
                            {
                                    v => v.When(a => !string.IsNullOrWhiteSpace(a.Country),
                                        () =>
                                        {
                                            v.RuleFor(a => a.Country)
                                                .Must(countryCode =>
                                                {
                                                    try
                                                    {
                                                        new RegionInfo(countryCode);
                                                        return true;
                                                    }
                                                    catch
                                                    {
                                                    }

                                                    return false;
                                                })
                                                .WithState(u =>
                                                    new ScimError(
                                                        HttpStatusCode.BadRequest,
                                                        ScimErrorType.InvalidValue,
                                                        "The attribute 'address.country' must be a valid country code as defined by [ISO3166-1 alpha-2]."));
                                        })
                            });
                });
            When(user => user.Entitlements != null && user.Entitlements.Any(),
                () =>
                {
                    RuleFor(user => user.Entitlements)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Entitlement>
                            {
                                    {
                                        entitlement => entitlement.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("entitlement.value")))
                                    }
                            });
                });
            When(user => user.Roles != null && user.Roles.Any(),
                () =>
                {
                    RuleFor(user => user.Roles)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Role>
                            {
                                    {
                                        role => role.Value,
                                        config => config
                                            .NotEmpty()
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    ScimErrorDetail.AttributeRequired("role.value")))
                                    }
                            });
                });
        }

        protected override void ConfigureCreateRuleSet()
        {
            When(user => !string.IsNullOrWhiteSpace(user.UserName),
                () =>
                {
                    RuleFor(user => user.UserName)
                        .MustAsync(async (userName, token) => await _UserRepository.IsUserNameAvailable(userName))
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.Conflict,
                                ScimErrorType.Uniqueness,
                                ScimErrorDetail.AttributeUnique("userName")));
                });

            When(user => user.Password != null,
                () =>
                {
                    RuleFor(user => user.Password)
                        .MustAsync(async (password, token) => await _PasswordManager.MeetsRequirements(password))
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                "The attribute 'password' does not meet the security requirements set by the provider."));
                });
        }

        protected override void ConfigureUpdateRuleSet()
        {
            RuleFor(user => user.Id)
                .Immutable(() => ExistingRecord.Id, StringComparer.OrdinalIgnoreCase)
                .WithState(u =>
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.Mutability,
                        ScimErrorDetail.AttributeImmutable("id")));

            // Updating a username validation
            When(user =>
                user.UserName != null &&
                !user.UserName.Equals(ExistingRecord.UserName, StringComparison.OrdinalIgnoreCase),
                () =>
                {
                    RuleFor(user => user.UserName)
                        .MustAsync(async (user, userName, token) => await _UserRepository.IsUserNameAvailable(userName))
                        .WithState(user =>
                            new ScimError(
                                HttpStatusCode.Conflict,
                                ScimErrorType.Uniqueness,
                                ScimErrorDetail.AttributeUnique("userName")));
                });

            // Updating a user password
            When(user => user.Password != null && _PasswordManager.PasswordIsDifferent(user.Password, ExistingRecord.Password),
                () =>
                {
                    RuleFor(user => user.Password)
                        .MustAsync(async (password, token) => await _PasswordManager.MeetsRequirements(password))
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.InvalidValue,
                                "The attribute 'password' does not meet the security requirements set by the provider."));
                });
        }

        /// <summary>
        /// The value indicates the set of natural languages that are preferred. 
        /// The format of the value is the same as the HTTP Accept-Language header 
        /// field (not including "Accept-Language:") and is specified in Section 
        /// 5.3.5 of[RFC7231].  The intent of this value is to enable cloud 
        /// applications to perform matching of language tags [RFC4647]
        /// </summary>
        /// <param name="preferredLanguage"></param>
        /// <returns></returns>
        private bool ValidatePreferredLanguage(string preferredLanguage)
        {
            IEnumerable<Tuple<string, decimal>> stringsWithQuality;
            if (TryParseWeightedValues(preferredLanguage, out stringsWithQuality))
            {
                if (stringsWithQuality.Any(
                    langWithQuality =>
                    {
                        try
                        {
                            CultureInfo.GetCultureInfo(langWithQuality.Item1);
                            return true;
                        }
                        catch (CultureNotFoundException) { }

                        return false;
                    }))
                {
                    return true;
                }
            }

            return false;
        }
            
        private bool TryParseWeightedValues(string multipleValueStringWithQuality, out IEnumerable<Tuple<string, decimal>> stringsWithQuality)
        {
            stringsWithQuality = null;

            if (String.IsNullOrWhiteSpace(multipleValueStringWithQuality)) return false;

            var values = multipleValueStringWithQuality.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            if (!values.Any()) return false;

            var parsed = values.Select(x =>
            {
                var sections = x.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var mediaRange = sections[0].Trim();
                var quality = 1m;

                for (var index = 1; index < sections.Length; index++)
                {
                    var trimmedValue = sections[index].Trim();
                    if (trimmedValue.StartsWith("q=", StringComparison.OrdinalIgnoreCase))
                    {
                        decimal temp;
                        var stringValue = trimmedValue.Substring(2);
                        if (Decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                        {
                            quality = temp;
                        }
                    }
                    else
                    {
                        mediaRange += ";" + trimmedValue;
                    }
                }

                return new Tuple<string, decimal>(mediaRange, quality);
            });

            if (!parsed.Any()) return false;

            stringsWithQuality = parsed.OrderByDescending(x => x.Item2).ToArray();

            return true;
        }
    }
}