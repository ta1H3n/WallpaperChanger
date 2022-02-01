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
            var screens = Screen.AllScreens;
            var images = new Dictionary<string, Image>();

            var monitors = Config.Screens[Config.Index];

            string res = $"{DateTime.Now}\n";

            int i = 0;
            foreach (var screen in monitors)
            {
                var wallpapers = new List<string>();

                if (Directory.Exists(screen.Path))
                {
                    if (screen.Path.EndsWith(".jpeg") || screen.Path.EndsWith(".jpg") || screen.Path.EndsWith(".png") || screen.Path.EndsWith(".bmp"))
                        wallpapers.Add(screen.Path);
                    else
                        wallpapers = ProcessDirectory(screen.Path, screen.Depth, screen.Exclude).ToList();
                }
                else
                {
                    Console.WriteLine("Invalid path");
                }

                Image img;
                for (int l = 0; l < wallpapers.Count; l++)
                {
                    int j = new Random().Next(wallpapers.Count);
                    img = Image.FromFile(wallpapers[j]);

                    if (screen.IsValidImage(img, screens[i]))
                    {
                        images.Add(screens[i].DeviceName, img);
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


        private static IEnumerable<string> ProcessDirectory(string targetDirectory, int depth, List<string> exclude)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries.Where(x => !exclude.Any(y => x.Contains(y))))
                if (fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg") || fileName.EndsWith(".png") || fileName.EndsWith(".bmp"))
                   yield return fileName;

            // Recurse into subdirectories of this directory.
            if (depth > 0)
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries.Where(x => !exclude.Any(y => x.Contains(y))))
                    foreach(var d in ProcessDirectory(subdirectory, depth - 1, exclude))
                        yield return d;
            }
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
            // not sure about what this is for
            //shortcut.WindowStyle = 1;
            //shortcut.Description = "my shortcut description";
            //shortcut.WorkingDirectory = "c:\\app";
            //shortcut.IconLocation = path;
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