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
        static int fps;
        static int quality;
        static int resolution;

        static bool connectionEnd;

        static async void sendScreenDataAsync(int delay_ms)
        {
            ScreenCapture screen = new ScreenCapture(1.0f);
            TcpClient sender = new TcpClient(new IPEndPoint(IPAddress.Parse(ipAddr), 0));
            sender.Connect(IPAddress.Parse(ipAddr), ipPort);
            //GZipStream zipStream = new GZipStream(sender.GetStream(), CompressionMode.Compress);
            while (!connectionEnd)
            {
                byte[] data = screen.captureWithMouse();
                
                await sender.GetStream().WriteAsync(data, 0, data.Length);
                //Console.WriteLine("wyslano " + data.Length + " bajtow");
                Thread.Sleep(delay_ms);
            }
        }

        static Task serverControl()
        {
            Console.WriteLine("Send client is running");
            Console.WriteLine("Type 'help' for more options");

            while (true)
            {
                string command = Console.ReadLine();
                var tab = command.Split(' ');
                int len = tab.Length;
                switch(tab[0])
                {
                    case "start":
                        {
                            try
                            {
                                connectionEnd = false;
                                Task t1 = new Task(()=> { sendScreenDataAsync((int)(1000 / fps)); });
                                t1.Start();
                            }
                            catch (Exception) { }
                        }
                        break;
                    case "stop":
                        {
                            connectionEnd = true;
                        }
                        break;
                    case "restart":
                        {
                            connectionEnd = true;
                            Thread.Sleep(1000);
                            try
                            {
                                connectionEnd = false;
                                Task t1 = new Task(() => { sendScreenDataAsync((int)(1000 / fps)); });
                                t1.Start();
                            }
                            catch (Exception) { }
                        }
                        break;
                    case "shutdown":
                        {
                            connectionEnd = true;
                            Environment.Exit(0);
                        }
                        break;
                    case "setup":
                        {
                            if (len % 2 == 1)
                            {
                                for (int i = 1; i < len; i = i + 2)
                                {
                                    switch (tab[i])
                                    {
                                        case "--addr":
                                            {
                                                try
                                                {
                                                    if (tab[i + 1].StartsWith("--")) throw new NotImplementedException();
                                                    ipAddr = tab[i+1];
                                                    Console.WriteLine("IP address: " + ipAddr);
                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        case "--port":
                                            {
                                                try
                                                {
                                                    ipPort = int.Parse(tab[i+1]);
                                                    Console.WriteLine("TCP port: " + ipPort);

                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        case "--fps":
                                            {
                                                try
                                                {
                                                    fps = int.Parse(tab[i+1]);
                                                    Console.WriteLine("FPS: " + fps);

                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        case "--qual":
                                            {
                                                try
                                                {
                                                    quality = int.Parse(tab[i+1]);
                                                    Console.WriteLine("JPEG Quality: " + quality);

                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        case "--res":
                                            {
                                                try
                                                {
                                                    resolution = int.Parse(tab[i+1]);
                                                    Console.WriteLine("Resolution: " + resolution + "p");
                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        default: break;
                                    }
                                }
                            }
                        }
                        break;
                    case "tech":
                        {
                            Console.WriteLine("Second Screen IP current settings: ");
                            Console.WriteLine("IP address: " + ipAddr);
                            Console.WriteLine("TCP port: " + ipPort);
                            Console.WriteLine("FPS: " + fps);
                            Console.WriteLine("Video quality (0-100%): " + quality + "%");
                            Console.WriteLine("Resolution: " + resolution + "p");
                        }
                        break;
                    case "help":
                        {

                            if(len==1)
                            {
                                Console.WriteLine("Second Screen IP helpdesk");
                                Console.WriteLine("List of commands:");
                                Console.WriteLine("start <ipAddress> <tcpPort> <mode>");
                                Console.WriteLine("stop");
                                Console.WriteLine("setup [--<param> <value>]");
                            }
                            else
                            {
                                switch(tab[1])
                                {
                                    case "":
                                        {

                                        }
                                        break;
                                    case "setup":
                                        {

                                            Console.WriteLine("change available parameters:");
                                            Console.WriteLine("--addr <ipv4_address>");
                                            Console.WriteLine("--port <tcp_port>");
                                            Console.WriteLine("--fps [25 | 30 | 50 | 60]");
                                            Console.WriteLine("--quality <value> (value 0-100)");
                                            Console.WriteLine("--res <resY> ");
                                        }
                                        break;
                                    case "1":
                                        {

                                        }
                                        break;
                                    default:break;
                                }
                            }
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
            fps = 30;
            quality = 100;
            resolution = 1080;
            connectionEnd = false;
            //ScreenCapture sc = new ScreenCapture(1.25f);
            //byte[] array = sc.captureWithMouse();
            //Console.WriteLine(array.Length); //66960 B

            //ScreenCapture.turnOffScreen(1000);
            //Task t1 = sendScreenData(17);
            Task t2 = serverControl();
            //t1.Wait();
            t2.Wait();
        }
    }
}
