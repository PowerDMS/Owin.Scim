namespace Owin.Scim.Patching
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class PatchResult : List<PatchOperation>
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

    public class PatchOperation
    {
        public PatchOperation(Expression path, object oldValue, object  newValue)
        {
            
        }
    }
}