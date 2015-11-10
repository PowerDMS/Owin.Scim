namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

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
}