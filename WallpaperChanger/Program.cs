using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallpaperChanger.Common;


namespace WallpaperChanger
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var monitors = Screen.AllScreens;


                var results = new Dictionary<string, ProviderResult>();
                var tasks = new Task[Config.Keys.Count];
                for (int i = 0; i < Config.Keys.Count; i++)
                {
                    var provider = Config.Providers[Config.Keys[i]];
                    var monitor = monitors[i];
                    
                    tasks[i] = Task.Run(() =>
                    {
                        var res = provider.GetImageAsync(monitor).Result;
                        results.Add(monitor.DeviceName, res);
                    });
                }

                if (tasks.All(x => x != null))
                {
                    Task.WaitAll(tasks);
                }

                var client = new WallpaperEngine(results.ToDictionary(x => x.Key, x => x.Value.Image));
                client.SetWallpapers();

                string res = $"{DateTime.Now}\n";
                for (int i = 0; i < Config.Keys.Count; i++)
                {
                    var provider = Config.Providers[Config.Keys[i]];
                    var monitor = monitors[i];
                    var result = results[monitor.DeviceName];
                    res += result.Path + "\n";

                    tasks[i] = Task.Run(() => provider.AfterImageSet(monitor, result));
                }
                AppendHistory(res);

                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                AppendToFile($"\\log\\ErrorLog_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt", ex.ToString());
            }
        }



        private static void AppendHistory(string text)
        {
            AppendToFile("\\history.txt.", text);
        }

        private static void AppendToFile(string file, string text)
        {
            string target = Path.Combine(Directory.GetCurrentDirectory() + file);
            Directory.CreateDirectory(Path.GetDirectoryName(target));

            using (FileStream fs = new FileStream(target, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(text);
                }
            }
        }
    }
}