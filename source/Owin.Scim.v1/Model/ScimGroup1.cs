namespace Owin.Scim.v1.Model
{
    using Scim.Model.Groups;

    public class ScimGroup1 : ScimGroup
    {
        public override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Schemas.Group; }
        }
    }
}