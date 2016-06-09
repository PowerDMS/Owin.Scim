namespace Owin.Scim.Model.Users
{
    using System;
    using System.Collections.Generic;

    using Extensions;

    using Newtonsoft.Json;
    
    public class ScimUser : Resource
    {
        public ScimUser()
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.User);
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.User; }
        }
        
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public Name Name { get; set; }
        
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        
        [JsonProperty(PropertyName = "nickName")]
        public string NickName { get; set; }
        
        [JsonProperty(PropertyName = "profileUrl")]
        public Uri ProfileUrl { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "userType")]
        public string UserType { get; set; }
        
        [JsonProperty(PropertyName = "preferredLanguage")]
        public string PreferredLanguage { get; set; }
        
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }
        
        [JsonProperty(PropertyName = "timezone")]
        public string Timezone { get; set; }
        
        [JsonProperty(PropertyName = "active")]
        public bool Active { get; set; }
        
        public string Password { get; set; }
        
        [JsonProperty(PropertyName = "emails")]
        public IEnumerable<Email> Emails { get; set; }
        
        [JsonProperty(PropertyName = "phoneNumbers")]
        public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
        
        [JsonProperty(PropertyName = "ims")]
        public IEnumerable<InstantMessagingAddress> Ims { get; set; }
        
        [JsonProperty(PropertyName = "photos")]
        public IEnumerable<Photo> Photos { get; set; }
        
        [JsonProperty(PropertyName = "addresses")]
        public IEnumerable<MailingAddress> Addresses { get; set; }
        
        [JsonProperty(PropertyName = "groups")]
        public IEnumerable<UserGroup> Groups { get; set; }
        
        [JsonProperty(PropertyName = "entitlements")]
        public IEnumerable<Entitlement> Entitlements { get; set; }
        
        [JsonProperty(PropertyName = "roles")]
        public IEnumerable<Role> Roles { get; set; }
        
        [JsonProperty(PropertyName = "x509Certificates")]
        public IEnumerable<X509Certificate> X509Certificates { get; set; }
        
        public override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                Active,
                Locale,
                Name = Name == null ? 0 : Name.CalculateVersion(),
                NickName,
                DisplayName,
                Password,
                PreferredLanguage,
                ProfileUrl,
                Timezone,
                Title,
                UserName,
                UserType,
                Addresses = Addresses.GetMultiValuedAttributeCollectionVersion(),
                Emails = Emails.GetMultiValuedAttributeCollectionVersion(),
                Entitlements = Entitlements.GetMultiValuedAttributeCollectionVersion(),
                Groups = Groups.GetMultiValuedAttributeCollectionVersion(),
                Ims = Ims.GetMultiValuedAttributeCollectionVersion(),
                PhoneNumbers = PhoneNumbers.GetMultiValuedAttributeCollectionVersion(),
                Photos = Photos.GetMultiValuedAttributeCollectionVersion(),
                Roles = Roles.GetMultiValuedAttributeCollectionVersion(),
                X509Certificates = X509Certificates.GetMultiValuedAttributeCollectionVersion()
            }.GetHashCode();
        }
    }
}