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

        public byte[] captureWithMouse()
        {
            var allBounds = Screen.AllScreens.Select(s => s.Bounds).ToArray();
            Cursor cur = Cursor.Current;
            Rectangle scrBounds = Rectangle.FromLTRB(allBounds.Min(b => b.Left), allBounds.Min(b => b.Top), (int) (allBounds.Max(b => b.Right) * DPI), (int) (allBounds.Max(b => b.Bottom) * DPI));
            //Console.WriteLine(allBounds.Length);
            Rectangle curBounds = new Rectangle(Cursor.Position, cur.Size);
            Bitmap desktopBMP = new Bitmap(scrBounds.Width, scrBounds.Height);
            Bitmap cursorBMP = new Bitmap(cur.Size.Width, cur.Size.Height);

            // --------------< save image to file >-----------------

            using (Graphics g = Graphics.FromImage(desktopBMP))
            {

                g.CopyFromScreen(scrBounds.Location, Point.Empty, scrBounds.Size);
                cur.Draw(g,curBounds);      
            }

            ImageConverter conv = new ImageConverter();
        
            return (byte[])conv.ConvertTo(desktopBMP, typeof(byte[]));

        }
    }
}
