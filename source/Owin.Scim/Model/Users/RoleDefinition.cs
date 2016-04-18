namespace Owin.Scim.Model.Users
{
    using System;

    public class RoleDefinition : MultiValuedAttributeDefinition
    {
        public RoleDefinition()
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