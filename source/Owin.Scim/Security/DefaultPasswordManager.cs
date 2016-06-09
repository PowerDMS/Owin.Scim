namespace Owin.Scim.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using NContext.Extensions;
    using NContext.Security.Cryptography;

    /// <summary>
    /// The default behavior of this class uses <see cref="Rfc2898DeriveBytes"/> to create and verify password hashes.
    /// </summary>
    public class DefaultPasswordManager : IManagePasswords
    {
        /// <summary>
        /// Default password complexity requires a minimum of: 8 characters; 1 uppercase, 1 lowercase, 1 number, and 1 special character.
        /// </summary>
        private readonly Regex _DefaultComplexityRule =
            new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Gets the length of the password salt. Defaults to 32 bytes.
        /// </summary>
        protected virtual int SaltLength
        {
            get { return 32; }
        }

        /// <summary>
        /// Returns the hashed value as hexidecimal.
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        public virtual string CreateHash(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            return CreateHashBytes(new UTF8Encoding(false).GetBytes(plainTextPassword)).ToHexadecimal();
        }

        /// <summary>
        /// Creates the hash bytes.
        /// </summary>
        /// <param name="plainTextPassword">The plain text password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>System.Byte[].</returns>
        protected virtual byte[] CreateHashBytes(byte[] plainTextPassword, byte[] salt = null)
        {
            if (salt == null)
            {
                salt = new byte[SaltLength];
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetNonZeroBytes(salt);
                }
            }

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(plainTextPassword, salt, 1000);
            var hash = rfc2898DeriveBytes.GetBytes(20);

            return CryptographyUtility.CombineBytes(salt, hash);
        }

        /// <summary>
        /// Verifies the hash of the <paramref name="plainTextPassword" /> matches the specified <paramref name="passwordHash" />.
        /// </summary>
        /// <param name="plainTextPassword">The password to verify.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns>Whether or not the password is the same.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// plainTextPassword
        /// or
        /// passwordHash
        /// </exception>
        public bool VerifyHash(string plainTextPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("passwordHash");
            
            var passwordHashBytes = passwordHash.ToBytesFromHexadecimal();
            var saltBytes = CryptographyUtility.GetBytes(passwordHashBytes, SaltLength);
            var saltedPlainTextHashBytes = CreateHashBytes(new UTF8Encoding(false).GetBytes(plainTextPassword), saltBytes);

            return CryptographyUtility.CompareBytes(passwordHashBytes, saltedPlainTextHashBytes);
        }

        /// <summary>
        /// Default password complexity requires a minimum of: 8 characters; 1 uppercase, 1 lowercase, 1 number, and 1 special character.
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        public virtual Task<bool> MeetsRequirements(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword)) return Task.FromResult(false);
            
            return Task.FromResult(_DefaultComplexityRule.IsMatch(plainTextPassword));
        }

        /// <summary>
        /// Checks to see whether <paramref name="plainTextPassword" /> is different than the <paramref name="existingPasswordHash" /> including checks for null.
        /// </summary>
        /// <param name="plainTextPassword">The plain text password.</param>
        /// <param name="existingPasswordHash">The existing password hash.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool PasswordIsDifferent(string plainTextPassword, string existingPasswordHash)
        {
            if (plainTextPassword == null && existingPasswordHash == null) return false;

            if (plainTextPassword == null || existingPasswordHash == null) return true;

            return !VerifyHash(plainTextPassword, existingPasswordHash);
        }
    }
}