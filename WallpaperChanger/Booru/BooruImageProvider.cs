using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using BooruSharp.Booru;
using WallpaperChanger.Interfaces;

namespace WallpaperChanger.Booru;

public class BooruImageProvider : IImageProvider<BooruConfiguration>
{
    public async Task<Image> GetImageAsync(BooruConfiguration source, Screen screen)
    {
        ABooru booru = source.BooruType switch
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
                        
        var result = await booru.GetRandomPostAsync(source.Tags);
        byte[] imageBytes = await new WebClient().DownloadDataTaskAsync(result.FileUrl.AbsoluteUri);
        var image = Image.FromStream(new MemoryStream(imageBytes));

        return image;
    }
}