namespace Owin.Scim.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Integration;

    public static class HttpContentExtensions
    {
        public static Task<T> ScimReadAsAsync<T>(this HttpContent content)
        {
            return content.ReadAsAsync<T>(using_a_scim_server.ClientJsonFormatter.AsEnumerable);
        }
    }
}