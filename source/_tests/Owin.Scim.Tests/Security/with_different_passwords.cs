namespace Owin.Scim.Tests.Security
{
    using Machine.Specifications;

    using Scim.Security;

    public class with_different_passwords : when_hashing_passwords
    {
        Establish context = () =>
        {
            PasswordManager = new DefaultPasswordManager();
            PlainText = "invalidpassword";
            CypherText = PasswordManager.CreateHash("password");
        };

        It should_return_true = () => Result.ShouldEqual(false);
    }
}