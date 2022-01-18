using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WallpaperChanger
{
    public sealed class Config
    {
        [JsonProperty("screens")]
        public static List<ScreenSettings> Screens { get; set; } = new List<ScreenSettings>();

        static Config()
        {
            string json = "";
            using (var stream = new FileStream(GetPath(), FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
                reader.Close();
            }

            JsonConvert.DeserializeObject<Config>(json);
        }

        private static string GetPath()
        {
            return Directory.GetCurrentDirectory() + @"\settings.json";
        }
    }
}
