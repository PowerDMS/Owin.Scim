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

        public override string GenerateETagHash()
        {
            unchecked
            {
                var hash = 19;
                hash = hash * 31 + GenerateETagHashInternal();
                hash = hash * 31 + (Enterprise == null
                    ? 0
                    : new
                    {
                        Enterprise.CostCenter,
                        Enterprise.Department,
                        Enterprise.Division,
                        Enterprise.EmployeeNumber,
                        Enterprise.Organization,
                        Enterprise.Manager?.Value
                    }.GetHashCode());

                return hash.ToString();
            }
        }
    }
}