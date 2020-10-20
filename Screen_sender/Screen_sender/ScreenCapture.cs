using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

namespace Screen_sender
{
    /// <summary>
    /// Mirroring -> same screen as primary <span/>
    /// Extend -> Additional space next to primary<span/>
    /// Only_second -> Only external screen is active
    /// </summary>
    enum ScreenMode
    {
        Mirroring = 1, 
        Extend = 2,
        Only_second = 3
    }

    class ScreenCapture
    {
        private float DPI;
        private Screen screen;
        Cursor cursor;
        Rectangle scrBounds;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        public ScreenCapture(float DPI)
        {
            this.DPI = DPI;
            this.screen = Screen.PrimaryScreen;
            this.cursor = Cursors.Default;
            this.scrBounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, (int)(screen.Bounds.Width * DPI), (int)(screen.Bounds.Height * DPI));
            //Console.WriteLine(Screen.PrimaryScreen.BitsPerPixel);
        }

        public static void turnOffScreen(int sleep_ms)
        {
            SendMessage(0xFFFF, 0x112, 0xF170, 2);
            Thread.Sleep(sleep_ms);
            SendMessage(0xFFFF, 0x112, 0xF170, -1);
        }

        public void saveScreenshot()
        {
            Rectangle scrBounds = Rectangle.FromLTRB(Screen.PrimaryScreen.Bounds.Left, Screen.PrimaryScreen.Bounds.Top, (int)(Screen.PrimaryScreen.Bounds.Right * DPI), (int)(Screen.PrimaryScreen.Bounds.Bottom * DPI));
            Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);

            // --------------< save image to file >-----------------
            ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 100L);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;
            using (Graphics g = Graphics.FromImage(desktopBMP))
            {
                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
            }
            desktopBMP.Save("screen_" + DateTime.Now.Year + '-' + DateTime.Now.DayOfYear + '-' + DateTime.Now.TimeOfDay + ".jpg", imageCodecInfo, encoderParameters);
        }

        public void saveScreenshot(Bitmap bmp)
        {
            ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 100L);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;
            bmp.Save("screen_" + DateTime.Now.Year + '-' + DateTime.Now.DayOfYear + '-' + DateTime.Now.TimeOfDay + ".jpg", imageCodecInfo, encoderParameters);
        }

        public Task<byte[]> captureNoMouseAsync()
        {
            return Task.Run(() =>
            {
                Rectangle curBounds = new Rectangle(new Point((int)(Cursor.Position.X * DPI - Cursor.Current.HotSpot.X), (int)(Cursor.Position.Y * DPI - Cursor.Current.HotSpot.Y)), cursor.Size);
                Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);

                // --------------< save image to file >-----------------

                Graphics g = Graphics.FromImage(desktopBMP);
                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                MemoryStream mss = new MemoryStream();
                desktopBMP.Save(mss, ImageFormat.Jpeg);
                return mss.ToArray();
                

                // ---------------< save smaller size >-----------------
                /*
                Graphics g2 = Graphics.FromImage(desktopBMP);
                g2.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                Bitmap b = new Bitmap(1280, 720);
                Graphics g = Graphics.FromImage(b);
                g.DrawImage(desktopBMP, 0, 0, 1280, 720);
                var mss = new MemoryStream(1280 * 720);
                b.Save(mss, ImageFormat.Jpeg);
                return mss.ToArray();
                */
            });
        }

        public Task<byte[]> captureWithMouseAsync()
        {
            return Task.Run(() =>
            {
                Rectangle curBounds = new Rectangle(new Point((int)(Cursor.Position.X * DPI - Cursor.Current.HotSpot.X), (int)(Cursor.Position.Y * DPI - Cursor.Current.HotSpot.Y)), cursor.Size);
                Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);

                // --------------< save image to file >-----------------

                Graphics g = Graphics.FromImage(desktopBMP);
                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                cursor.Draw(g,curBounds);
                MemoryStream mss = new MemoryStream();
                desktopBMP.Save(mss, ImageFormat.Jpeg);
                return mss.ToArray();
                

                // ---------------< save smaller size >-----------------
                /*
                Graphics g2 = Graphics.FromImage(desktopBMP);
                g2.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                cursor.Draw(g2, curBounds);
                Bitmap b = new Bitmap(1280, 720);
                Graphics g = Graphics.FromImage(b);
                g.DrawImage(desktopBMP, 0, 0, 1280, 720);
                var mss = new MemoryStream(1280 * 720);
                b.Save(mss, ImageFormat.Jpeg);
                return mss.ToArray();
                */
            });
        }
    }
}
