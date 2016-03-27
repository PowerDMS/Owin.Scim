namespace Owin.Scim.Model.Users
{
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
                EmployeeNumber,
                CostCenter,
                Organization,
                Division,
                Department,
                Manager?.Value
            }.GetHashCode();
        }
    }
}