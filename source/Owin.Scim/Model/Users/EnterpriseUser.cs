namespace Owin.Scim.Model.Users
{
    public class EnterpriseUser : User
    {
        public EnterpriseUser()
        {
            AddSchema(ScimConstants.Schemas.UserEnterprise);
        }

        public string EmployeeNumber { get; set; }

        public string CostCenter { get; set; }

        public string Organization { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public Manager Manager { get; set; }
    }
}