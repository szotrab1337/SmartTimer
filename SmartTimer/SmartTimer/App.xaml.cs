using Acr.UserDialogs;
using SmartTimer.Models;
using SmartTimer.ViewModels;
using SmartTimer.Views;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartTimer
{
    public partial class App : Application
    {

        public static Database database;
        public static Database Database
        {
            get
            {
                if (database == null)
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db3"));

                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            MessagingCenter.Subscribe<TimerViewModel, Duration>(this, "SetAlarms", (sender, Duration) =>
            {
                if (!Duration.Single)
                    SetSecondaryAlarm(Duration.SecondaryDuration);    

                SetMainAlarm(Duration.MainDuration);
            });

            MessagingCenter.Subscribe<TimerViewModel>(this, "CancelAlarms", (sender) =>
            {
                if (MainToken != null)
                {
                    MainToken.Cancel();
                    MainToken.Dispose();
                }

                if (SecondaryToken != null)
                {
                    SecondaryToken.Cancel();
                    SecondaryToken.Dispose();
                }
            });
        }

        protected override void OnSleep()
        {
           
        }

        protected override void OnResume()
        {
            try
            {
                if (MainToken != null)
                    MessagingCenter.Send(this, "RefreshTimer");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private void SetMainAlarm(double seconds)
        {
            try
            {
                MainToken = new CancellationTokenSource();

                MainAlarm = Task.Factory.StartNew(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(seconds), MainToken.Token).Wait();
                    MessagingCenter.Send(this, "MainTimerEnded");
                }, MainToken.Token);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        private void SetSecondaryAlarm(double seconds)
        {
            try
            {
                SecondaryToken = new CancellationTokenSource();

                SecondaryAlarm = Task.Factory.StartNew(() =>
                {
                    Task.Delay(TimeSpan.FromSeconds(seconds), SecondaryToken.Token).Wait();
                    MessagingCenter.Send(this, "SecondaryTimerEnded");
                }, SecondaryToken.Token);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        private Task MainAlarm;
        private Task SecondaryAlarm;
        private CancellationTokenSource MainToken;
        private CancellationTokenSource SecondaryToken;
    }
}
