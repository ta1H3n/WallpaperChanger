using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WallpaperChanger
{
    public class ScreenSettings
    {
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("exclude")]
        public List<string> Exclude { get; set; } = new List<string>();
        [JsonProperty("depth")]
        public int Depth { get; set; } = 0;
        [JsonProperty("orientation")]
        public ScreenOrientation Orientation { get; set; } = ScreenOrientation.Any;
        [JsonProperty("ratio")]
        public double Ratio { get; set; } = 1;
        [JsonProperty("minHeight")]
        public int MinHeight { get; set; } = -1;
        [JsonProperty("minWidth")]
        public int MinWidth { get; set; } = -1;


        public bool IsValidImage(Image img, Screen screen)
        {
            switch (Orientation)
            {
                case ScreenOrientation.Landscape:
                    if (img.Height > img.Width * Ratio)
                        return false;
                    break;
                case ScreenOrientation.Portrait:
                    if (img.Width > img.Height * Ratio)
                        return false;
                    break;
            }

            if (MinWidth == 0 && img.Width < screen.WorkingArea.Width)
                return false;
            else if (MinWidth != -1 && img.Width < MinWidth)
                return false;

            if (MinHeight == 0 && img.Height < screen.WorkingArea.Height)
                return false;
            else if (MinHeight != -1 && img.Height < MinHeight)
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
}
