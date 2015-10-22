namespace Owin.Scim.Model
{
    using System;

    using Newtonsoft.Json;

    [JsonConverter(typeof(ScimTypeConverter))]
    public sealed class ScimType
    {
        private readonly string _Type;

        [JsonConstructor]
        internal ScimType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException("type");

            _Type = type;
        }

        public static implicit operator string(ScimType type)
        {
            return type == null ? null : type.ToString();
        }

        /// <summary>
        /// The specified filter syntax was invalid (does not comply with Figure 1), or the 
        /// specified attribute and filter comparison combination is not supported.
        /// </summary>
        public static ScimType InvalidFilter
        {
            get { return new ScimType("invalidFilter"); }
        }

        /// <summary>
        /// The specified filter yields many more results than the server is willing to calculate 
        /// or process.For example, a filter such as "(userName pr)" by itself would return all 
        /// entries with a "userName" and MAY not be acceptable to the service provider.
        /// </summary>
        public static ScimType TooMany
        {
            get { return new ScimType("tooMany"); }
        }

        /// <summary>
        /// One or more of the attribute values are already in use or are reserved.
        /// </summary>
        public static ScimType Uniqueness
        {
            get { return new ScimType("uniqueness"); }
        }

        /// <summary>
        /// The attempted modification is not compatible with the target attribute's mutability or 
        /// current state (e.g., modification of an "immutable" attribute with an existing value).
        /// </summary>
        public static ScimType Mutability
        {
            get { return new ScimType("mutability"); }
        }

        /// <summary>
        /// The request body message structure was invalid or did not conform to the request schema.
        /// </summary>
        public static ScimType InvalidSyntax
        {
            get { return new ScimType("invalidSyntax"); }
        }

        /// <summary>
        /// The "path" attribute was invalid or malformed.
        /// </summary>
        public static ScimType InvalidPath
        {
            get { return new ScimType("invalidPath"); }
        }

        /// <summary>
        /// The specified "path" did not yield an attribute or attribute value that could be 
        /// operated on.This occurs when the specified "path" value contains a filter that yields no match.
        /// </summary>
        public static ScimType NoTarget
        {
            get { return new ScimType("noTarget"); }
        }

        /// <summary>
        /// A required value was missing, or the value specified was not compatible with the operation 
        /// or attribute type or resource schema.
        /// </summary>
        public static ScimType InvalidValue
        {
            get { return new ScimType("invalidValue"); }
        }

        /// <summary>
        /// The specified SCIM protocol version is not supported.
        /// </summary>
        public static ScimType InvalidVers
        {
            get { return new ScimType("invalidVers"); }
        }

        /// <summary>
        /// The specified request cannot be completed, due to the passing of sensitive (e.g., 
        /// personal) information in a request URI.  For example, personal information SHALL NOT 
        /// be transmitted over request URIs.
        /// </summary>
        public static ScimType Sensitive
        {
            get { return new ScimType("sensitive"); }
        }

        public override string ToString()
        {
            return _Type;
        }
    }
}