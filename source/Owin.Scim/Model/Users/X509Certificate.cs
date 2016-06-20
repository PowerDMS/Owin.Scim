namespace Owin.Scim.Model.Users
{
    using Newtonsoft.Json;

    public class X509Certificate : MultiValuedAttribute
    {
        [JsonProperty("value")]
        public new byte[] Value { get; set; }
    }
}