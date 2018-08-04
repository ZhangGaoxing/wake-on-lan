using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using WOL.Model;
using WOL.Utility;

namespace WOL.Droid.Activities
{
    [Activity(Label = "AboutActivity")]
    class AddActivity : Activity
    {
        EditText DeviceName;

        EditText DeviceMac1;
        EditText DeviceMac2;
        EditText DeviceMac3;
        EditText DeviceMac4;
        EditText DeviceMac5;
        EditText DeviceMac6;

        EditText DeviceIp1;
        EditText DeviceIp2;
        EditText DeviceIp3;
        EditText DeviceIp4;

        EditText DeviceBroadcast1;
        EditText DeviceBroadcast2;
        EditText DeviceBroadcast3;
        EditText DeviceBroadcast4;

        EditText DeviceDesc;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Add);

            DeviceName = FindViewById<EditText>(Resource.Id.DeviceName);

            DeviceMac1 = FindViewById<EditText>(Resource.Id.DeviceMac1);
            DeviceMac2 = FindViewById<EditText>(Resource.Id.DeviceMac2);
            DeviceMac3 = FindViewById<EditText>(Resource.Id.DeviceMac3);
            DeviceMac4 = FindViewById<EditText>(Resource.Id.DeviceMac4);
            DeviceMac5 = FindViewById<EditText>(Resource.Id.DeviceMac5);
            DeviceMac6 = FindViewById<EditText>(Resource.Id.DeviceMac6);

            DeviceIp1 = FindViewById<EditText>(Resource.Id.DeviceIp1);
            DeviceIp2 = FindViewById<EditText>(Resource.Id.DeviceIp2);
            DeviceIp3 = FindViewById<EditText>(Resource.Id.DeviceIp3);
            DeviceIp4 = FindViewById<EditText>(Resource.Id.DeviceIp4);

            DeviceBroadcast1 = FindViewById<EditText>(Resource.Id.DeviceBroadcast1);
            DeviceBroadcast2 = FindViewById<EditText>(Resource.Id.DeviceBroadcast2);
            DeviceBroadcast3 = FindViewById<EditText>(Resource.Id.DeviceBroadcast3);
            DeviceBroadcast4 = FindViewById<EditText>(Resource.Id.DeviceBroadcast4);

            DeviceDesc = FindViewById<EditText>(Resource.Id.DeviceDesc);
        }

        private void DeviceSave_Click(object sender, EventArgs e)
        {
            DeviceInfo device = new DeviceInfo
            {
                Name = DeviceName.Text,
                MacAddress = $"{Convert.ToByte(DeviceMac1.Text, 16)}-{Convert.ToByte(DeviceMac2.Text, 16)}-{Convert.ToByte(DeviceMac3.Text, 16)}-{Convert.ToByte(DeviceMac4.Text, 16)}-{Convert.ToByte(DeviceMac5.Text, 16)}-{Convert.ToByte(DeviceMac6.Text, 16)}",
                IpAddress = $"{Convert.ToByte(DeviceIp1.Text)}.{Convert.ToByte(DeviceIp2.Text)}.{Convert.ToByte(DeviceIp3.Text)}.{Convert.ToByte(DeviceIp4.Text)}",
                BroadcastAddress = $"{Convert.ToByte(DeviceBroadcast1.Text)}.{Convert.ToByte(DeviceBroadcast1.Text)}.{Convert.ToByte(DeviceBroadcast1.Text)}.{Convert.ToByte(DeviceBroadcast1.Text)}",
                Description = DeviceDesc.Text,
            };

            SqliteManager<DeviceInfo> sqlite = new SqliteManager<DeviceInfo>();
            bool res = false;
            try
            {
                res = sqlite.Insert(device);
            }
            catch (Exception)
            {
                sqlite.CreateTable();
                res = sqlite.Insert(device);
            }
        }
    }
}