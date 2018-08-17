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
    [Activity(Label = "@string/app_name", Theme = "@android:style/Theme.NoDisplay", ExcludeFromRecents = true)]
    class WakeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Intent intent = Intent;
            string broadcast = intent.GetStringExtra("BroadcastAddress");
            string[] macStr = intent.GetStringExtra("MacAddress").Split('-');
            int sendingCount = intent.GetIntExtra("SendingCount", 1);
            int port = intent.GetIntExtra("Port", 7);

            byte[] mac = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                mac[i] = Convert.ToByte(macStr[i], 16);
            }

            for (int i = 0; i < sendingCount; i++)
            {
                WolManager.Wake(broadcast, port, mac);
            }

            Toast.MakeText(this, GetString(Resource.String.wake_success), ToastLength.Long).Show();

            Finish();
        }

    }
}