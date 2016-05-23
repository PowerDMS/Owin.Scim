namespace ConsoleHost.Scim
{
    using Owin.Scim.Configuration;
    using Owin.Scim.Model.Users;

    public class CustomUserDefinition : UserDefinition
    {
        public CustomUserDefinition(ScimServerConfiguration serverConfiguration) 
            : base(serverConfiguration)
        {
            For(u => u.NickName)
                .SetRequired(true)
                .AddCanonicalizationRule(nickName => nickName.ToUpper());

            SetValidator<CustomUserValidator>();
        }
    }
}