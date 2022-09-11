using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using WallpaperChanger.Common;

namespace WallpaperChanger.Files;

public class FileImageProvider : IImageProvider
{
    [JsonProperty]
    public ProviderType ProviderType { get; }
    [JsonProperty]
    public string Path { get; set; }
    [JsonProperty]
    public List<string> Exclude { get; set; } = new List<string>();
    [JsonProperty]
    public int Depth { get; set; } = 0;
    
    [JsonProperty]
    public ScreenOrientation Orientation { get; set; } = ScreenOrientation.Any;
    [JsonProperty]
    public double ImageAspectRatio { get; set; } = 1;
    [JsonProperty]
    public double ImageToScreenSizeRatio { get; set; } = 0;
    [JsonProperty]
    public int MinHeight { get; set; } = 0; 
    [JsonProperty]
    public int MinWidth { get; set; } = 0;
    
    
    public Task<ProviderResult> GetImageAsync(Screen screen)
    {
        var wallpapers = ProcessDirectory().ToArray();

        var rnd = new Random();

        foreach (var path in wallpapers.OrderBy(x => rnd.Next()))
        {
            var img = Image.FromFile(path);
            if (IsValidImage(img, screen))
            {
                return Task.FromResult(new ProviderResult
                {
                    Image = img,
                    Path = path
                });
            }
        }
        return Task.FromResult<ProviderResult>(new ProviderResult
        {
            Image = null,
            Path = "No matches found"
        });
    }

    public Task AfterImageSet(Screen screen, ProviderResult result)
    {
        CreateOrReplaceShortcut(result.Path, screen.DeviceName.Replace("\\", "").Replace(".", ""));
        return Task.CompletedTask;
    }

    private IEnumerable<string> ProcessDirectory() => ProcessDirectory(Path, Depth, Exclude);
    private static IEnumerable<string> ProcessDirectory(string targetDirectory, int depth, IReadOnlyCollection<string> exclude)
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
            foreach (string subdirectory in subdirectoryEntries.Where(x => !exclude.Any(x.Contains)))
            foreach (var d in ProcessDirectory(subdirectory, depth - 1, exclude))
                yield return d;
        }
    }
    
    private bool IsValidImage(Image img, Screen screen)
    {
        switch (Orientation)
        {
            case ScreenOrientation.Landscape:
                if (img.Height > img.Width * ImageAspectRatio)
                    return false;
                break;
            case ScreenOrientation.Portrait:
                if (img.Width > img.Height * ImageAspectRatio)
                    return false;
                break;
        }

        if (img.Width < screen.WorkingArea.Width * ImageToScreenSizeRatio)
            return false;
        else if (img.Width < MinWidth)
            return false;

        if (img.Height < screen.WorkingArea.Height * ImageToScreenSizeRatio)
            return false;
        else if (img.Height < MinHeight)
            return false;

        return true;
    }
    
    private static void CreateOrReplaceShortcut(string sourcePath, string name)
    {
        string target = Directory.GetCurrentDirectory() + "\\" + name + "_image_shortcut.lnk";
        if (Directory.Exists(target))
            Directory.Delete(target);

        WshShell wsh = new WshShell();
        IWshShortcut shortcut = wsh.CreateShortcut(target) as IWshShortcut;
        shortcut.Arguments = "";
        shortcut.TargetPath = System.IO.Path.GetFullPath(sourcePath);
        shortcut.Save();
    }
}

public enum ScreenOrientation
{
    Portrait,
    Landscape,
    Any
}