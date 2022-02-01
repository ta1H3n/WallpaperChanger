using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WallpaperChanger
{
    public class WallpaperDirectory
    {
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("exclude")]
        public List<string> Exclude { get; set; } = new List<string>();
        [JsonProperty("depth")]
        public int Depth { get; set; } = 0;

        public IEnumerable<string> ProcessDirectory() => ProcessDirectory(Path, Depth, Exclude);
        public static IEnumerable<string> ProcessDirectory(string targetDirectory, int depth, List<string> exclude)
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
                foreach (string subdirectory in subdirectoryEntries.Where(x => !exclude.Any(y => x.Contains(y))))
                    foreach (var d in ProcessDirectory(subdirectory, depth - 1, exclude))
                        yield return d;
            }
        }
    }
}
