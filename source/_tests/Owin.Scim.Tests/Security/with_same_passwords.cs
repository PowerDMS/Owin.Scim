namespace Owin.Scim.Tests.Security
{
    using Machine.Specifications;

    using Scim.Security;

    public class with_same_passwords : when_hashing_passwords
    {
        Establish context = () =>
        {
            PasswordManager = new DefaultPasswordManager();
            PlainText = "password";
            CypherText = PasswordManager.CreateHash(PlainText);
        };

        It should_return_true = () => Result.ShouldEqual(true);
    }
}