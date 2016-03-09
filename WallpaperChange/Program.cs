using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TweetSharp;
using System.Threading;

namespace WallpaperChange
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var image = GetTimeLineImage("mohitverma3222");
                if (!string.IsNullOrEmpty(image))
                    Set(new Uri(image), Style.Stretched);
                Thread.Sleep(10000);
            }
        }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(Uri uri, Style style)
        {
            System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

        }

        public static string GetTimeLineImage(string screenName)
        {
            try
            {
                var service = new TwitterService("", "");
                service.AuthenticateWith("", "");

                var options = new ListTweetsOnUserTimelineOptions();
                options.ScreenName = screenName;
                options.Count = 5;
                var currentTweets = service.ListTweetsOnUserTimeline(options);
                return currentTweets.ToList()[0].Entities.Media.ToList()[0].MediaUrlHttps;
            }

            catch (Exception ex)
            {
                return "";
            }


        }

    }
}
