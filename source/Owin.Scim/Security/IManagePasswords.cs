namespace Owin.Scim.Security
{
    using System.Threading.Tasks;

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

        /// <summary>
        /// Validates the specified <paramref name="plainTextPassword"/> meets the complexity requirements set by the server.
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <returns></returns>
        Task<bool> MeetsRequirements(string plainTextPassword);

        /// <summary>
        /// Checks to see whether <paramref name="plainTextPassword"/> is different than the <paramref name="existingPasswordHash"/> including checks for null.
        /// </summary>
        /// <param name="plainTextPassword"></param>
        /// <param name="existingPasswordHash"></param>
        /// <returns></returns>
        bool PasswordIsDifferent(string plainTextPassword, string existingPasswordHash);
    }
}