using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace WallpaperChanger
{
    public class Program
    {
        public static void Main()
        {
            var monitors = Screen.AllScreens;
            var images = new Dictionary<string, Image>();

            var screens = Config.Screens[Config.Index];
            var profiles = Config.Directories;

            string res = $"{DateTime.Now}\n";

            int i = 0;
            foreach (var screen in screens)
            {
                var wallpapers = new List<string>();

                foreach (var p in screen.Directories)
                {
                    wallpapers.AddRange(profiles[p].ProcessDirectory());
                }

                Image img;
                for (int l = 0; l < wallpapers.Count; l++)
                {
                    int j = new Random().Next(wallpapers.Count);
                    img = Image.FromFile(wallpapers[j]);

                    if (screen.IsValidImage(img, monitors[i]))
                    {
                        images.Add(monitors[i].DeviceName, img);
                        CreateOrReplaceShortcut(wallpapers[j], i.ToString());
                        res += $"{wallpapers[j]}\n";
                        break;
                    }
                }
                i++;
            }

            var client = new WallpaperEngine(images);
            client.SetWallpapers();
            AppendHistory(res);
        }


        private static void CreateOrReplaceShortcut(string path, string name)
        {
            string target = Directory.GetCurrentDirectory() + "\\screen_" + name + "_image_shortcut.lnk";
            if (Directory.Exists(target))
                Directory.Delete(target);

            WshShell wsh = new WshShell();
            IWshShortcut shortcut = wsh.CreateShortcut(target) as IWshShortcut;
            shortcut.Arguments = "";
            shortcut.TargetPath = path;
            shortcut.Save();
        }

        private static void AppendHistory(string text)
        {
            string target = Directory.GetCurrentDirectory() + "\\history.txt.";

            using (FileStream fs = new FileStream(target, FileMode.Append)) {
                using (StreamWriter sw = new StreamWriter(fs)) { 
                    sw.WriteLine(text);
                }
            }
        }
    }
}