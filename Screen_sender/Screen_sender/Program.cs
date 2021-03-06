﻿using System;
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
        static int resolution;
        static float dpi;
        static bool mouse;

        static bool connectionEnd;

        static async Task sendScreenDataAsync(int delay_ms)
        {
            ScreenCapture screen = new ScreenCapture(dpi);
            TcpClient sender = new TcpClient(new IPEndPoint(IPAddress.Parse(ipAddr), 0));
            sender.Connect(IPAddress.Parse(ipAddr), ipPort);
            
            //int it = 0;
            while (!connectionEnd)
            {
                await screen.captureWithMouseAsync().ContinueWith(
                    async (data) =>
                    {
                        await sender.GetStream().WriteAsync(data.Result, 0, data.Result.Length);
                    });  
                //Console.WriteLine(++it);
                Thread.Sleep(1);
            }
            sender.Close();
        }

        static async Task sendScreenDataNoMouseAsync(int delay_ms)
        {
            ScreenCapture screen = new ScreenCapture(dpi);
            TcpClient sender = new TcpClient(new IPEndPoint(IPAddress.Parse(ipAddr), 0));
            sender.Connect(IPAddress.Parse(ipAddr), ipPort);
            
            //int it = 0;
            while (!connectionEnd)
            {
                await screen.captureNoMouseAsync().ContinueWith(
                    async (data) =>
                    {
                        await sender.GetStream().WriteAsync(data.Result, 0, data.Result.Length);
                    });
                //Console.WriteLine(++it);
                Thread.Sleep(1);
            }
            sender.Close();
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
                                Task t1;
                                if (mouse == true) t1 = sendScreenDataAsync((int)(1000 / fps));
                                else t1 = sendScreenDataNoMouseAsync((int)(1000 / fps));
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
                            Thread.Sleep(300);
                            try
                            {
                                connectionEnd = false;
                                Task t1;
                                if (mouse == true) t1 = sendScreenDataAsync((int)(1000 / fps));
                                else t1 = sendScreenDataNoMouseAsync((int)(1000 / fps));
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
                                        case "--dpi":
                                            {
                                                try
                                                {
                                                    dpi = float.Parse(tab[i + 1]);
                                                    Console.WriteLine("Dpi: " + dpi);
                                                }
                                                catch (Exception) { Console.WriteLine("error command"); }
                                            }
                                            break;
                                        case "--mouse":
                                            {

                                                mouse = (mouse != true);
                                                var _is = (mouse == true ? "On" : "Off");
                                                Console.WriteLine("Mouse cursor: "+ _is);
                                            }
                                            break;
                                        default:
                                            {
                                                Console.WriteLine("error command");
                                            }
                                            break;
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
                            Console.WriteLine("Resolution: " + resolution + "p");
                            Console.WriteLine("Dpi: " + dpi);
                            Console.WriteLine("Mouse: " + mouse.ToString());
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
                                Console.WriteLine("restart");
                                Console.WriteLine("tech");
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
                                            Console.WriteLine("--res <resY> ");
                                            Console.WriteLine("--dpi <value (e.g. 1,25)>  ");
                                            Console.WriteLine("--mouse");
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
            Console.WindowHeight = 17;
            Console.WindowWidth = 60;
            ipPort = 11308;
            ipAddr = "127.0.0.1";
            fps = 30;
            resolution = 720;
            connectionEnd = false;
            dpi = 1.25f;
            mouse = true;
            
            //Task t1 = sendScreenData(17);
            Task t2 = serverControl();
            //t1.Wait();
            t2.Wait();
        }
    }
}
