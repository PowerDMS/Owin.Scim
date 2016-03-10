namespace Owin.Scim.Querying
{
    using System;

    public class PathFilterExpression : IEquatable<PathFilterExpression>
    {
        public PathFilterExpression(string path, string filter)
        {
            Path = path;
            Filter = filter;
        }

        public string Path { get; private set; }

        public string Filter { get; private set; }

        public static PathFilterExpression CreatePathOnly(string path)
        {
            return new PathFilterExpression(path, null);
        }

        public static PathFilterExpression CreateFilterOnly(string filter)
        {
            return new PathFilterExpression(null, filter);
        }

        public bool Equals(PathFilterExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Path, other.Path) && string.Equals(Filter, other.Filter);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathFilterExpression) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ (Filter != null ? Filter.GetHashCode() : 0);
            }
        }

        public static bool operator ==(PathFilterExpression left, PathFilterExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PathFilterExpression left, PathFilterExpression right)
        {
            return !Equals(left, right);
        }
    }
}