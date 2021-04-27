using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PRIME.Core.MedCheck;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PRIME.Core.Common.Models.RX
{

    public partial class RxResponse
    {
        [JsonProperty("idGroup")]
        public IdGroup IdGroup { get; set; }
    }

    public partial class IdGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rxnormId")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> RxnormId { get; set; }
    }

    public partial class RxResponse
    {
        public static RxResponse FromJson(string json) => JsonConvert.DeserializeObject<RxResponse>(json, RXConverter.Settings);
    }

    public static class RXSerialize
    {
        public static string ToJson(this RxResponse self) => JsonConvert.SerializeObject(self, RXConverter.Settings);
    }

    internal static class RXConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DecodeArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(List<long>);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            var value = new List<long>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var converter = RXParseStringConverter.Singleton;
                var arrayItem = (long)converter.ReadJson(reader, typeof(long), null, serializer);
                value.Add(arrayItem);
                reader.Read();
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (List<long>)untypedValue;
            writer.WriteStartArray();
            foreach (var arrayItem in value)
            {
                var converter = RXParseStringConverter.Singleton;
                converter.WriteJson(writer, arrayItem, serializer);
            }
            writer.WriteEndArray();
            return;
        }

        public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
    }

    internal class RXParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly RXParseStringConverter Singleton = new RXParseStringConverter();
    }
  


}
