using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace Receiver
{
    [Activity(Label = "Control")]
    public class Control : Activity
    {
        //Instances   
        private Button btnLogout;
        private ImageView imageView;
        NetworkStream stream;
        TcpClient client = Connection.Instance.client;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Control);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);

            ThreadPool.QueueUserWorkItem(o => MethodToRun());

            btnLogout.Click += delegate
            {
                StartActivity(typeof(Connect));
                client.Close();
            };
        }

        void MethodToRun()
        {
            stream = client.GetStream();
            String s = "TSC1";
            byte[] message = Encoding.ASCII.GetBytes(s);
            stream.Write(message, 0, message.Length);
            while (true)
            {
                var data = getData(client);
                Android.Graphics.Bitmap image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                RunOnUiThread(() => imageView.SetImageBitmap(image));
                stream.Write(message, 0, message.Length);
            }
        }
        public byte[] getData(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] fileSizeBytes = new byte[4];
            int bytes = stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int buffersize = 2048;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(buffersize, bytesLeft);
                if (client.Available < curDataSize)
                    curDataSize = client.Available;//This save me  

                bytes = stream.Read(data, bytesRead, curDataSize);
                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }
            return data;
        }
        public void getimg(TcpClient client)
        {
            stream = client.GetStream();
            String s = "TSC1";
            byte[] message = Encoding.ASCII.GetBytes(s);
            stream.Write(message, 0, message.Length);
            while (true)
            {
                var data = getData(client);
                var image = BitmapFactory.DecodeByteArray(data, 0, data.Length);

                imageView.SetImageBitmap(image);
                imageView.Invalidate();

            }
        }

    }
}








