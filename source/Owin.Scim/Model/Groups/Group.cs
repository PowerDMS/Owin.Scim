namespace Owin.Scim.Model.Groups
{
    using System.Collections.Generic;

    using Extensions;

    public class Group : Resource
    {
        public Group()
        {
            AddSchema(ScimConstants.Schemas.Group);
        }

        public string DisplayName { get; set; }

        public IEnumerable<Member> Members { get; set; }

        public override string CalculateVersion()
        {
            return CalculateVersionInternal().ToString();
        }

        protected int CalculateVersionInternal()
        {
            return new
            {
                ExternalId,
                Id,
                DisplayName,
                // TODO: add hash for Members
            }.GetHashCode();
        }
    }
}