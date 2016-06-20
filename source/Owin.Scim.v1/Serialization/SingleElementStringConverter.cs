namespace Owin.Scim.v1.Serialization
{
    using System;
    using System.Collections;

    using Extensions;

    using Newtonsoft.Json;

    public class SingleElementStringConverter : JsonConverter
    {
        public override bool CanRead
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumerator = ((IEnumerable) value).GetEnumerator();
            if (enumerator.MoveNext())
                writer.WriteValue(enumerator.Current.ToString());
            else
                writer.WriteNull();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsNonStringEnumerable();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}