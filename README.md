# WallpaperChanger
Allows selecting different folder slideshow for each of your monitors. 
Source can be your file system or booru image databases.

### Usage
1. Download zip file from Releases or compile yourself with dotnet.
1. Extract in a folder of your choice.
1. Edit `Settings.json`
1. Run `WallpaperChanger.exe`, if everything is correct, it will create a wallpaper.bmp image in the same directory and set it as your desktop background.
1. To automate, you can create a Task Scheduler job.

### Settings
Edit `Settings.json`:
- `Keys` - keys of the configurations (one for each of your monitors)
- `Providers` - configurations in a `"Key": { configuration }` pattern, where each configuration can have one the following types:
  - Getting image from your file system:
    - `ProviderType` - `File`
    - `Orientation` - `Landscape`, `Portrait` or `Any`. Default: Any
    - `ImageAspectRatio` - maximum image width/height or height/width depending on `orientation`. Default: 1. Expected range 0-1
    - `ImageToScreenSizeRatio` - minimum image height/screen height and image width/screen width. Default: 0. Expected range 0-1
    - `MinHeight` - minimum height of the image. Default: 0
    - `MinWidth` - minimum width of the image. Default: 0
    - `Directories` - index of the directories defined below to search for wallpapers in.
    - `Path` - path to a folder or an image.
    - `Exclude` - exclude directories containing these keywords.
    - `Depth` - how deep to recursively search subdirectories. Default: 0
  - Getting images from a booru
    - `ProviderType` - `Booru`
    - `Booru` - `Konachan` `Gelbooru` `Danbooru` `E621` `Allthefallen` `Sankaku` `Yandere`
    - `Tags` - array of string tags for image search
