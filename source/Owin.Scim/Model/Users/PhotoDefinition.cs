namespace Owin.Scim.Model.Users
{
    using System;

    public class PhotoDefinition : MultiValuedAttributeDefinition
    {
        public PhotoDefinition()
        {
            For(p => p.Value)
                .AddCanonicalizationRule(value => value.ToLower());
        }

        public override Type DefinitionType
        {
            get { return typeof(Photo); }
        }
    }
}