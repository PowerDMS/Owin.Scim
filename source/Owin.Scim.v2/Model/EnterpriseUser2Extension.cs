namespace Owin.Scim.v2.Model
{
    using Scim.Model.Users;

    public class EnterpriseUser2Extension : EnterpriseUserExtension
    {
        protected override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Schemas.UserEnterprise; }
        }
    }
}