using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Toucan.Contract;

namespace Toucan.Server.Core
{
    public class DateTimeConverter : JsonConverter
    {
        private readonly Type[] types = new Type[] { typeof(DateTime), typeof(DateTime?) };
        private readonly TimeZoneInfo sourceTimeZone;

        public DateTimeConverter(TimeZoneInfo sourceTimeZone)
        {
            this.sourceTimeZone = sourceTimeZone;
        }

        public override bool CanConvert(Type objectType)
        {
            return this.types.Any(o => o == objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Date && this.types.Any(o => o == objectType))
            {
                object value = reader.Value;

                DateTime date = value.GetType() == typeof(DateTime?) ? (DateTime)value : ((DateTime?)value).Value;

                existingValue = date.ToSourceUtc(this.sourceTimeZone, null);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type == JTokenType.Date)
            {
                if (value != null)
                {
                    DateTime? date = new Nullable<DateTime>((DateTime)value);
                    string kind = date.HasValue ? date.Value.Kind.ToString() : "";
                    date = TimeZoneInfo.ConvertTimeFromUtc(date.Value, this.sourceTimeZone);
                    t = JToken.FromObject(date);
                }
            }

            t.WriteTo(writer);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
    }
}