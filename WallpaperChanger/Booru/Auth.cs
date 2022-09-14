using Newtonsoft.Json;

namespace WallpaperChanger.Booru;

public class Auth
{
    [JsonProperty]
    public string UserId { get; set; }
    [JsonProperty]
    public string PasswordHash { get; set; }
}