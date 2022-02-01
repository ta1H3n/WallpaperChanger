﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace WallpaperChanger
{
    public class WallpaperEngine
    {
        const int SetDeskWallpaper = 20;
        const int UpdateIniFile = 0x01;
        const int SendWinIniChange = 0x02;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        Point PrimaryMonitorPoint = new Point(0, 0);
        readonly string DefaultBackgroundFile;
        readonly Dictionary<string, Image> Images = new Dictionary<string, Image>();


        readonly List<string> Screens = new List<string>();
        int Index = 0;

        public WallpaperEngine(Dictionary<string, Image> images)
        {
            Images = images;
            DefaultBackgroundFile = Directory.GetCurrentDirectory() + "\\wallpaper.bmp";

            //figure out where the main monitor is in relation to the virtualScreenBitmap
            foreach (Screen scr in Screen.AllScreens)
            {
                if (!Images.ContainsKey(scr.DeviceName))
                    Images.Add(scr.DeviceName, null);
                Screens.Add(scr.DeviceName);
                if (scr.Bounds.Left < PrimaryMonitorPoint.X)
                    PrimaryMonitorPoint.X = scr.Bounds.Left;
                if (scr.Bounds.Top < PrimaryMonitorPoint.Y)
                    PrimaryMonitorPoint.Y = scr.Bounds.Top;
            }
            PrimaryMonitorPoint.X *= -1;
            PrimaryMonitorPoint.Y *= -1;

            ////Image for multiple screens
            //Images.Add("all", null);

            //set Images in Dictionary in case there are previous Images
            if (File.Exists(DefaultBackgroundFile))
            {
                using (var old = new Bitmap(DefaultBackgroundFile))
                {
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (Images.TryGetValue(scr.DeviceName, out Image val) && val == null)
                        {
                            Rectangle rectangle = new Rectangle(PrimaryMonitorPoint.X + scr.Bounds.Left, PrimaryMonitorPoint.Y + scr.Bounds.Top, scr.Bounds.Width, scr.Bounds.Height);
                            if (old.Width >= (rectangle.X + rectangle.Width) &&
                                old.Height >= (rectangle.Y + rectangle.Height))
                                Images[scr.DeviceName] = (Bitmap)old.Clone(rectangle, old.PixelFormat);
                        }
                    }
                }
            }
        }



        public void SetWallpapers()
        {
            CreateBackgroundImage(Method.multiple);
            GC.Collect();
        }

        public void SetAlternatingWallpapers(string file)
        {
            Images[Screens[Index]] = Image.FromFile(file);
            Index++;
            if (Index == Screens.Count)
                Index = 0;

            CreateBackgroundImage(Method.multiple);
            GC.Collect();
        }

        public void SetWallpaperForScreen(Screen screen, string file)
        {
            Images[screen.DeviceName] = Image.FromFile(file);
            CreateBackgroundImage(Method.multiple);
            GC.Collect();
        }

        public void SetWallpaper(string file)
        {
            Images["all"] = Image.FromFile(file);
            CreateBackgroundImage(Method.single);
            GC.Collect();
        }


        private enum Method
        {
            multiple,
            single
        }
        private void CreateBackgroundImage(Method method)
        {

            using (var virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (var virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    switch (method)
                    {
                        // alternated Screen Images
                        case Method.multiple:
                            foreach (var screen in Screen.AllScreens)
                            {
                                // gets the image which we want to place in virtualScreenGraphic
                                var image = Images.ContainsKey(screen.DeviceName) ? Images[screen.DeviceName] : null;

                                //sets the position and size where the images will go
                                Rectangle rectangle = new Rectangle(PrimaryMonitorPoint.X + screen.Bounds.Left, PrimaryMonitorPoint.Y + screen.Bounds.Top, screen.Bounds.Width, screen.Bounds.Height);

                                // produce a image for the screen and fill it with the desired image... centered
                                var monitorBitmap = new Bitmap(rectangle.Width, rectangle.Height);
                                if (image != null)
                                    DrawImageCentered(Graphics.FromImage(monitorBitmap), image, rectangle);

                                //draws the picture at the right place in virtualScreenGraphic
                                virtualScreenGraphic.DrawImage(monitorBitmap, rectangle);
                            }
                            break;

                        //Single screen Image
                        case Method.single:
                            // gets the image which we want to place in virtualScreenGraphic
                            var image2 = Images["all"];

                            //sets the position and size where the images will go
                            Rectangle rectangle2 = new Rectangle(0, 0, virtualScreenBitmap.Width, virtualScreenBitmap.Height);

                            // fill with the desired image... centered                            
                            if (image2 != null)
                                DrawImageCentered(virtualScreenGraphic, image2, rectangle2);

                            //draws the picture at the right place in virtualScreenGraphic
                            virtualScreenGraphic.DrawImage(virtualScreenBitmap, rectangle2);
                            break;
                    }

                    virtualScreenBitmap.Save(DefaultBackgroundFile, ImageFormat.Jpeg);
                }
            }

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", 22.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
            //key.SetValue(@"WallpaperStyle", 0.ToString());
            //key.SetValue(@"TileWallpaper", 1.ToString());
            SystemParametersInfo(SetDeskWallpaper, 0, DefaultBackgroundFile, UpdateIniFile | SendWinIniChange);
        }


        private void DrawImageCentered(Graphics g, Image img, Rectangle monitorRect)
        {
            //double ratiodev = (1.0 * monitorRect.Width / monitorRect.Height) - (1.0 * img.Width / img.Height);
            img = CropImage(img, monitorRect);

            float heightRatio = (float)monitorRect.Height / (float)img.Height;
            float widthRatio = (float)monitorRect.Width / (float)img.Width;
            int height;
            int width;
            int x = 0;
            int y = 0;

            if (heightRatio < widthRatio)
            {
                width = (int)((float)img.Width * heightRatio);
                height = (int)((float)img.Height * heightRatio);
                x = (int)((float)(monitorRect.Width - width) / 2f);
            }
            else
            {
                width = (int)((float)img.Width * widthRatio);
                height = (int)((float)img.Height * widthRatio);
                y = (int)((float)(monitorRect.Height - height) / 2f);
            }
            Rectangle rect = new Rectangle(x, y, width, height);
            g.DrawImage(img, rect);
        }

        private Image CropImage(Image img, Rectangle monitorRect)
        {
            double ratiodev = (1.0 * monitorRect.Width / monitorRect.Height) - (1.0 * img.Width / img.Height);
            int height = img.Height;
            int width = img.Width;

            Rectangle rect;
            if (ratiodev < 0)
            {
                rect = new Rectangle(0, 0, (int)((1.0 * monitorRect.Width / monitorRect.Height) * height), height);
                rect.X = (width - rect.Width) / 2;
            }
            else
            {
                rect = new Rectangle(0, 0, width, (int)(1.0 * width / (1.0 * monitorRect.Width / monitorRect.Height)));
                rect.Y = (height - rect.Height) / 2;
            }

            return ((Bitmap)img).Clone(rect, img.PixelFormat);

        }
    }
}