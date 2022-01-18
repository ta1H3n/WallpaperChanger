# WallpaperChanger
Allows selecting different folder slideshow for each of your monitors. With options how deep to crawl in subdirectories, filter on images based on orientation (Portrait|Landscape) and resolution.

### Usage
- Download zip file from Releases or compile yourself with dotnet.
- Extract in a folder of your choice.
- Edit Settings.json for each of your screens:
  - Screens are in order (like in your Display settings window), you can add as many as you want.
  - `path` - path to a folder or an image.
  - `depth` - how many parent subdirectories to search.
  - `orientation` - `Landscape`, `Portrait` or `Any`.
  - `minHeight` - minimum height of the image. `-1` to disable, `0` to use monitor height`
  - `minWidth` - minimum width of the image. `-1` to disable, `0` to use monitor width`
- Run `WallpaperChanger.exe`, if everything is correct, it will create a wallpaper.bmp image in the same directory and set it as your desktop background.
- To automate, you can create a Task Scheduler job.
