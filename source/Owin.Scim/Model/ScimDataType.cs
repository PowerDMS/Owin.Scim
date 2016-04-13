namespace Owin.Scim.Model
{
    public class ScimDataType
    {
        private static readonly ScimDataType _String = new ScimDataType("string");

        private static readonly ScimDataType _Boolean = new ScimDataType("boolean");

        private static readonly ScimDataType _Decimal = new ScimDataType("decimal");

        private static readonly ScimDataType _Integer = new ScimDataType("integer");

        private static readonly ScimDataType _DateTime = new ScimDataType("datetime");

        private static readonly ScimDataType _Reference = new ScimDataType("reference");

        private static readonly ScimDataType _Complex = new ScimDataType("complex");

        private readonly string _Value;

        private ScimDataType(string value)
        {
            _Value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ScimDataType"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(ScimDataType dataType)
        {
            return dataType._Value;
        }

        public static ScimDataType String
        {
            get { return _String; }
        }

        public static ScimDataType Boolean
        {
            get { return _Boolean; }
        }

        public static ScimDataType Decimal
        {
            get { return _Decimal; }
        }

        public static ScimDataType Integer
        {
            get { return _Integer; }
        }

        public static ScimDataType DateTime
        {
            get { return _DateTime; }
        }

        public static ScimDataType Reference
        {
            get { return _Reference; }
        }

        public static ScimDataType Complex
        {
            get { return _Complex; }
        }
    }
}