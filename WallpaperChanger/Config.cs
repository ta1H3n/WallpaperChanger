using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace WallpaperChanger
{
    public sealed class Config
    {
        [JsonProperty("index")]
        public static int Index { get; set; }

        [JsonProperty("screens")]
        public static List<List<ScreenSettings>> Screens { get; set; } = new List<List<ScreenSettings>>();

        [JsonProperty("directories")]
        public static List<WallpaperDirectory> Directories { get; set; } = new List<WallpaperDirectory>();

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
