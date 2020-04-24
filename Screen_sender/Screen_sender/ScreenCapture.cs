﻿using System;
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

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        public ScreenCapture(float DPI)
        {
            this.DPI = DPI;
            Console.WriteLine(Screen.PrimaryScreen.BitsPerPixel);
        }
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
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
            ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
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
            ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 100L);
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;
            bmp.Save("screen_" + DateTime.Now.Year + '-' + DateTime.Now.DayOfYear + '-' + DateTime.Now.TimeOfDay + ".jpg", imageCodecInfo, encoderParameters);
        }

        public byte[] captureNoMouse()
        {
            Screen scr = Screen.PrimaryScreen;
            Rectangle scrBounds = Rectangle.FromLTRB(scr.Bounds.Left, scr.Bounds.Top, (int)(scr.Bounds.Right * DPI), (int)(scr.Bounds.Bottom * DPI));
            //Console.WriteLine(allBounds.Length);
            Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);

            // --------------< save image to file >-----------------
            using (Graphics g = Graphics.FromImage(desktopBMP))
            {
                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
            }
            using (var mss = new MemoryStream())
            {
                ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = encoderParameter;
                desktopBMP.Save(mss, imageCodecInfo, encoderParameters);
                return mss.ToArray();
            }
        }

        public byte[] captureWithMouse()
        {
            Screen scr = Screen.PrimaryScreen;
            Cursor cur = Cursors.Default;
            Rectangle scrBounds = Rectangle.FromLTRB(scr.Bounds.Left, scr.Bounds.Top, (int) (scr.Bounds.Right * DPI), (int) (scr.Bounds.Bottom * DPI));
            //Console.WriteLine(allBounds.Length);
            Rectangle curBounds = new Rectangle(new Point((int)(Cursor.Position.X*DPI - Cursor.Current.HotSpot.X), (int)(Cursor.Position.Y*DPI - Cursor.Current.HotSpot.Y)), cur.Size);
            Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);

            // --------------< save image to file >-----------------
            using (Graphics g = Graphics.FromImage(desktopBMP))
            {
                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                cur.Draw(g,curBounds);      
            }
            using (var mss = new MemoryStream())
            {
                ImageCodecInfo imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = encoderParameter;
                desktopBMP.Save(mss, imageCodecInfo, encoderParameters);
                return mss.ToArray();
            }
        }
    }
}
