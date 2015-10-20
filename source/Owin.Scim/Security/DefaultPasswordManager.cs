namespace Owin.Scim.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class DefaultPasswordManager : IManagePasswords
    {
        public string CreateHash(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            using (var cryptoProvider = new MD5Cng())
            {
                return
                    Encoding.UTF8.GetString(
                        cryptoProvider.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword)));
            }
        }

        public bool VerifyHash(string plainTextPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("passwordHash");

            return CreateHash(plainTextPassword).Equals(passwordHash, StringComparison.Ordinal);
        }
    }
}