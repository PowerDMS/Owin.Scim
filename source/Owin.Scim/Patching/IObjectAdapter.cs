namespace Owin.Scim.Patching
{
    using System.Collections.Generic;

    using Operations;

    /// <summary>
    /// Defines the operations that can be performed on a JSON patch document.
    /// </summary>  
    public interface IObjectAdapter    
    {
        IEnumerable<PatchOperation> Add(Operation operation, object objectToApplyTo);

        IEnumerable<PatchOperation> Remove(Operation operation, object objectToApplyTo);

        IEnumerable<PatchOperation> Replace(Operation operation, object objectToApplyTo);
    }
}