using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Sender
{
    class Program
    {

        public static TcpClient client;
        private static TcpListener listener;
        private static string ipString;
        private Screen screen = Screen.PrimaryScreen;
        private static float DPI = 1.25f;
        private static Cursor cursor = Cursors.Default;
        static Rectangle scrBounds = new Rectangle(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, (int)(Screen.PrimaryScreen.Bounds.Width * 1.25f), (int)(Screen.PrimaryScreen.Bounds.Height * 1.25f));
        static void Main(string[] args)
        {
            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipString = address.ToString();
                }
            }
            //Save Screenshot function  
            Bitmap SaveScreenshot()
            {
                var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                               Screen.PrimaryScreen.Bounds.Height,
                                               PixelFormat.Format32bppArgb);
                // Create a graphics object from the bitmap.  
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                // Take the screenshot from the upper left corner to the right   bottom corner.  
                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                            Screen.PrimaryScreen.Bounds.Y,
                                            0,
                                            0,
                                            Screen.PrimaryScreen.Bounds.Size,
                                            CopyPixelOperation.SourceCopy);
                return bmpScreenshot;
            }
            //Convert Image to byte type data.  
            void sendData(byte[] data, NetworkStream stream)
            {
                int bufferSize = 1024;

                byte[] dataLength = BitConverter.GetBytes(data.Length);
                stream.Write(dataLength, 0, 4);

                int bytesSent = 0;
                int bytesLeft = data.Length;

                while (bytesLeft > 0)
                {
                    int curDataSize = Math.Min(bufferSize, bytesLeft);

                    stream.Write(data, bytesSent, curDataSize);

                    bytesSent += curDataSize;
                    bytesLeft -= curDataSize;
                }
            }
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipString), 1234);
            listener = new TcpListener(ep);
            listener.Start();
            while (true)
            {
                Console.WriteLine(@"    
            ===================================================    
                   Started listening requests at: {0}:{1}    
            ===================================================",
            ep.Address, ep.Port);
                client = listener.AcceptTcpClient();
                Console.WriteLine("Connected to client!" + " \n");
                while (client.Connected)
                {
                    try
                    {
                        const int bytesize = 1024 * 1024;
                        byte[] buffer = new byte[bytesize];
                        string x = client.GetStream().Read(buffer, 0, bytesize).ToString();
                        var MEM = client.GetStream();

                        var data = ASCIIEncoding.ASCII.GetString(buffer);

                        if (data.ToUpper().Contains("TSC1"))
                        {
                            do
                            {
                                Console.WriteLine("Take Screenshot!" + " \n");
                                var bitmap = SaveScreenshot();
                                var stream = new MemoryStream();
                                bitmap.Save(stream, ImageFormat.Png);
                                sendData(stream.ToArray(), MEM);
                                stream.Dispose();
                                string z = client.GetStream().Read(buffer, 0, bytesize).ToString();
                            } while (client.Connected);
                        }
                    }
                    catch (Exception exc)
                    {
                        client.Dispose();
                        client.Close();
                    }
                }
            }

        }

    }
}
