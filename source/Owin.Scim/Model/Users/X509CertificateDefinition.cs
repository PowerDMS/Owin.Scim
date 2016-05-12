namespace Owin.Scim.Model.Users
{
    using Canonicalization;
    using Configuration;

    public class X509CertificateDefinition : ScimTypeDefinitionBuilder<X509Certificate>
    {
        public X509CertificateDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(cert => cert.Value)
                .SetDescription(@"The value of an X.509 certificate.");

            For(cert => cert.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}