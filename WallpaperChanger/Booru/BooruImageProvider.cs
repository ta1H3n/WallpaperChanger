using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using BooruSharp.Booru;
using Newtonsoft.Json;
using WallpaperChanger.Common;

namespace WallpaperChanger.Booru;

public class BooruImageProvider : IImageProvider
{
    [JsonProperty]
    public ProviderType ProviderType { get; }
    [JsonProperty]
    public BooruType BooruType { get; set; } = BooruType.Konachan;
    [JsonProperty]
    public string[] Tags { get; set; } = Array.Empty<string>();
    
    
    public async Task<ProviderResult> GetImageAsync(Screen screen)
    {
        ABooru booru = BooruType switch
        {
            BooruType.Allthefallen => new Atfbooru() { Auth = GetAuth(BooruType.Allthefallen) },
            BooruType.Danbooru => new DanbooruDonmai() { Auth = GetAuth(BooruType.Danbooru) },
            BooruType.E621 => new E621() { Auth = GetAuth(BooruType.E621) },
            BooruType.Gelbooru => new Gelbooru() { Auth = GetAuth(BooruType.Gelbooru) },
            BooruType.Konachan => new Konachan() { Auth = GetAuth(BooruType.Konachan) },
            BooruType.Sankaku => new SankakuComplex() { Auth = GetAuth(BooruType.Sankaku) },
            BooruType.Yandere => new Yandere() { Auth = GetAuth(BooruType.Yandere) },
            _ => throw new InvalidEnumArgumentException()
        };
                        
        var result = await booru.GetRandomPostAsync(Tags);
        byte[] imageBytes = await new WebClient().DownloadDataTaskAsync(result.FileUrl.AbsoluteUri);
        var image = Image.FromStream(new MemoryStream(imageBytes));

        return new ProviderResult
        {
            Image = image,
            Path = result.PostUrl.AbsoluteUri
        };
    }

    public Task AfterImageSet(Screen screen, ProviderResult result)
    {
        var name = screen.DeviceName.Replace("\\", "").Replace(".", "");
        string target = Directory.GetCurrentDirectory() + "\\" + name + "_image.jpg";
        if (Directory.Exists(target))
            Directory.Delete(target);

        result.Image.Save(target);
        return Task.CompletedTask;
    }

    private BooruAuth GetAuth(BooruType booruType)
    {
        var auth = Config.Auth[ProviderType.Booru][booruType];
        return new BooruAuth(auth.UserId, auth.PasswordHash);
    }
}

public enum BooruType
{
    Konachan,
    Gelbooru,
    Danbooru,
    E621,
    Allthefallen,
    Sankaku,
    Yandere
}