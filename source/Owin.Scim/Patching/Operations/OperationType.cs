namespace Owin.Scim.Patching.Operations
{
    public enum OperationType
    {
        /// <summary>
        /// This is the default value when it fails to de-serialize
        /// </summary>
        Invalid,
        Add,
        Remove,
        Replace
    }
}