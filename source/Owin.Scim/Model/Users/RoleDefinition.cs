namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class RoleDefinition : MultiValuedAttributeDefinition
    {
        public RoleDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(e => e.Value)
                .SetDescription("The value of a role.");
        }

        public override Type DefinitionType
        {
            get { return typeof(Role); }
        }
    }
}