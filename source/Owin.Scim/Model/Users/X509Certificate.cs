namespace Owin.Scim.Model.Users
{
    public class X509Certificate : MultiValuedAttribute
    {
        public new byte[] Value { get; set; }
    }
}