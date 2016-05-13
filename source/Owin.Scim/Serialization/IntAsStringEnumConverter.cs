namespace Owin.Scim.Serialization
{
    using System;

    using Newtonsoft.Json;

    public class IntAsStringEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(((int) value).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                if (!IsNullable(objectType))
                    throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));

                return (object) null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return Enum.ToObject(objectType, Convert.ToInt32(reader.Value));
            }

            throw new JsonSerializationException(string.Format("Cannot convert non-integer value to {0}.", objectType));
        }

        private static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious

            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>

            return false; // value-type
        }
    }
}