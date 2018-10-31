using Horsesoft.Music.Data.Model.Menu;
using Newtonsoft.Json;
using System;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public class MenuConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMenuComponent);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(Menu.Menu));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
