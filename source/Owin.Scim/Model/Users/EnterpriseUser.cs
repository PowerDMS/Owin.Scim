namespace Owin.Scim.Model.Users
{
    using Newtonsoft.Json;

    public class EnterpriseUser : User
    {
        public EnterpriseUser()
        {
            AddSchema(ScimConstants.Schemas.UserEnterprise);
            Enterprise = new EnterpriseUserExtension();
        }

        [JsonProperty(ScimConstants.Schemas.UserEnterprise)]
        public EnterpriseUserExtension Enterprise { get; set; }
    }
}