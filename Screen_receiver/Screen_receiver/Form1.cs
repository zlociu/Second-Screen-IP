﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace Screen_receiver
{
    public partial class Form1 : Form
    {
        public void listenTask()
        {
            while (true)
            {
                TcpListener receiver = new TcpListener(IPAddress.Any, 11308);
                receiver.Start();
                TcpClient client = receiver.AcceptTcpClient();
                client.ReceiveTimeout = 500;
                byte[] data = new byte[1920 * 1080 * 4];
                MemoryStream m1 = new MemoryStream(data, 0, data.Length);
                //GZipStream zipStream = new GZipStream(client.GetStream(), CompressionMode.Decompress);
                while (true)
                {
                    try
                    {
                        client.GetStream().Read(data, 0, data.Length);
                        Image img = Image.FromStream(m1);
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            pictureBox1.Image = img;
                            pictureBox1.Refresh();
                        });
                    }
                    catch (IOException)
                    {
                        pictureBox1.Invoke((MethodInvoker)delegate
                        {
                            receiver.Stop();
                            Bitmap flag = new Bitmap(pictureBox1.Size.Width, pictureBox1.Height);
                            Graphics flagGraphics = Graphics.FromImage(flag);
                            flagGraphics.FillRectangle(Brushes.LightGray, 0, 0, flag.Width, flag.Height);
                            flagGraphics.DrawString("Waiting connection . . .", new Font("Microsoft Tai Le", 40), Brushes.DeepSkyBlue, new Point(350, 300));
                            pictureBox1.Image = flag;
                            pictureBox1.Refresh();
                        });
                        break;
                    }
                    catch (Exception)
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.AutoSize = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap flag = new Bitmap(pictureBox1.Size.Width, pictureBox1.Height);
            Graphics flagGraphics = Graphics.FromImage(flag);

            flagGraphics.FillRectangle(Brushes.LightGray, 0, 0, flag.Width, flag.Height);
            flagGraphics.DrawString("Waiting connection . . .", new Font("Microsoft Tai Le", 40), Brushes.DeepSkyBlue, new Point(350,300));
            pictureBox1.Image = flag;
            pictureBox1.Refresh();
            var t = new Thread(listenTask);
            t.IsBackground = true;
            t.Start();
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            pictureBox1.Size = Size;
        }

    }
}
