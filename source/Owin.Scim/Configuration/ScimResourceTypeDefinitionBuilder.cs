namespace Owin.Scim.Configuration
{
    using Model;

    public class ScimResourceTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>
        where T : Resource
    {
        private readonly string _Endpoint;

        private readonly string _Name;

        private readonly string _Schema;

        public ScimResourceTypeDefinitionBuilder(
            ScimServerConfiguration configuration, 
            string name, 
            string schema, 
            string endpoint) 
            : base(configuration)
        {
            _Name = name;
            _Schema = schema;

            if (!endpoint.StartsWith("/"))
            {
                endpoint = endpoint.Insert(0, "/");
            }

            _Endpoint = endpoint;
        }
    }
}