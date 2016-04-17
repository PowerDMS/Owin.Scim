namespace Owin.Scim.Model
{
    using System;

    using Newtonsoft.Json;
    
    public abstract class MultiValuedAttribute : IEquatable<MultiValuedAttribute>
    {
        public string Type { get; set; }

        public bool Primary { get; set; }
        
        public string Display { get; set; }

        public string Value { get; set; }

        [JsonProperty("$ref")]
        public Uri Ref { get; set; }

        protected internal virtual int CalculateVersion()
        {
            return new
            {
                Type,
                Value,
                Primary
            }.GetHashCode();
        }

#region IEquatable Implementation

        public bool Equals(MultiValuedAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Type, other.Type) && 
                   string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as MultiValuedAttribute;

            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 
                    ((Type != null ? Type.GetHashCode() : 0) * 397) ^ 
                    (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(MultiValuedAttribute left, MultiValuedAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MultiValuedAttribute left, MultiValuedAttribute right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}