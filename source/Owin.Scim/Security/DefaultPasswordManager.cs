namespace Owin.Scim.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using NContext.Extensions;
    using NContext.Security.Cryptography;

    public class DefaultPasswordManager : IManagePasswords
    {
        private readonly IProvideHashing _HashProvider;

        public DefaultPasswordManager(IProvideHashing hashProvider)
        {
            _HashProvider = hashProvider;
        }

        public string CreateHash(string plainTextPassword)
        {
            return CreateHashBytes(plainTextPassword).ToHexadecimal();
        }

        private byte[] CreateHashBytes(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            /* Before comparing or evaluating the uniqueness of a "userName" or 
               "password" attribute, service providers MUST use the preparation, 
               enforcement, and comparison of internationalized strings (PRECIS) 
               preparation and comparison rules described in Sections 3 and 4, 
               respectively, of [RFC7613], which is based on the PRECIS framework
               specification [RFC7564]. */

            return _HashProvider.CreateHash<MD5Cng>(Encoding.UTF8.GetBytes(plainTextPassword), 16);
        }

        public bool VerifyHash(string plainTextPassword, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentNullException("plainTextPassword");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("passwordHash");

            /* Before comparing or evaluating the uniqueness of a "userName" or 
               "password" attribute, service providers MUST use the preparation, 
               enforcement, and comparison of internationalized strings (PRECIS) 
               preparation and comparison rules described in Sections 3 and 4, 
               respectively, of [RFC7613], which is based on the PRECIS framework
               specification [RFC7564]. */

            return _HashProvider.CompareHash<MD5Cng>(
                Encoding.UTF8.GetBytes(plainTextPassword),
                passwordHash.ToBytesFromHexadecimal());
        }
    }
}