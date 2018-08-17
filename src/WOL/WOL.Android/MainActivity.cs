using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Views;
using Android.Content;
using WOL.Droid.Activities;
using WOL.Model;
using WOL.Utility;
using WOL.Droid.Adapters;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace WOL.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        List<DeviceInfo> data;
        SqliteManager<DeviceInfo> sqlite = new SqliteManager<DeviceInfo>();

        DeviceListAdapter adapter;
        ListView DeviceList;

        TextView Tips;

        CancellationTokenSource scanCTS = new CancellationTokenSource();

        #region Add View
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

        EditText DevicePort;
        EditText SendingCount;

        EditText DeviceDesc;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Tips = FindViewById<TextView>(Resource.Id.Tips);
            Tips.Click += (s, e) =>
            {
                ShowScanDialog();
            };

            DeviceList = FindViewById<ListView>(Resource.Id.DeviceList);

            try
            {
                data = sqlite.QueryAll();
            }
            catch
            {
                sqlite.CreateTable();
                data = sqlite.QueryAll();
            }

            if (data.Count != 0)
            {
                DeviceList.Visibility = ViewStates.Visible;
                Tips.Visibility = ViewStates.Gone;
            }

            adapter = new DeviceListAdapter(this, Resource.Layout.device_list_item, data);
            DeviceList.Adapter = adapter;
            DeviceList.ItemLongClick += (s, e) =>
            {
                Wake(e);
            };
            DeviceList.ItemClick += (s, e) =>
            {
                Edit(e);
            };
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.About:
                    Intent intent = new Intent(this, typeof(AboutActivity));
                    StartActivity(intent);
                    break;
                case Resource.Id.Scan:
                    ShowScanDialog();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Add();
        }

        private async void ShowScanDialog()
        {
            View view = View.Inflate(this, Resource.Layout.Scan, null);
            TextView ScaningDetail = view.FindViewById<TextView>(Resource.Id.ScaningDetail);

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetView(view)
                .SetCancelable(false)
                .Create();
            AlertDialog show = alertDialog.Show();

            var token = scanCTS.Token;
            Task t1 = Task.Run(() =>
            {
                NetworkManager.GetNetworkIpAndMask(out IPAddress ip, out IPAddress sub);
                var networkAddress = NetworkManager.CalNetworkAddress(ip, sub).ToString();
                networkAddress = networkAddress.Substring(0, networkAddress.Length - 1);

                for (int i = 2; i <= 254; i = i + 1)
                {
                    string addr = $"{networkAddress}{i}";
                    UdpClient udp = new UdpClient(addr, 23452);
                    udp.Send(new byte[] { 0 }, 1);
                    ScaningDetail.Text = addr;
                }
            }, token);

            await t1;

            TaskScheduler csc = TaskScheduler.FromCurrentSynchronizationContext();
            await t1.ContinueWith((t) =>
            {
                var raw = NetworkManager.GetClientMac();

                foreach (var item in raw)
                {
                    DeviceInfo device = new DeviceInfo
                    {
                        Name = item[0],
                        MacAddress = item[1],
                        IpAddress = item[0],
                        BroadcastAddress = NetworkManager.CalBroadcast(IPAddress.Parse(item[0]), NetworkManager.CalSubnetMask(IPAddress.Parse(item[0]))).ToString(),
                        Port = 7,
                        SendingCount = 1,
                    };

                    bool isEqual = false;
                    foreach (var d in data)
                    {
                        if (d.IpAddress == device.IpAddress)
                        {
                            isEqual = true;
                        }
                    }
                    if (!isEqual)
                    {
                        sqlite.Insert(device);
                    }
                }

                data = sqlite.QueryAll();
                adapter = new DeviceListAdapter(this, Resource.Layout.device_list_item, data);
                DeviceList.Adapter = adapter;

                if (data.Count != 0)
                {
                    DeviceList.Visibility = ViewStates.Visible;
                    Tips.Visibility = ViewStates.Gone;
                }

                show.Dismiss();
            }, token, TaskContinuationOptions.AttachedToParent, csc);
        }

        private bool TestDataFormat()
        {
            try
            {
                Convert.ToByte(DeviceMac1.Text, 16);
                Convert.ToByte(DeviceMac2.Text, 16);
                Convert.ToByte(DeviceMac3.Text, 16);
                Convert.ToByte(DeviceMac4.Text, 16);
                Convert.ToByte(DeviceMac5.Text, 16);
                Convert.ToByte(DeviceMac6.Text, 16);

                Convert.ToByte(DeviceIp1.Text);
                Convert.ToByte(DeviceIp2.Text);
                Convert.ToByte(DeviceIp3.Text);
                Convert.ToByte(DeviceIp4.Text);

                Convert.ToByte(DeviceBroadcast1.Text);
                Convert.ToByte(DeviceBroadcast2.Text);
                Convert.ToByte(DeviceBroadcast3.Text);
                Convert.ToByte(DeviceBroadcast4.Text);

                Convert.ToByte(DevicePort.Text);
                Convert.ToByte(SendingCount.Text);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void InitViewControlsContents(DeviceInfo device)
        {
            string[] mac = device.MacAddress.Split('-');
            string[] ip = device.IpAddress.Split('.');
            string[] broadcast = device.BroadcastAddress.Split('.');

            DeviceName.Text = device.Name;

            DeviceMac1.Text = mac[0];
            DeviceMac2.Text = mac[1];
            DeviceMac3.Text = mac[2];
            DeviceMac4.Text = mac[3];
            DeviceMac5.Text = mac[4];
            DeviceMac6.Text = mac[5];

            DeviceIp1.Text = ip[0];
            DeviceIp2.Text = ip[1];
            DeviceIp3.Text = ip[2];
            DeviceIp4.Text = ip[3];

            DeviceBroadcast1.Text = broadcast[0];
            DeviceBroadcast2.Text = broadcast[1];
            DeviceBroadcast3.Text = broadcast[2];
            DeviceBroadcast4.Text = broadcast[3];

            DevicePort.Text = device.Port.ToString();
            SendingCount.Text = device.SendingCount.ToString();

            DeviceDesc.Text = device.Description;
        }

        private void InitViewControls(View view)
        {
            DeviceName = view.FindViewById<EditText>(Resource.Id.DeviceName);

            DeviceMac1 = view.FindViewById<EditText>(Resource.Id.DeviceMac1);
            DeviceMac2 = view.FindViewById<EditText>(Resource.Id.DeviceMac2);
            DeviceMac3 = view.FindViewById<EditText>(Resource.Id.DeviceMac3);
            DeviceMac4 = view.FindViewById<EditText>(Resource.Id.DeviceMac4);
            DeviceMac5 = view.FindViewById<EditText>(Resource.Id.DeviceMac5);
            DeviceMac6 = view.FindViewById<EditText>(Resource.Id.DeviceMac6);

            DeviceIp1 = view.FindViewById<EditText>(Resource.Id.DeviceIp1);
            DeviceIp2 = view.FindViewById<EditText>(Resource.Id.DeviceIp2);
            DeviceIp3 = view.FindViewById<EditText>(Resource.Id.DeviceIp3);
            DeviceIp4 = view.FindViewById<EditText>(Resource.Id.DeviceIp4);

            DeviceBroadcast1 = view.FindViewById<EditText>(Resource.Id.DeviceBroadcast1);
            DeviceBroadcast2 = view.FindViewById<EditText>(Resource.Id.DeviceBroadcast2);
            DeviceBroadcast3 = view.FindViewById<EditText>(Resource.Id.DeviceBroadcast3);
            DeviceBroadcast4 = view.FindViewById<EditText>(Resource.Id.DeviceBroadcast4);

            DevicePort = view.FindViewById<EditText>(Resource.Id.DevicePort);
            SendingCount = view.FindViewById<EditText>(Resource.Id.SendingCount);

            DeviceDesc = view.FindViewById<EditText>(Resource.Id.DeviceDesc);
        }

        private void Wake(AdapterView.ItemLongClickEventArgs e)
        {
            DeviceInfo device = data[e.Position];

            byte[] mac = new byte[6];
            string[] macStr = device.MacAddress.Split('-');
            for (int i = 0; i < 6; i++)
            {
                mac[i] = Convert.ToByte(macStr[i], 16);
            }

            for (int i = 0; i < device.SendingCount; i++)
            {
                WolManager.Wake(device.BroadcastAddress, device.Port, mac);
            }
            Toast.MakeText(this, GetString(Resource.String.wake_success), ToastLength.Long).Show();
        }

        private void Add()
        {
            View view = View.Inflate(this, Resource.Layout.Add, null);

            InitViewControls(view);

            view.ViewAttachedToWindow += (s, e) =>
            {
                DeviceIp4.FocusChange += (_s, _e) =>
                {
                    if (_e.HasFocus == false)
                    {
                        try
                        {
                            IPAddress ip = IPAddress.Parse($"{DeviceIp1.Text}.{DeviceIp2.Text}.{DeviceIp3.Text}.{DeviceIp4.Text}");

                            IPAddress mask = NetworkManager.CalSubnetMask(ip);
                            IPAddress broadcast = NetworkManager.CalBroadcast(ip, mask);

                            var bStr = broadcast.ToString().Split('.');

                            DeviceBroadcast1.Text = bStr[0];
                            DeviceBroadcast2.Text = bStr[1];
                            DeviceBroadcast3.Text = bStr[2];
                            DeviceBroadcast4.Text = bStr[3];
                        }
                        catch
                        {

                        }
                    }
                };
            };

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetView(view)
                .SetPositiveButton(GetString(Resource.String.add_device_save), (s, e) =>
                {
                    if (!TestDataFormat())
                    {
                        Toast.MakeText(this, GetString(Resource.String.add_device_error), ToastLength.Long).Show();

                        return;
                    }

                    DeviceInfo device = new DeviceInfo
                    {
                        Name = DeviceName.Text,
                        MacAddress = $"{DeviceMac1.Text}-{DeviceMac2.Text}-{DeviceMac3.Text}-{DeviceMac4.Text}-{DeviceMac5.Text}-{DeviceMac6.Text}",
                        IpAddress = $"{DeviceIp1.Text}.{DeviceIp2.Text}.{DeviceIp3.Text}.{DeviceIp4.Text}",
                        BroadcastAddress = $"{DeviceBroadcast1.Text}.{DeviceBroadcast2.Text}.{DeviceBroadcast3.Text}.{DeviceBroadcast4.Text}",
                        Port = Convert.ToByte(DevicePort.Text),
                        SendingCount = Convert.ToByte(SendingCount.Text),
                        Description = DeviceDesc.Text,
                    };

                    sqlite.Insert(device);

                    data = sqlite.QueryAll();
                    adapter = new DeviceListAdapter(this, Resource.Layout.device_list_item, data);
                    DeviceList.Adapter = adapter;

                    if (data.Count != 0)
                    {
                        DeviceList.Visibility = ViewStates.Visible;
                        Tips.Visibility = ViewStates.Gone;
                    }

                    Toast.MakeText(this, GetString(Resource.String.add_device_save_success), ToastLength.Long).Show();
                })
                .SetNegativeButton(GetString(Resource.String.add_device_cancel), (s, e) =>
                {
                    (s as AlertDialog).Dismiss();
                })
                .Create();
            AlertDialog show = alertDialog.Show();
        }

        private void Edit(AdapterView.ItemClickEventArgs e)
        {
            DeviceInfo device = data[e.Position];

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            View view = View.Inflate(this, Resource.Layout.Add, null);

            InitViewControls(view);
            InitViewControlsContents(device);

            alertDialog.SetView(view)
                .SetPositiveButton(GetString(Resource.String.add_device_save), (_s, _e) =>
                {
                    if (!TestDataFormat())
                    {
                        Toast.MakeText(this, GetString(Resource.String.add_device_error), ToastLength.Long).Show();

                        return;
                    }

                    device.Name = DeviceName.Text;
                    device.MacAddress = $"{DeviceMac1.Text}-{DeviceMac2.Text}-{DeviceMac3.Text}-{DeviceMac4.Text}-{DeviceMac5.Text}-{DeviceMac6.Text}";
                    device.IpAddress = $"{DeviceIp1.Text}.{DeviceIp2.Text}.{DeviceIp3.Text}.{DeviceIp4.Text}";
                    device.BroadcastAddress = $"{DeviceBroadcast1.Text}.{DeviceBroadcast2.Text}.{DeviceBroadcast3.Text}.{DeviceBroadcast4.Text}";
                    device.Port = Convert.ToByte(DevicePort.Text);
                    device.SendingCount = Convert.ToByte(SendingCount.Text);
                    device.Description = DeviceDesc.Text;

                    sqlite.Update(device);

                    data = sqlite.QueryAll();
                    adapter = new DeviceListAdapter(this, Resource.Layout.device_list_item, data);
                    DeviceList.Adapter = adapter;

                    Toast.MakeText(this, GetString(Resource.String.add_device_save_success), ToastLength.Long).Show();
                })
                .SetNegativeButton(GetString(Resource.String.add_device_cancel), (_s, _e) =>
                {
                    (_s as AlertDialog).Dismiss();
                })
                .SetNeutralButton(Resource.String.add_device_delete, (_s, _e) =>
                {
                    sqlite.Delete(device);

                    data = sqlite.QueryAll();
                    adapter = new DeviceListAdapter(this, Resource.Layout.device_list_item, data);
                    DeviceList.Adapter = adapter;

                    if (data.Count != 0)
                    {
                        DeviceList.Visibility = ViewStates.Visible;
                        Tips.Visibility = ViewStates.Gone;
                    }

                    Toast.MakeText(this, GetString(Resource.String.add_device_delete_success), ToastLength.Long).Show();
                })
                .Create();
            AlertDialog show = alertDialog.Show();
        }
    }
}

