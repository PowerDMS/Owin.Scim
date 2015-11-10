namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

    using Helpers;

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
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public bool HasChanged
        {
            get { return OldValue != NewValue; }
        }
    }
}