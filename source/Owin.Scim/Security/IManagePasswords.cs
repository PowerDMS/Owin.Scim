namespace Owin.Scim.Security
{
    public interface IManagePasswords
    {
        /// <summary>
        /// Creates a hash from the plain text password specified.
        /// </summary>
        /// <param name="plainTextPassword">The plain text password.</param>
        /// <returns>Ciphertext hash of the .</returns>
        string CreateHash(string plainTextPassword);

        /// <summary>
        /// Verifies the hash of the <paramref name="plainTextPassword"/> matches the specified <paramref name="passwordHash"/>.
        /// </summary>
        /// <param name="plainTextPassword">The password to verify.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns>Whether or not the password is the same. </returns>
        bool VerifyHash(string plainTextPassword, string passwordHash);
    }
}