using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using WallpaperChanger.Booru;
using WallpaperChanger.Common;
using WallpaperChanger.Files;

namespace WallpaperChanger
{
    public sealed class Config
    {
        [JsonProperty]
        public static List<string> Keys { get; set; }
        [JsonProperty]
        public static Dictionary<string, IImageProvider> Providers { get; set; } = new();

        static Config()
        {
            string json = "";
            using (var stream = new FileStream(GetPath(), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            JsonConvert.DeserializeObject<Config>(json, new ProviderConverter());
        }

        private static string GetPath()
        {
            return Directory.GetCurrentDirectory() + @"\settings.json";
        }
    }
}

public class ProviderConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(IImageProvider));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var typeEnum = token[nameof(IImageProvider.ProviderType)]!.ToObject<ProviderType>();

        var type = typeEnum switch
        {
            ProviderType.Booru => typeof(BooruImageProvider),
            ProviderType.File => typeof(FileImageProvider),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        if (existingValue == null || existingValue.GetType() != type)
        {
            var contract = serializer.ContractResolver.ResolveContract(type);
            existingValue = contract.DefaultCreator();
        }

        using var subReader = token.CreateReader();
        serializer.Populate(subReader, existingValue);
        
        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
