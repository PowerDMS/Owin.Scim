namespace Owin.Scim.Tests
{
    using System.Net.Http;

    using Integration;

    public class ScimObjectContent<T> : ObjectContent<T>
    {
        public ScimObjectContent(T value) 
            : base(value, using_a_scim_server.ClientJsonFormatter)
        {
        }
    }
}