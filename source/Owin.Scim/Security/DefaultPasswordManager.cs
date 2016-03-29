namespace Owin.Scim.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using NContext.Extensions;
    using NContext.Security.Cryptography;

    public class DefaultPasswordManager : IManagePasswords
    {
        private readonly IEnumerable<Regex> _ComplexityRules = new[]
        {
            new Regex(@"[a-z]", RegexOptions.Compiled | RegexOptions.Singleline),
            new Regex(@"[A-Z]", RegexOptions.Compiled | RegexOptions.Singleline),
            new Regex(@"[0-9]", RegexOptions.Compiled | RegexOptions.Singleline)
        };

        protected virtual int SaltLength
        {
            get { return 32; }
        }

        protected virtual int IterationCount
        {
            get { return 1000; }
        }

        public virtual string CreateHash(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            return CreateHashBytes(new UTF8Encoding(false).GetBytes(plainTextPassword)).ToHexadecimal();
        }

        protected virtual byte[] CreateHashBytes(byte[] plainTextPassword)
        {
            var salt = new byte[SaltLength];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(plainTextPassword, salt, IterationCount);
            var hash = rfc2898DeriveBytes.GetBytes(20);

            return CryptographyUtility.CombineBytes(salt, hash);
        }

        public bool VerifyHash(string plainTextPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("passwordHash");
            
            var passwordHashBytes = passwordHash.ToBytesFromHexadecimal();

            byte[] saltBytes = CryptographyUtility.GetBytes(passwordHashBytes, SaltLength);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(plainTextPassword, saltBytes, IterationCount);
            var plainTextHashBytes = rfc2898DeriveBytes.GetBytes(20);
            byte[] saltedPlainTextHashBytes = CryptographyUtility.CombineBytes(saltBytes, plainTextHashBytes);

            return CryptographyUtility.CompareBytes(passwordHashBytes, saltedPlainTextHashBytes);
        }

        public virtual Task<bool> MeetsRequirements(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword)) return Task.FromResult(false);
            
            return Task.FromResult(_ComplexityRules.All(x => x.IsMatch(plainTextPassword)));
        }

        public bool PasswordIsDifferent(string plainTextPassword, string existingPasswordHash)
        {
            if (plainTextPassword == null && existingPasswordHash == null) return false;

            if (plainTextPassword == null || existingPasswordHash == null) return true;

            return !VerifyHash(plainTextPassword, existingPasswordHash);
        }
    }
}