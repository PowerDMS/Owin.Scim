namespace Owin.Scim.Tests.Security
{
    using Machine.Specifications;

    using Scim.Security;

    public class when_hashing_passwords
    {
        Because of = () => Result = PasswordManager.VerifyHash(PlainText, CypherText);

        protected static IManagePasswords PasswordManager;

        protected static string PlainText;

        protected static string CypherText;

        protected static bool Result;
    }
}