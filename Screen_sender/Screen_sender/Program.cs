using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.IO;


namespace Screen_sender
{
    class Program
    {

        static int ipPort;
        static string ipAddr;

        static bool connectionEnd;

        static async Task sendScreenData(int delay_ms)
        {
            ScreenCapture screen = new ScreenCapture(1.25f);
            TcpClient sender = new TcpClient(new IPEndPoint(IPAddress.Parse(ipAddr), 0));
            sender.Connect(IPAddress.Parse(ipAddr), ipPort);
            //GZipStream zipStream = new GZipStream(sender.GetStream(), CompressionMode.Compress);
            while (!connectionEnd)
            {
                byte[] data = screen.captureWithMouse();
                
                await sender.GetStream().WriteAsync(data, 0, data.Length);
                Console.WriteLine("wyslano " + data.Length + " bajtow");
                Thread.Sleep(delay_ms);
            }
        }

        static Task serverControl()
        {
            Console.WriteLine("jkk");
            while (true)
            {
                string command = Console.ReadLine();
                var tab = command.Split(new char[]{' '}, 2);
                int len = tab.Length;
                switch(tab[0])
                {
                    case "start":
                        {
                            ipAddr = tab[1];
                            ipPort = int.Parse(tab[2]);

                        }
                        break;
                    case "stop":
                        {
                            connectionEnd = true;
                        }
                        break;
                    case "shutdown":
                        {
                            connectionEnd = true;
                            Environment.Exit(0);
                        }
                        break;
                    case "change":
                        {
                            switch (tab[1])
                            {
                                case "": break;
                                default: break;
                            }
                        }
                        break;
                    case "help":
                        {
                            Console.WriteLine("Second Screen IP helpdesk");
                            Console.WriteLine("List of commands:");
                            Console.WriteLine("start <ipAddress> <tcpPort> <mode>");
                            Console.WriteLine("stop");
                            Console.WriteLine("change <mode>");
                        }
                        break;
                    default: Console.WriteLine("unknown command"); break;
                }
            }
        }

        static void Main(string[] args)
        {
            ipPort = 11308;
            ipAddr = "127.0.0.1";
            connectionEnd = false;
            //ScreenCapture sc = new ScreenCapture(1.25f);
            //byte[] array = sc.captureWithMouse();
            //Console.WriteLine(array.Length); //66960 B

            //ScreenCapture.turnOffScreen(1000);
            Task t1 = sendScreenData(17);
            //Task t2 = serverControl();
            t1.Wait();
            //t2.Wait();
        }
    }
}
