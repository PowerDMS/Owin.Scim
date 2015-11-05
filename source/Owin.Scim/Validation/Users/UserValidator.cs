namespace Owin.Scim.Validation.Users
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using ErrorHandling;

    using Extensions;

    using FluentValidation;

    using Model;
    using Model.Users;

    using Repository;

    using Security;

    public class UserValidator : ValidatorBase<User>
    {
        private readonly IUserRepository _UserRepository;

        private readonly IVerifyPasswordComplexity _PasswordComplexityVerifier;

        private readonly IManagePasswords _PasswordManager;

        public UserValidator(
            IUserRepository userRepository,
            IVerifyPasswordComplexity passwordComplexityVerifier,
            IManagePasswords passwordManager)
        {
            _UserRepository = userRepository;
            _PasswordComplexityVerifier = passwordComplexityVerifier;
            _PasswordManager = passwordManager;
        }

        protected override async Task<ValidationResult> ValidateAsyncInternal(User entity, string ruleSet = RuleSetConstants.Default)
        {
            var validator = await CreateFluentValidator();

            var result = await validator.ValidateAsync(entity, ruleSet: ruleSet);

            return new ValidationResult(
                result.Errors.Any() 
                    ? result.Errors.Select(e => (ScimError)e.CustomState) 
                    : null);
        }

        private Task<IValidator<User>> CreateFluentValidator()
        {
            return Task.FromResult<IValidator<User>>(
                new FluentUserValidator(_UserRepository, _PasswordComplexityVerifier, _PasswordManager));
        }

        private class FluentUserValidator : AbstractValidator<User>
        {
            private readonly IUserRepository _UserRepository;

            private readonly IVerifyPasswordComplexity _PasswordComplexityVerifier;

            private readonly IManagePasswords _PasswordManager;

            private string _UserId;

            public FluentUserValidator(
                IUserRepository userRepository,
                IVerifyPasswordComplexity passwordComplexityVerifier,
                IManagePasswords passwordManager)
            {
                _UserRepository = userRepository;
                _PasswordComplexityVerifier = passwordComplexityVerifier;
                _PasswordManager = passwordManager;

                var userRecord = new Lazy<User>(() => GetUser().Result, LazyThreadSafetyMode.ExecutionAndPublication);
                ConfigureDefaultRuleSet();
                ConfigureCreateRuleSet();
                ConfigureUpdateRuleSet(userRecord);
            }

            private void ConfigureDefaultRuleSet()
            {
                RuleSet("default", () =>
                {
                    RuleFor(u => u.UserName)
                        .NotEmpty()
                        .WithState(u => 
                            new ScimError(
                                HttpStatusCode.BadRequest, 
                                ScimErrorType.InvalidValue, 
                                ErrorDetail.AttributeRequired("userName")));

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
                                                        ErrorDetail.AttributeRequired("email.value")))
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
                                                        ErrorDetail.AttributeRequired("im.value")))
                                        }
                                    });
                        });
                    When(user => user.PhoneNumbers != null && user.PhoneNumbers.Any(),
                        () =>
                        {
                            // TODO: (DG) Add validation / configuration for PhoneNumberTypes for validation.
                            /* The value SHOULD be specified according to the format defined 
                               in [RFC3966], e.g., 'tel:+1-201-555-0123'. */

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
                                                        ErrorDetail.AttributeRequired("phoneNumber.value")))
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
                                                        ErrorDetail.AttributeRequired("photo.value")))
                                                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                                                .WithState(u =>
                                                    new ScimError(
                                                        HttpStatusCode.BadRequest,
                                                        ScimErrorType.InvalidValue,
                                                        "The attribute 'photo.value' must be a valid URI."))
                                        }
                                    });
                        });
                    When(user => user.Addresses != null && user.Addresses.Any(),
                        () =>
                        {
                            RuleFor(user => user.Addresses)
                                .SetCollectionValidator(
                                    new GenericExpressionValidator<Address>
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
                                                        ErrorDetail.AttributeRequired("entitlement.value")))
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
                                                        ErrorDetail.AttributeRequired("role.value")))
                                        }
                                    });
                        });
                });
            }

            private void ConfigureCreateRuleSet()
            {
                RuleSet("create", () =>
                {
                    RuleFor(user => user.UserName)
                        .MustAsync(async userName =>
                        {
                            return await _UserRepository.IsUserNameAvailable(userName);
                        })
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.Conflict,
                                ScimErrorType.Uniqueness,
                                ErrorDetail.AttributeUnique("userName")));

                    When(user => !string.IsNullOrWhiteSpace(user.Password),
                        () =>
                        {
                            RuleFor(user => user.Password)
                                .MustAsync(password => _PasswordComplexityVerifier.MeetsRequirements(password))
                                .WithState(u =>
                                    new ScimError(
                                        HttpStatusCode.BadRequest,
                                        ScimErrorType.InvalidValue,
                                        "The attribute 'password' does not meet the security requirements set by the provider."));
                        });
                });
            }

            private void ConfigureUpdateRuleSet(Lazy<User> userRecord)
            {
                RuleSet("update", () =>
                {
                    RuleFor(user => user.Id)
                        .Immutable(() => userRecord.Value.Id, StringComparer.OrdinalIgnoreCase)
                        .WithState(u =>
                            new ScimError(
                                HttpStatusCode.BadRequest,
                                ScimErrorType.Mutability,
                                ErrorDetail.AttributeImmutable("id")));

                    // Updating a username validation
                    When(user =>
                        !string.IsNullOrWhiteSpace(user.UserName) &&
                        !user.UserName.Equals(userRecord.Value.UserName, StringComparison.OrdinalIgnoreCase),
                        () =>
                        {
                            RuleFor(user => user.UserName)
                                .MustAsync(async (user, userName) =>
                                {
                                    return await _UserRepository.IsUserNameAvailable(userName);
                                })
                                .WithState(u =>
                                    new ScimError(
                                        HttpStatusCode.Conflict,
                                        ScimErrorType.Uniqueness,
                                        ErrorDetail.AttributeUnique("userName")));
                        });

                    // Updating a user password
                    When(user =>
                        !string.IsNullOrWhiteSpace(user.Password) &&
                        (userRecord.Value.Password == null ||
                         !_PasswordManager.VerifyHash(user.Password, userRecord.Value.Password)),
                        () =>
                        {
                            RuleFor(user => user.Password)
                                .MustAsync(password => _PasswordComplexityVerifier.MeetsRequirements(password))
                                .WithState(u =>
                                    new ScimError(
                                        HttpStatusCode.BadRequest,
                                        ScimErrorType.InvalidValue,
                                        "The attribute 'password' does not meet the security requirements set by the provider."));
                        });
                });
            }

            public override Task<FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<User> context)
            {
                _UserId = context.InstanceToValidate.Id;

                return base.ValidateAsync(context);
            }

            private async Task<User> GetUser()
            {
                return await _UserRepository.GetUser(_UserId);
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

                if (string.IsNullOrWhiteSpace(multipleValueStringWithQuality)) return false;

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
                            if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
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
}