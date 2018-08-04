using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WOL.Droid.Activities
{
    [Activity(Label = "AboutActivity")]
    public class AboutActivity : Activity
    {
        TextView version;
        ImageView weibo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);

            version = FindViewById<TextView>(Resource.Id.version);
            weibo = FindViewById<ImageView>(Resource.Id.weibo);

            PackageInfo info = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.Activities);
            version.Text = info.VersionName;

            weibo.Click += (_s, _e) =>
            {
                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);
                intent.AddCategory(Intent.CategoryBrowsable);
                intent.SetData(Android.Net.Uri.Parse("http://www.weibo.com/279639933"));
                StartActivity(intent);
            };
        }
    }
}