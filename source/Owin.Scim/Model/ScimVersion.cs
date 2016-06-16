namespace Owin.Scim.Model
{
    using System;
    using System.Net;

    using ErrorHandling;

    public class ScimVersion : IEquatable<ScimVersion>
    {
        public ScimVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ScimException(HttpStatusCode.InternalServerError, "ScimVersion cannot be null.");

            Version = version.ToLower();
        }

        public string Version { get; private set; }

        public bool Equals(ScimVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScimVersion) obj);
        }

        public static bool operator ==(ScimVersion left, ScimVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScimVersion left, ScimVersion right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }
    }
}