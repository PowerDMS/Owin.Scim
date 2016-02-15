namespace Owin.Scim.Serialization
{
    using System;

    using Model;

    using Newtonsoft.Json;

    public class ScimErrorTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value as string;
            if (value == null) return null;

            ScimErrorType error = (ScimErrorType) value;

            return error;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (string);
        }
    }
}