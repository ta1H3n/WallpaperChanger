using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallpaperChanger.Common;

public interface IImageProvider
{
    ProviderType ProviderType { get; }
    
    Task<ProviderResult> GetImageAsync(Screen screen);
    Task AfterImageSet(Screen screen, ProviderResult result);
}

public enum ProviderType
{
    Booru,
    File
}