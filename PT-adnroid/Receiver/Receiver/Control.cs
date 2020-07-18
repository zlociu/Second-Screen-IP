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
        private Button btnTakeScreen, btnLogout;
        private ImageView imageView;
        NetworkStream stream;
        TcpClient client = Connection.Instance.client;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Control);
            btnTakeScreen = FindViewById<Button>(Resource.Id.btnTakeScreen);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);


            //var thread = new Thread(() => MethodToRun(client));
            //thread.Start();
            ThreadPool.QueueUserWorkItem(o => MethodToRun());

            //Take Screenshot command button  


            btnTakeScreen.Click += async delegate
            {
                getimg(client);
                //stream = client.GetStream();
                //String s = "TSC1";
                //byte[] message = Encoding.ASCII.GetBytes(s);
                //stream.Write(message, 0, message.Length);
                //while (true)
               // {
                    //var data = getData(client);
                    //var image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                    //imageView.SetImageBitmap(image);
                    //message = Encoding.ASCII.GetBytes(s);
                    //stream.Write(message, 0, message.Length);
                //}
            };
            //Logout button  
            btnLogout.Click += delegate
            {
                StartActivity(typeof(MainActivity));
                client.Close();
            };
        }
        //Convert byte to Image  
        void MethodToRun()
        {
            stream = client.GetStream();
            String s = "TSC1";
            byte[] message = Encoding.ASCII.GetBytes(s);
            stream.Write(message, 0, message.Length);
            while (true)
            {
                var data = getData(client);
                var image = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                RunOnUiThread(() => imageView.SetImageBitmap(image));
                //update(image, imageView);
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

            int buffersize = 1024;
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
                //RunOnUiThread(() => { imageView.SetImageBitmap(image); });
                imageView.SetImageBitmap(image);
                imageView.Invalidate();

                //message = Encoding.ASCII.GetBytes(s);
                //stream.Write(message, 0, message.Length);
            }
        }
        public void update(Bitmap image, ImageView v)
        {
            imageView.SetImageBitmap(image);
        }
    }
}








