namespace Owin.Scim.Configuration
{
    public class ScimServerConfiguration
    {
        public ScimServerConfiguration()
        {
            RequireSsl = true;
        }

        public bool RequireSsl { get; set; }

        public string PublicOrigin { get; set; }
    }
}