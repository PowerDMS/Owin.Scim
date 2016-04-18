namespace Owin.Scim.Model.Users
{
    [ScimTypeDefinition(typeof(X509CertificateDefinition))]
    public class X509Certificate : MultiValuedAttribute
    {
        public new byte[] Value { get; set; }
    }
}