namespace Owin.Scim.Model
{
    using System;

    using Newtonsoft.Json;

    [JsonConverter(typeof(ScimTypeConverter))]
    public sealed class ScimErrorType : IEquatable<ScimErrorType>
    {
        private readonly string _Type;

        private readonly string _Message;

        [JsonConstructor]
        internal ScimErrorType(string type, string message)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException("type");

            _Type = type;
            _Message = message;
        }

        public static implicit operator string (ScimErrorType errorType)
        {
            return errorType == null ? null : errorType.ToString();
        }

        public string Type
        {
            get { return _Type; }
        }

        public string Message
        {
            get { return _Message; }
        }

        /// <summary>
        /// The specified filter syntax was invalid (does not comply with Figure 1), or the 
        /// specified attribute and filter comparison combination is not supported.
        /// </summary>
        public static ScimErrorType InvalidFilter
        {
            get
            {
                return new ScimErrorType(
                    "invalidFilter",
                    "The specified filter syntax was invalid (does not comply with Figure 1), or the " +
                    "specified attribute and filter comparison combination is not supported.");
            }
        }

        /// <summary>
        /// The specified filter yields many more results than the server is willing to calculate 
        /// or process.For example, a filter such as "(userName pr)" by itself would return all 
        /// entries with a "userName" and MAY not be acceptable to the service provider.
        /// </summary>
        public static ScimErrorType TooMany
        {
            get
            {
                return new ScimErrorType(
                    "tooMany",
                    "The specified filter yields many more results than the server is willing to calculate " +
                    "or process.For example, a filter such as \"(userName pr)\" by itself would return all entries " +
                    "with a \"userName\" and MAY not be acceptable to the service provider.");
            }
        }

        /// <summary>
        /// One or more of the attribute values are already in use or are reserved.
        /// </summary>
        public static ScimErrorType Uniqueness
        {
            get
            {
                return new ScimErrorType(
                    "uniqueness",
                    "One or more of the attribute values are already in use or are reserved.");
            }
        }

        /// <summary>
        /// The attempted modification is not compatible with the target attribute's mutability or 
        /// current state (e.g., modification of an "immutable" attribute with an existing value).
        /// </summary>
        public static ScimErrorType Mutability
        {
            get
            {
                return new ScimErrorType(
                    "mutability",
                    "The attempted modification is not compatible with the target attribute's mutability or " +
                    "current state (e.g., modification of an \"immutable\" attribute with an existing value).");
            }
        }

        /// <summary>
        /// The request body message structure was invalid or did not conform to the request schema.
        /// </summary>
        public static ScimErrorType InvalidSyntax
        {
            get
            {
                return new ScimErrorType(
                    "invalidSyntax",
                    "The request body message structure was invalid or did not conform to the request schema.");
            }
        }

        /// <summary>
        /// The "path" attribute was invalid or malformed.
        /// </summary>
        public static ScimErrorType InvalidPath
        {
            get
            {
                return new ScimErrorType(
                    "invalidPath",
                    "The \"path\" attribute was invalid or malformed.");
            }
        }

        /// <summary>
        /// The specified "path" did not yield an attribute or attribute value that could be 
        /// operated on.This occurs when the specified "path" value contains a filter that yields no match.
        /// </summary>
        public static ScimErrorType NoTarget
        {
            get
            {
                return new ScimErrorType(
                    "noTarget",
                    "The specified \"path\" did not yield an attribute or attribute value that could be " +
                    "operated on.This occurs when the specified \"path\" value contains a filter that yields no match.");
            }
        }

        /// <summary>
        /// A required value was missing, or the _Value specified was not compatible with the operation 
        /// or attribute type or resource schema.
        /// </summary>
        public static ScimErrorType InvalidValue
        {
            get
            {
                return new ScimErrorType(
                    "invalidValue",
                    "A required value was missing, or the _Value specified was not compatible with the operation " +
                    "or attribute type or resource schema.");
            }
        }

        /// <summary>
        /// The specified SCIM protocol version is not supported.
        /// </summary>
        public static ScimErrorType InvalidVers
        {
            get
            {
                return new ScimErrorType(
                    "invalidVers",
                    "The specified SCIM protocol version is not supported.");
            }
        }

        /// <summary>
        /// The specified request cannot be completed, due to the passing of sensitive (e.g., 
        /// personal) information in a request URI.  For example, personal information SHALL NOT 
        /// be transmitted over request URIs.
        /// </summary>
        public static ScimErrorType Sensitive
        {
            get
            {
                return new ScimErrorType(
                    "sensitive",
                    "The specified request cannot be completed, due to the passing of sensitive (e.g., " +
                    "personal) information in a request URI.  For example, personal information SHALL NOT " +
                    "be transmitted over request URIs.");
            }
        }

        public override string ToString()
        {
            return _Type;
        }

        #region IEquatable Implementation

        public bool Equals(ScimErrorType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_Type, other._Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ScimErrorType && Equals((ScimErrorType)obj);
        }

        public override int GetHashCode()
        {
            return (_Type != null ? _Type.GetHashCode() : 0);
        }

        public static bool operator ==(ScimErrorType left, ScimErrorType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScimErrorType left, ScimErrorType right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}