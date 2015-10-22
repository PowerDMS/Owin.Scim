namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class ScimErrorResponse<T> : ScimResponse<T>
    {
        private readonly IEnumerable<ScimError> _Errors;

        public ScimErrorResponse(ScimError error)
            : this(new[] { error })
        {
        }

        public ScimErrorResponse(IEnumerable<ScimError> errors)
        {
            _Errors = errors.ToList();
        }

        /// <summary>
        /// Gets whether the instance is left. 
        /// </summary> 
        /// <value>
        /// The is left.
        /// </value>
        public override bool IsLeft
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the error. 
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public IEnumerable<ScimError> Errors
        {
            get
            {
                return _Errors;
            }
        }

        /// <summary>
        /// Gets the left value. 
        /// </summary> 
        /// <returns>
        /// ScimError.
        /// </returns>
        public override IEnumerable<ScimError> GetLeft()
        {
            return _Errors;
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