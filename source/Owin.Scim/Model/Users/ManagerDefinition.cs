namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class ManagerDefinition : ScimTypeDefinitionBuilder<Manager>
    {
        public ManagerDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(m => m.Value)
                .SetDescription(@"The ""id"" of the SCIM resource representing the user's manager.");

            For(m => m.Ref)
                .SetDescription(@"The URI of the SCIM resource representing the user's manager.");

            For(m => m.DisplayName)
                .SetDescription(@"The displayName of the user's manager.")
                .SetMutability(Mutability.ReadOnly);
        }
    }
}