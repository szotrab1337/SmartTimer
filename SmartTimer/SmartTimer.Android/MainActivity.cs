using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using Xamarin.Forms;
using Android.Content;
using SmartTimer.ViewModels;
using SmartTimer.Models;
using Plugin.LocalNotification;

namespace SmartTimer.Droid
{
    [Activity(Label = "SmartTimer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            UserDialogs.Init(this);
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            Window.SetStatusBarColor(Android.Graphics.Color.Black);
            Window.SetNavigationBarColor(Android.Graphics.Color.Black);
            NotificationCenter.CreateNotificationChannel();

            MessagingCenter.Subscribe<TimerViewModel, NotificationRequest>(this, "NewNotification", (sender, Notification) =>
            {
                PushNewNotification(Notification);
            });
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void PushNewNotification(NotificationRequest Notification)
        {
            NotificationCenter.Current.Show(Notification);
        }

        public int ElapsedSeconds;
        public bool TimerHelper;

        //public void TimerStart(Duration duration, bool Resume = false)
        //{
        //    if (!Resume)
        //        ElapsedSeconds = 0;
         
        //    TimerHelper = true;
        //    bool SecondaryNotificationSend = false;
        //    bool MainNotificationSend = false;
        //    bool StrikeSecondaryAlarm = true;

        //    if (Resume && duration.SecondaryDuration <= ElapsedSeconds)
        //        StrikeSecondaryAlarm = false;

        //    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        //    {
        //        ElapsedSeconds += 1;

        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            MessagingCenter.Send(Xamarin.Forms.Application.Current, "ElapsedSeconds", ElapsedSeconds);
        //        });

        //        if (ElapsedSeconds >= duration.SecondaryDuration && !SecondaryNotificationSend && !duration.Single && StrikeSecondaryAlarm)
        //        {
        //            SecondaryNotificationSend = true;
        //            MessagingCenter.Send(Xamarin.Forms.Application.Current, "SecondaryTimerEnded");
        //        }

        //        if (ElapsedSeconds >= duration.MainDuration && !MainNotificationSend)
        //        {
        //            TimerHelper = false;
        //            MainNotificationSend = true;
        //            MessagingCenter.Send(Xamarin.Forms.Application.Current, "MainTimerEnded");
        //        }

        //        return TimerHelper;
        //    });
        //}
    }
}