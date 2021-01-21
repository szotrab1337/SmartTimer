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
using Android.Icu.Util;

namespace SmartTimer.Droid
{
    [Activity(Label = "SmartTimer", Icon = "@drawable/smarttimerico", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
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

            MessagingCenter.Subscribe<TimerViewModel, Duration>(this, "SetAlarm", (sender, Duration) =>
            {
                this.IsSingle = Duration.Single;

                if (!IsSingle)
                    SetAlarm(2, Duration.SecondaryDuration);

                SetAlarm(1, Duration.MainDuration);
            });
            
            MessagingCenter.Subscribe<TimerViewModel>(this, "CancelAlarm", (sender) =>
            {
                if (!IsSingle)
                    CancelAlarm(2);

                CancelAlarm(1);
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

        public bool IsSingle { get; set; }

        private void SetAlarm(int RequestCode, double Seconds)
        {
            var calendar = Calendar.Instance;
            calendar.Add(CalendarField.Second, Convert.ToInt32(Seconds));

            var am = GetSystemService(Context.AlarmService) as AlarmManager;
            var intent = new Intent(this, typeof(MyAlarmReciver));
            intent.PutExtra("ALARM_TYPE", RequestCode);
            intent.AddFlags(ActivityFlags.IncludeStoppedPackages);
            intent.AddFlags(ActivityFlags.ReceiverForeground);
            var pi = PendingIntent.GetBroadcast(this, RequestCode, intent, PendingIntentFlags.UpdateCurrent);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
                am.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, calendar.TimeInMillis, pi);
            else
                am.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pi);
        }
        
        private void CancelAlarm(int RequestCode)
        {
            try
            {
                var am = GetSystemService(Context.AlarmService) as AlarmManager;
                var intent = new Intent(this, typeof(MyAlarmReciver));
                intent.PutExtra("ALARM_TYPE", RequestCode);
                intent.AddFlags(ActivityFlags.IncludeStoppedPackages);
                intent.AddFlags(ActivityFlags.ReceiverForeground);
                var pi = PendingIntent.GetBroadcast(this, RequestCode, intent, PendingIntentFlags.NoCreate);

                am.Cancel(pi);
            }
            catch (Exception ex)
            { }
        }
    }
}