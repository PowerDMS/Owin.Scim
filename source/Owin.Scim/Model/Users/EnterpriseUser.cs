namespace Owin.Scim.Model.Users
{
    using System.Collections.Generic;

    public class EnterpriseUser : User
    {
        public override ISet<string> Schemas
        {
            get
            {
                return new HashSet<string>
                {
                    ScimConstants.Schemas.User,
                    ScimConstants.Schemas.UserEnterprise
                };
            }
        }

        public string EmployeeNumber { get; set; }

        public string CostCenter { get; set; }

        public string Organization { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public Manager Manager { get; set; }
    }
}