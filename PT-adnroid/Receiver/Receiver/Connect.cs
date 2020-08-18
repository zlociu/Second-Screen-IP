using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Receiver
{
    [Activity(Label = "Connect")]
    public class Connect : Activity
    {
        private EditText edtIp;
        private Button btnConnect, btnLogout;
        private TcpClient client;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            client = new TcpClient();
            SetContentView(Resource.Layout.Connect);
            edtIp = FindViewById<EditText>(Resource.Id.edtIpAddress);
            btnConnect = FindViewById<Button>(Resource.Id.btnConnect);

            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            btnLogout.Click += async delegate
            {
                StartActivity(typeof(MainActivity));
            };

            btnConnect.Click += async delegate
            {
                try
                {
                    client.Connect(edtIp.Text, 1234);
                    if (client.Connected)
                    {
                        Connection.Instance.client = client;
                        Toast.MakeText(this, "Client connected to server!", ToastLength.Short).Show();
                        Intent intent = new Intent(this, typeof(Control));
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, "Connection failed!", ToastLength.Short).Show();
                    }
                }
                catch (Exception x)
                {
                    Toast.MakeText(this, "Connection failed!", ToastLength.Short).Show();
                }
            };
        }
    }
}