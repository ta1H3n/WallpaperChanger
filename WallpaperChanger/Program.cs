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

            int i = 0;
            foreach (var screen in Config.Screens)
            {
                var wallpapers = new List<string>();

                if (Directory.Exists(screen.Path))
                {
                    if (screen.Path.EndsWith(".jpeg") || screen.Path.EndsWith(".jpg") || screen.Path.EndsWith(".png") || screen.Path.EndsWith(".bmp"))
                        wallpapers.Add(screen.Path);
                    else
                        wallpapers = ProcessDirectory(screen.Path, screen.Depth).ToList();
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
                        break;
                    }
                }
                i++;
            }

            var client = new WallpaperEngine(images);
            client.SetWallpapers();
        }


        private static IEnumerable<string> ProcessDirectory(string targetDirectory, int depth)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                if (fileName.EndsWith(".jpeg") || fileName.EndsWith(".jpg") || fileName.EndsWith(".png") || fileName.EndsWith(".bmp"))
                   yield return fileName;

            // Recurse into subdirectories of this directory.
            if (depth > 0)
            {
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    foreach(var d in ProcessDirectory(subdirectory, depth - 1))
                        yield return d;
            }
        }
    }
}