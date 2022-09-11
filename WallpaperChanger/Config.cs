using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using WallpaperChanger.Interfaces;

namespace WallpaperChanger
{
    public sealed class Config
    {
        public static string Key { get; set; }

        public static Dictionary<string, List<List<string>>> Screens { get; set; }

        public static List<IProviderCnfiguration> Configurations { get; set; }

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
