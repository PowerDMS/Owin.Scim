namespace Owin.Scim.Model.Users
{
    using System.ComponentModel;

    using Newtonsoft.Json;

    [ScimTypeDefinition(typeof(MailingAddressDefinition))]
    public class MailingAddress : MultiValuedAttribute
    {
        [Description(@"The full mailing address, formatted for display or use with a mailing label. This attribute MAY contain newlines.")]
        [JsonProperty("formatted")]
        public string Formatted { get; set; }

        [Description(@"
        The full street address component, which may include house number, 
        street name, P.O. box, and multi-line extended street address 
        information. This attribute MAY contain newlines.")]
        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [Description(@"The city or locality component.")]
        [JsonProperty("locality")]
        public string Locality { get; set; }

        [Description(@"The state or region component.")]
        [JsonProperty("region")]
        public string Region { get; set; }

        [Description(@"The zip code or postal code component.")]
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [Description(@"The country name component.")]
        [JsonProperty("country")]
        public string Country { get; set; }

        protected internal override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                Formatted,
                StreetAddress,
                Locality,
                Region,
                PostalCode,
                Country
            }.GetHashCode();
        }
    }
}