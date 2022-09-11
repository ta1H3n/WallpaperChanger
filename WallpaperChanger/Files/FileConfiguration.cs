using System.Collections.Generic;
using WallpaperChanger.Interfaces;

namespace WallpaperChanger.Files;

public class FileConfiguration : IProviderCnfiguration
{
    public string Key { get; set; }
    public string Path { get; set; }
    public List<string> Exclude { get; set; } = new List<string>();
    public int Depth { get; set; } = 0;
    
    
    public ScreenOrientation Orientation { get; set; } = ScreenOrientation.Any;
    public double ImageAspectRatio { get; set; } = 1;
    public double ImageToScreenSizeRatio { get; set; } = 0;
    public int MinHeight { get; set; } = 0;
    public int MinWidth { get; set; } = 0;
}