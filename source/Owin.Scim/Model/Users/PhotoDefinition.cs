namespace Owin.Scim.Model.Users
{
    public class PhotoDefinition : MultiValuedAttributeDefinition
    {
        public PhotoDefinition()
        {
            For(p => p.Value)
                .AddCanonicalizationRule(value => value.ToLower());
        }
    }
}