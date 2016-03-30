namespace Owin.Scim.Endpoints
{
    using Configuration;

    public class SchemasController : ScimControllerBase
    {
        public SchemasController(ScimServerConfiguration scimServerConfiguration) 
            : base(scimServerConfiguration)
        {
        }
    }
}