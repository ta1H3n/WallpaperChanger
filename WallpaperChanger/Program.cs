using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using BooruSharp.Booru;


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

                var tasks = new Task[2];
                foreach (var screen in screens)
                {
                    if (screen.Booru != BooruType.File)
                    {
                        var monitorName = monitors[i].DeviceName;
                        tasks[i] = Task.Run(() =>
                        {
                            ABooru booru = screen.Booru switch
                            {
                                BooruType.Allthefallen => new Atfbooru(),
                                BooruType.Danbooru => new DanbooruDonmai(),
                                BooruType.E621 => new E621(),
                                BooruType.Gelbooru => new Gelbooru(),
                                BooruType.Konachan => new Konachan(),
                                BooruType.Sankaku => new SankakuComplex(),
                                BooruType.Yandere => new Yandere(),
                                _ => throw new InvalidEnumArgumentException()
                            };

                            var result = booru.GetRandomPostAsync(screen.Tags).Result;
                            byte[] imageBytes = new WebClient().DownloadData(result.FileUrl.AbsoluteUri);
                            var image = Image.FromStream(new MemoryStream(imageBytes));

                            res += $"{result.PostUrl}\n";
                            images.Add(monitorName, image);
                        });
                    }
                    else
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
                    }
                    i++;
                }

                Task.WaitAll(tasks);

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