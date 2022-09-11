using System;
using WallpaperChanger.Interfaces;

namespace WallpaperChanger.Booru;

public class BooruConfiguration : IProviderCnfiguration
{
    public string Key { get; set; }
    public BooruType BooruType { get; set; } = BooruType.Konachan;
    public string[] Tags { get; set; } = Array.Empty<string>();
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