using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallpaperChanger.Interfaces;

namespace WallpaperChanger.Files;

public class FileImageProvider : IImageProvider<FileConfiguration>
{
    public Task<Image> GetImageAsync(FileConfiguration source, Screen screen)
    {
        var wallpapers = ProcessDirectory(source).ToArray();

        var count = wallpapers.Count();
        var rnd = new Random();

        for (int i = 0; i < count && i < 10; i++)
        {
            var path = wallpapers[rnd.Next(count)];
            var img = Image.FromFile(path);

            if (IsValidImage(source, img, screen))
            {
                return Task.FromResult(img);
            }
        }

        return Task.FromResult<Image>(null);
    }

    private static IEnumerable<string> ProcessDirectory(FileConfiguration config) => ProcessDirectory(config.Path, config.Depth, config.Exclude);
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
    
    private static bool IsValidImage(FileConfiguration source, Image img, Screen screen)
    {
        switch (source.Orientation)
        {
            case ScreenOrientation.Landscape:
                if (img.Height > img.Width * source.ImageAspectRatio)
                    return false;
                break;
            case ScreenOrientation.Portrait:
                if (img.Width > img.Height * source.ImageAspectRatio)
                    return false;
                break;
        }

        if (img.Width < screen.WorkingArea.Width * source.ImageToScreenSizeRatio)
            return false;
        else if (img.Width < source.MinWidth)
            return false;

        if (img.Height < screen.WorkingArea.Height * source.ImageToScreenSizeRatio)
            return false;
        else if (img.Height < source.MinHeight)
            return false;

        return true;
    }
}