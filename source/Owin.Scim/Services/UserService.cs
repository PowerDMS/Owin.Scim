namespace Owin.Scim.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using Configuration;

    using ErrorHandling;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    using NContext.Extensions;

    using Repository;

    using Security;

    using Validation;
    using Validation.Users;

    public class UserService : IUserService
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        private readonly UserValidator _UserValidator;
        
        protected delegate void CanonicalizationRule<in T>(T attribute, ref Object state) where T : MultiValuedAttribute;

        public UserService(
            ScimServerConfiguration serverConfiguration,
            IUserRepository userRepository,
            IManagePasswords passwordManager,
            UserValidator userValidator)
        {
            _ServerConfiguration = serverConfiguration;
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
            _UserValidator = userValidator;
        }

        public async Task<IScimResponse<User>> CreateUser(User user)
        {
            await CanonicalizeUser(user);

            var newUser = Mapper.Map(user, new User()); // Replace all new User metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(newUser, RuleSetConstants.Create);

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);
            
            var userRecord = await _UserRepository.CreateUser(user);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> RetrieveUser(string userId)
        {
            var userRecord = await _UserRepository.GetUser(userId);
            if (userRecord == null)
                return new ScimErrorResponse<User>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(userId)));

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null) return null;

            await CanonicalizeUser(user);

            Mapper.Map(user, userRecord); // Replace all userRecord metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(userRecord, RuleSetConstants.Update);

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);

            if (!string.IsNullOrWhiteSpace(userRecord.Password))
            {
                userRecord.Password = _PasswordManager.CreateHash(
                    Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
            }

            await _UserRepository.UpdateUser(userRecord);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<Unit>> DeleteUser(string userId)
        {
            var result = await _UserRepository.DeleteUser(userId);
            if (result == null)
                return new ScimErrorResponse<Unit>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(userId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }

        protected virtual Task CanonicalizeUser(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Locale))
            {
                user.Locale = user.Locale.Replace('_', '-'); // Supports backwards compatability
            }

            // TODO: (DG) Create generic canonicalization rule for supporting ScimServerConfig / Canon Types per attribute.

            // ADDRESSES
            CanonicalizeMultiValueAttributes(user.Addresses,
                ((Address attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // CERTIFICATES
            CanonicalizeMultiValueAttributes(user.X509Certificates,
                ((X509Certificate attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // EMAILS
            CanonicalizeMultiValueAttributes(user.Emails,
                ((Email attribute, ref object state) =>
                {
                    if (string.IsNullOrWhiteSpace(attribute.Value)) return;

                    var atIndex = attribute.Value.IndexOf('@') + 1;
                    if (atIndex == 0) return; // IndexOf returned -1

                    var cEmail = attribute.Value.Substring(0, atIndex) + attribute.Value.Substring(atIndex).ToLower();
                    attribute.Value = cEmail;

                    if (!string.IsNullOrWhiteSpace(attribute.Display))
                    {
                        atIndex = attribute.Display.IndexOf('@') + 1;
                        if (atIndex == 0) return; // IndexOf returned -1

                        cEmail = attribute.Display.Substring(0, atIndex) + attribute.Display.Substring(atIndex).ToLower();
                        attribute.Display = cEmail;
                    }
                }),
                ((Email attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // ENTITLEMENTS
            CanonicalizeMultiValueAttributes(user.Entitlements,
                ((Entitlement attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // INSTANT MESSAGE ADDRESSES
            CanonicalizeMultiValueAttributes(user.Ims,
                ((InstantMessagingAddress attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // PHONE NUMBERS
            CanonicalizeMultiValueAttributes(user.PhoneNumbers,
                ((PhoneNumber attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // PHOTOS
            CanonicalizeMultiValueAttributes(user.Photos,
                ((Photo attribute, ref object state) => LowercaseCanonicalization(attribute, photo => photo.Value)),
                ((Photo attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));

            // ROLES
            CanonicalizeMultiValueAttributes(user.Roles,
                ((Role attribute, ref object state) => EnforceSinglePrimaryAttribute(attribute, ref state)));
            
            return Task.FromResult(0);
        }

        private void CanonicalizeMultiValueAttributes<T>(
            IEnumerable<T> attributes, 
            params CanonicalizationRule<T>[] canonicalizationRules)
            where T : MultiValuedAttribute
        {
            if (attributes == null || !attributes.Any()) return;

            var stateCache = new Dictionary<CanonicalizationRule<T>, object>();
            attributes.ForEach(attribute =>
            {
                if (attribute == null) return;

                canonicalizationRules.ForEach(rule =>
                {
                    if (!stateCache.ContainsKey(rule))
                        stateCache[rule] = null;

                    var state = stateCache[rule];
                    rule(attribute, ref state);
                    stateCache[rule] = state;
                });
            });
        }

        protected void LowercaseCanonicalization<T, TProperty>(T attribute, Expression<Func<T, TProperty>> expression)
            where T : MultiValuedAttribute
            where TProperty : class, IComparable, ICloneable, IConvertible, IEnumerable
        {
            var mE = expression.Body as MemberExpression;
            if (mE == null) throw new InvalidOperationException("Expression body must be a MemberExpression to an attribute's string property.");

            var pI = mE.Member as PropertyInfo;
            if (pI == null) throw new InvalidOperationException("Expression body member must be an attribute's string property.");

            var value = pI.GetValue(attribute) as string;

            if (string.IsNullOrWhiteSpace(value)) return;

            pI.SetValue(attribute, value.ToLower());
        }

        protected void EnforceSinglePrimaryAttribute(MultiValuedAttribute attribute, ref Object state)
        {
            bool hasPrimary = false;
            if (state != null)
                hasPrimary = (bool) state;

            if (!hasPrimary && attribute.Primary)
            {
                state = true;
            }

            if (hasPrimary && attribute.Primary)
            {
                attribute.Primary = false;
            }
        }
    }
}