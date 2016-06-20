namespace Owin.Scim.v2.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Querying;

    using Scim.Model;

    public class ScimListResponse2 : ScimListResponse
    {
        public ScimListResponse2(IEnumerable<Resource> resources)
        {
            Resources = resources == null ? new List<Resource>() : resources.ToList();
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Messages.ListResponse; }
        }
    }
}