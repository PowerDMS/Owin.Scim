namespace Owin.Scim.Patching
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Helpers;

    using Newtonsoft.Json.Serialization;

    public class PatchResult : List<PatchOperationResult>
    {
        public int HttpStatusCode
        {
            get
            {
                if (Count == 0)
                {
                    return 204;
                }

                return 200;
            }
        }
    }

    public class PatchOperationResult
    {
        public PatchOperationResult(JsonPatchProperty property, object oldValue, object  newValue)
        {
            
        }
    }
}