namespace Owin.Scim.Patching
{
    using Operations;

    /// <summary>
    /// Defines the operations that can be performed on a JSON patch document.
    /// </summary>  
    public interface IObjectAdapter    
    {
        void Add(Operation operation, object objectToApplyTo);

        void Remove(Operation operation, object objectToApplyTo);

        void Replace(Operation operation, object objectToApplyTo);
    }
}