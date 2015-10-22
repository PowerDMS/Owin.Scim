namespace Owin.Scim.Model
{
    using System;

    using Newtonsoft.Json;

    public class ScimTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new ScimType((string)reader.Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (string);
        }
    }
}