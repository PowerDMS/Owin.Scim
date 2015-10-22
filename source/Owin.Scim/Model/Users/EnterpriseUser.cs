namespace Owin.Scim.Model.Users
{
    using System.Collections.Generic;

    public class EnterpriseUser : User
    {
        private IEnumerable<string> _Schemas;

        public EnterpriseUser()
        {
            _Schemas = new List<string>
                {
                    ScimConstants.Schemas.User,
                    ScimConstants.Schemas.UserEnterprise
                };
        }

        public override IEnumerable<string> Schemas
        {
            get { return _Schemas; }
            set { _Schemas = value; }
        }

        public string EmployeeNumber { get; set; }

        public string CostCenter { get; set; }

        public string Organization { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public Manager Manager { get; set; }
    }
}