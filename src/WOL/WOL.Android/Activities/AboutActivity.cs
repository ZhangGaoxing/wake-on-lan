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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);

            TextView version = FindViewById<TextView>(Resource.Id.version);
            TextView email = FindViewById<TextView>(Resource.Id.email);
            ImageView weibo = FindViewById<ImageView>(Resource.Id.weibo);
            ImageView github = FindViewById<ImageView>(Resource.Id.github);

            PackageInfo info = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.Activities);
            version.Text = info.VersionName;

            email.Click += (_s, _e) =>
            {
                try
                {
                    Intent intent = new Intent(Intent.ActionSend, Android.Net.Uri.Parse("mailto:zhangyuexin121@live.cn"));
                    StartActivity(intent);
                    //StartActivity(Intent.CreateChooser(intent, "E-Mail App"));
                }
                catch (Exception)
                {

                }
            };

            weibo.Click += (_s, _e) =>
            {
                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);
                intent.AddCategory(Intent.CategoryBrowsable);
                intent.SetData(Android.Net.Uri.Parse("http://www.weibo.com/279639933"));
                StartActivity(intent);
            };

            github.Click += (_s, _e) =>
            {
                Intent intent = new Intent();
                intent.SetAction(Intent.ActionView);
                intent.AddCategory(Intent.CategoryBrowsable);
                intent.SetData(Android.Net.Uri.Parse("https://github.com/ZhangGaoxing/wake-on-lan"));
                StartActivity(intent);
            };
        }
    }
}