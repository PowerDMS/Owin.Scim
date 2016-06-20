namespace Owin.Scim.v1.Model
{
    using Scim.Model.Users;

    public class ScimUser1 : ScimUser
    {
        public override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Schemas.User; }
        }
    }
}