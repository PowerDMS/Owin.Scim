namespace Owin.Scim.Patching
{
    using Helpers;

    public class PatchOperationResult
    {
        public PatchOperationResult(JsonPatchProperty property, object oldValue, object  newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public JsonPatchProperty Property { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public bool HasChanged
        {
            get { return OldValue != NewValue; }
        }
    }
}