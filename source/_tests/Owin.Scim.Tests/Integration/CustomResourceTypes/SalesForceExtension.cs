namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using Model;

    using Newtonsoft.Json;

    public class SalesForceExtension : ResourceExtension
    {
        [JsonProperty("customerIdentifier")]
        public string CustomerIdentifier { get; set; }

        protected internal override string SchemaIdentifier
        {
            get { return CustomSchemas.SalesForceExtension; }
        }

        public override int CalculateVersion()
        {
            if (CustomerIdentifier == null)
                return 0;

            return CustomerIdentifier.GetHashCode();
        }
    }
}