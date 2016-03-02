namespace Owin.Scim.Model.Users
{
    [SchemaIdentifier(ScimConstants.Schemas.UserEnterprise)]
    public class EnterpriseUserExtension : ResourceExtension
    {
        public string EmployeeNumber { get; set; }

        public string CostCenter { get; set; }

        public string Organization { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public Manager Manager { get; set; }

        public override int CalculateVersion()
        {
            return new
            {
                CostCenter,
                Department,
                Division,
                EmployeeNumber,
                Organization,
                Manager?.Value
            }.GetHashCode();
        }
    }
}