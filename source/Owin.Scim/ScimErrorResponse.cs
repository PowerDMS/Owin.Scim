namespace Owin.Scim
{
    using Model;

    public class ScimErrorResponse<T> : ScimResponse<T>
    {
        private readonly ScimError _Error;

        public ScimErrorResponse(ScimError error)
        {
            _Error = error;
        }

        /// <summary>
        /// Gets whether the instance is left. 
        /// </summary> 
        /// <value>
        /// The is left.
        /// </value>
        public override bool IsLeft => true;

        /// <summary>
        /// Gets the error. 
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ScimError Error => _Error;

        /// <summary>
        /// Gets the left value. 
        /// </summary> 
        /// <returns>
        /// ScimError.
        /// </returns>
        public override ScimError GetLeft()
        {
            return _Error;
        }

        /// <summary>
        /// Gets the right value. 
        /// </summary> 
        /// <returns>
        /// T.
        /// </returns>
        public override T GetRight()
        {
            return default(T);
        }
    }
}