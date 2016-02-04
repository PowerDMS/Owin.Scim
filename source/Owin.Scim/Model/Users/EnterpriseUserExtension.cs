namespace Owin.Scim.Model.Users
{
    public class EnterpriseUserExtension
    {
        public string EmployeeNumber { get; set; }

        public string CostCenter { get; set; }

        public string Organization { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public Manager Manager { get; set; }
    }
}