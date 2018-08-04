using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using WOL.Model;

namespace WOL.Droid.Adapters
{
    class DeviceListAdapter : ArrayAdapter<DeviceInfo>
    {
        private int resourceId;

        public DeviceListAdapter(Context context, int textViewResourceId, List<DeviceInfo> objects) : base(context, textViewResourceId, objects)
        {
            resourceId = textViewResourceId;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            DeviceInfo device = GetItem(position);

            View view = LayoutInflater.From(Context).Inflate(resourceId, null);

            TextView DeviceItemName = view.FindViewById<TextView>(Resource.Id.DeviceItemName);
            TextView DeviceItemIp = view.FindViewById<TextView>(Resource.Id.DeviceItemIp);
            TextView DeviceItemMac = view.FindViewById<TextView>(Resource.Id.DeviceItemMac);

            DeviceItemName.Text = device.Name;
            DeviceItemIp.Text = device.IpAddress;
            DeviceItemMac.Text = device.MacAddress;

            return view;
        }
    }
}