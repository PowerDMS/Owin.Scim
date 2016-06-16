namespace Owin.Scim.Model.Users
{
    using System.ComponentModel;

    using Newtonsoft.Json;

    [Description("Enterprise user.")]
    public abstract class EnterpriseUserExtension : ResourceExtension
    {
        [JsonProperty("employeeNumber")]
        public string EmployeeNumber { get; set; }

        [JsonProperty("costCenter")]
        public string CostCenter { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("division")]
        public string Division { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("manager")]
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
                Manager = Manager == null ? null : Manager.Value
            }.GetHashCode();
        }
    }
}