namespace Owin.Scim.v2.Model
{
    using Scim.Model.Users;

    public class ScimUser2 : ScimUser
    {
        public override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Schemas.User; }
        }
    }
}