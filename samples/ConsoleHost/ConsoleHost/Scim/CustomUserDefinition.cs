namespace ConsoleHost.Scim
{
    using Owin.Scim.Configuration;
    using Owin.Scim.v2.Model;

    public class CustomUserDefinition : ScimUser2Definition
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