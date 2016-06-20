namespace Owin.Scim.v1.Model
{
    using Scim.Model.Users;

    public class EnterpriseUser1Extension : EnterpriseUserExtension
    {
        protected override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Schemas.UserEnterprise; }
        }
    }
}