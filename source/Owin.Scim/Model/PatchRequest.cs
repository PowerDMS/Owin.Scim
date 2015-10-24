namespace Owin.Scim.Model
{
    using System.Collections.Generic;

    using Marvin.JsonPatch;

    public class PatchRequest<T> where T : Resource
    {
        public IEnumerable<string> Schemas { get; set; }

        public JsonPatchDocument<T> Operations { get; set; }
    }
}