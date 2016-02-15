namespace Owin.Scim.Model.Users
{
    public class Address : MultiValuedAttribute
    {
        public string Formatted { get; set; }

        public string StreetAddress { get; set; }

        public string Locality { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        protected internal override int GetETagHashCode()
        {
            unchecked
            {
                var hash = 19;
                hash = hash * 31 + base.GetETagHashCode();
                hash = hash * 31 + new
                {
                    Formatted,
                    StreetAddress,
                    Locality,
                    Region,
                    PostalCode,
                    Country
                }.GetHashCode();

                return hash;
            }
        }
    }
}