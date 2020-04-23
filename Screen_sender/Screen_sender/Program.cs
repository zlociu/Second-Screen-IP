using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Screen_sender
{
    class Program
    {

        static int ipPort;
        static string ipAddr;

        static bool connectionEnd = false;

        static async Task sendScreenData(int delay_ms)
        {
            ScreenCapture screen = new ScreenCapture(1.25f);
            TcpClient sender = new TcpClient(new IPEndPoint(IPAddress.Parse(ipAddr), ipPort));
            while(!connectionEnd)
            {
                byte[] data = screen.captureWithMouse(1.25f);
                await sender.GetStream().WriteAsync(null, 0, 0);
                Thread.Sleep(17);
            }
        }

        static Task serverControl()
        {
            Console.WriteLine("jkk");
            return null;
        }

        static void Main(string[] args)
        {
            ipPort = 11309;
            ipAddr = "127.0.0.1";
            connectionEnd = false;
            ScreenCapture sc = new ScreenCapture(1.25f);
            byte[] array = sc.captureWithMouse();
            //Task t1 = sendScreenData();
            //Task t2 = serverControl();
            //t1.Wait();
            //t2.Wait();
        }
    }
}
