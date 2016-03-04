namespace Owin.Scim.Model.Groups
{
    using System;
    using System.Collections.Generic;

    using Extensions;

    public class Group : Resource
    {
        public Group() : this(null)
        {
        }

        public Group(params Type[] withExtensionTypes)
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Group);

            if (withExtensionTypes != null)
            {
                foreach (var extensionType in withExtensionTypes)
                {
                    AddExtension(extensionType);
                }
            }
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.Group; }
        }

        public string DisplayName { get; set; }

        public IEnumerable<Member> Members { get; set; }

        public override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                DisplayName,
                Members =  Members.GetMultiValuedAttributeCollectionVersion()
            }.GetHashCode();
        }
    }
}