# WallpaperChanger
Allows selecting different folder slideshow for each of your monitors. With options how deep to crawl in subdirectories, filter on images based on orientation (Portrait|Landscape) and resolution.

### Usage
1. Download zip file from Releases or compile yourself with dotnet.
1. Extract in a folder of your choice.
1. Edit `Settings.json`
1. Run `WallpaperChanger.exe`, if everything is correct, it will create a wallpaper.bmp image in the same directory and set it as your desktop background.
1. To automate, you can create a Task Scheduler job.

### Settings
Edit `Settings.json`:
- `index` - which list of screens to use:
- `screens` - List of list of screens. You can add as many as you want, the list with `index` index will be used. Accepts two different configurations:
  - Getting image from your file system:
    - `orientation` - `Landscape`, `Portrait` or `Any`. Default: Any
    - `imageAspectRatio` - maximum image width/height or height/width depending on `orientation`. Default: 1. Expected range 0-1
    - `imageToScreenSizeRatio` - minimum image height/screen height and image width/screen width. Default: 0. Expected range 0-1
    - `minHeight` - minimum height of the image. Default: 0
    - `minWidth` - minimum width of the image. Default: 0
    - `directories` - index of the directories defined below to search for wallpapers in.
  - Getting images from a booru
    - `booru` - `Konachan` `Gelbooru` `Danbooru` `E621` `Allthefallen` `Sankaku` `Yandere`
    - `tags` - array of string tags for image search
- `directories` - You can add as many directories as you want:
  - `path` - path to a folder or an image.
  - `exclude` - exclude directories containing these keywords.
  - `depth` - how deep to recursively search subdirectories. Default: 0
