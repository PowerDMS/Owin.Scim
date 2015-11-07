namespace Owin.Scim.Patching.Helpers
{
    public class PatchMember
    {
        public PatchMember(
            string propertyPathInParent, 
            JsonPatchProperty jsonPatchProperty,
            object target = null)
        {
            Target = target;
            PropertyPathInParent = propertyPathInParent;
            JsonPatchProperty = jsonPatchProperty;
        }

        public string PropertyPathInParent { get; private set; }

        public JsonPatchProperty JsonPatchProperty { get; private set; }

        public object Target { get; private set; }
    }
}