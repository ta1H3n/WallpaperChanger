using System;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms;

namespace WallpaperChanger
{
    public class ScreenSettings
    {
        [JsonProperty("orientation")]
        public ScreenOrientation Orientation { get; set; } = ScreenOrientation.Any;
        [JsonProperty("imageAspectRatio")]
        public double ImageAspectRatio { get; set; } = 1;
        [JsonProperty("imageToScreenSizeRatio")]
        public double ImageToScreenSizeRatio { get; set; } = 0;
        [JsonProperty("minHeight")]
        public int MinHeight { get; set; } = 0;
        [JsonProperty("minWidth")]
        public int MinWidth { get; set; } = 0;

        [JsonProperty("directories")]
        public int[] Directories { get; set; } = new int[0];


        [JsonProperty("booru")] 
        public BooruType Booru { get; set; } = BooruType.File;
        [JsonProperty("tags")] 
        public string[] Tags { get; set; } = Array.Empty<string>();


        public bool IsValidImage(Image img, Screen screen)
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
    }

    public enum ScreenOrientation
    {
        Portrait,
        Landscape,
        Any
    }

    public enum BooruType
    {
        File,
        Konachan,
        Gelbooru,
        Danbooru,
        E621,
        Allthefallen,
        Sankaku,
        Yandere
    }
}
