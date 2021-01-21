using Acr.UserDialogs;
using SmartTimer.Models;
using SmartTimer.ViewModels;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

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
            MessagingCenter.Subscribe<TimerViewModel, Duration>(this, "SetAlarm", (sender, Duration) =>
            {
                this.IsTimerSet = true;
            });
            
            MessagingCenter.Subscribe<TimerViewModel>(this, "CancelAlarm", (sender) =>
            {
                this.IsTimerSet = false;
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            if (IsTimerSet)
                MessagingCenter.Send(this, "RefreshUI");
        }

        private bool IsTimerSet { get; set; }
    }
}
