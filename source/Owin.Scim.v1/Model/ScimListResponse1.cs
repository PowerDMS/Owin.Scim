namespace Owin.Scim.v1.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Querying;

    using Scim.Model;

    public class ScimListResponse1 : ScimListResponse
    {
        public ScimListResponse1(IEnumerable<Resource> resources)
        {
            Resources = resources == null ? new List<Resource>() : resources.ToList();
        }

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Messages.ListResponse; }
        }
    }
}