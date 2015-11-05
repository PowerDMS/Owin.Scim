namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

    using Model;

    public class PatchRequest<T> where T : Resource
    {
        public IEnumerable<string> Schemas { get; set; }

        public JsonPatchDocument<T> Operations { get; set; }
    }
}