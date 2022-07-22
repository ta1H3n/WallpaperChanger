﻿using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace WallpaperChanger
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var monitors = Screen.AllScreens;
                var images = new Dictionary<string, Image>();

                var screens = Config.Screens[Config.Index];
                var profiles = Config.Directories;

                string res = $"{DateTime.Now}\n";

                int i = 0;
                List<string> selected = new List<string>();
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
                            selected.Add(wallpapers[j]);
                            res += $"{wallpapers[j]}\n";
                            break;
                        }
                    }
                    i++;
                }

                var client = new WallpaperEngine(images);
                client.SetWallpapers();

                AppendHistory(res);
                for (int j = 0; j < selected.Count; j++)
                {
                    CreateOrReplaceShortcut(selected[j], j.ToString());
                }
            }
            catch (Exception ex)
            {
                WriteToFile($"\\log\\ErrorLog_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt", ex.ToString());
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
            shortcut.TargetPath = Path.GetFullPath(path);
            shortcut.Save();
        }

        private static void AppendHistory(string text)
        {
            WriteToFile("\\history.txt.", text);
        }

        private static void WriteToFile(string file, string text)
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