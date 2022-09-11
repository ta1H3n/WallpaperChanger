using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallpaperChanger.Interfaces;

public interface IImageProvider<T> where T : IProviderCnfiguration
{
    Task<Image> GetImageAsync(T source, Screen screen);
}