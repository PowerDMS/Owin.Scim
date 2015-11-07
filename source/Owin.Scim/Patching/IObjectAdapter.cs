namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

    using Operations;

    /// <summary>
    /// Defines the operations that can be performed on a JSON patch document.
    /// </summary>  
    public interface IObjectAdapter    
    {
        IEnumerable<PatchOperationResult> Add(Operation operation, object objectToApplyTo);

        IEnumerable<PatchOperationResult> Remove(Operation operation, object objectToApplyTo);

        IEnumerable<PatchOperationResult> Replace(Operation operation, object objectToApplyTo);
    }
}