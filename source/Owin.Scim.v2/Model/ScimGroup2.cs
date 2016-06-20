namespace Owin.Scim.v2.Model
{
    using Scim.Model.Groups;

    public class ScimGroup2 : ScimGroup
    {
        public override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Schemas.Group; }
        }
    }
}