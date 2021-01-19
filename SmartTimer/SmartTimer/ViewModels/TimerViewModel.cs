using Acr.UserDialogs;
using Android.Content;
using Android.Media;
using Android.OS;
using Plugin.LocalNotification;
using SmartTimer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SmartTimer.ViewModels
{
    public class TimerViewModel : INotifyPropertyChanged
    {
        public TimerViewModel()
        {
            #region DefaultValues

            FirstStepIsVisible = true;
            SecondStepIsVisible = false;
            ApproveIsEnabled = false;
            MainButtonsIsVisible = true;
            EndButtonIsVisible = false;
            StopButtonIsVisible = true;
            ResumeButtonIsVisible = false;
            MainDuration = TimeSpan.FromMilliseconds(0).ToString(@"hh\:mm\:ss\:fff");
            SecondaryDuration = TimeSpan.FromMilliseconds(0).ToString(@"hh\:mm\:ss\:fff");
            MainProgress = 0;
            SecondaryProgress = 0;

            #endregion

            #region Commands

            ApproveCommand = new Command(ApproveAction);
            CancelCommand = new Command(CancelAction);
            StopCommand = new Command(StopAction);
            EndCommand = new Command(EndAction);
            ResumeCommand = new Command(ResumeAction);

            #endregion

            #region Messenger

            MessagingCenter.Subscribe<App>(this, "SecondaryTimerEnded", (sender) =>
            {
                EndSecondaryTimer();
            });

            MessagingCenter.Subscribe<App>(this, "MainTimerEnded", (sender) =>
            {
                TimerHelper = false;
                ChangeButtonsVisibility();
                EndMainTimer();
            });

            MessagingCenter.Subscribe<TemplatesViewModel, Template>(this, "NewTimerFromTemplate", (sender, Template) =>
            {
                TimerFromTemplate(Template);
            });

            MessagingCenter.Subscribe<App>(this, "RefreshTimer", (sender) =>
            {
                RefreshTimer();
            });

            #endregion
        }

        #region Commands

        public ICommand ApproveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand EndCommand { get; set; }
        public ICommand ResumeCommand { get; set; }

        #endregion

        #region Attributes

        public string MainDuration
        {
            get { return _MainDuration; }
            set { _MainDuration = value; OnPropertyChanged("MainDuration"); }
        }
        private string _MainDuration;
        
        public int ElapsedSeconds
        {
            get { return _ElapsedSeconds; }
            set { _ElapsedSeconds = value; OnPropertyChanged("ElapsedSeconds"); }
        }
        private int _ElapsedSeconds;

        public string SecondaryDuration
        {
            get { return _SecondaryDuration; }
            set { _SecondaryDuration = value; OnPropertyChanged("SecondaryDuration"); }
        }
        private string _SecondaryDuration;

        public TimeSpan MainPickedDuration
        {
            get { return _MainPickedDuration; }
            set { _MainPickedDuration = value; OnPropertyChanged("MainPickedDuration"); ValidateApproveButton(); }
        }
        private TimeSpan _MainPickedDuration;
        
        public DateTime MainTimerEnd
        {
            get { return _MainTimerEnd; }
            set { _MainTimerEnd = value; OnPropertyChanged("MainTimerEnd"); }
        }
        private DateTime _MainTimerEnd;
        
        public DateTime PauseDateTime
        {
            get { return _PauseDateTime; }
            set { _PauseDateTime = value; OnPropertyChanged("PauseDateTime"); }
        }
        private DateTime _PauseDateTime;

        public TimeSpan SecondaryPickedDuration
        {
            get { return _SecondaryPickedDuration; }
            set { _SecondaryPickedDuration = value; OnPropertyChanged("SecondaryPickedDuration"); ValidateApproveButton(); }
        }
        private TimeSpan _SecondaryPickedDuration;

        public bool FirstStepIsVisible
        {
            get { return _FirstStepIsVisible; }
            set { _FirstStepIsVisible = value; OnPropertyChanged("FirstStepIsVisible"); }
        }
        private bool _FirstStepIsVisible;

        public bool SecondStepIsVisible
        {
            get { return _SecondStepIsVisible; }
            set { _SecondStepIsVisible = value; OnPropertyChanged("SecondStepIsVisible"); }
        }
        private bool _SecondStepIsVisible;

        public bool StopButtonIsVisible
        {
            get { return _StopButtonIsVisible; }
            set { _StopButtonIsVisible = value; OnPropertyChanged("StopButtonIsVisible"); }
        }
        private bool _StopButtonIsVisible;

        public bool ResumeButtonIsVisible
        {
            get { return _ResumeButtonIsVisible; }
            set { _ResumeButtonIsVisible = value; OnPropertyChanged("ResumeButtonIsVisible"); }
        }
        private bool _ResumeButtonIsVisible;

        public bool SecondaryDurationIsVisible
        {
            get { return _SecondaryDurationIsVisible; }
            set { _SecondaryDurationIsVisible = value; OnPropertyChanged("SecondaryDurationIsVisible"); }
        }
        private bool _SecondaryDurationIsVisible;

        public double MainProgress
        {
            get { return _MainProgress; }
            set { _MainProgress = value; OnPropertyChanged("MainProgress"); }
        }
        private double _MainProgress;

        public double SecondaryProgress
        {
            get { return _SecondaryProgress; }
            set { _SecondaryProgress = value; OnPropertyChanged("SecondaryProgress"); }
        }
        private double _SecondaryProgress;

        public bool ApproveIsEnabled
        {
            get { return _ApproveIsEnabled; }
            set { _ApproveIsEnabled = value; OnPropertyChanged("ApproveIsEnabled"); }
        }
        private bool _ApproveIsEnabled;

        public bool MainButtonsIsVisible
        {
            get { return _MainButtonsIsVisible; }
            set { _MainButtonsIsVisible = value; OnPropertyChanged("MainButtonsIsVisible"); }
        }
        private bool _MainButtonsIsVisible;

        public bool EndButtonIsVisible
        {
            get { return _EndButtonIsVisible; }
            set { _EndButtonIsVisible = value; OnPropertyChanged("EndButtonIsVisible"); }
        }
        private bool _EndButtonIsVisible;
        
        public bool TimerHelper
        {
            get { return _TimerHelper; }
            set { _TimerHelper = value; OnPropertyChanged("TimerHelper"); }
        }
        private bool _TimerHelper;

        public bool SecondaryNotificationSend
        {
            get { return _SecondaryNotificationSend; }
            set { _SecondaryNotificationSend = value; OnPropertyChanged("SecondaryNotificationSend"); }
        }
        private bool _SecondaryNotificationSend;

        public Ringtone Ringtone { get; set; }
        public Vibrator Vibrator { get; set; }

        #endregion

        #region Methods

        public void ApproveAction()
        {
            try
            {
                ElapsedSeconds = 0;
                SetTimer();
                ChangeVisibility();
                ValidateSecondaryDurationVisibility();
                SetAlarms();

                MainTimerEnd = DateTime.Now.Add(TimeSpan.FromSeconds(MainPickedDuration.TotalSeconds));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void CancelAction()
        {
            try
            {
                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz anulować odliczanie?",
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie"
                });

                if (!result)
                    return;

                StopButtonIsVisible = true;
                ResumeButtonIsVisible = false;
                TimerHelper = false;

                MessagingCenter.Instance.Send(this, "CancelAlarms");

                ChangeVisibility();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void StopAction()
        {
            try
            {
                TimerHelper = false;
                ChangeStopResumeButtonsVisibility();
                MessagingCenter.Instance.Send(this, "CancelAlarms");

                PauseDateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
}

        public void ResumeAction()
        {
            try
            {
                SetTimer();
                ChangeStopResumeButtonsVisibility();
                AlarmsResume();

                MainTimerEnd = MainTimerEnd.AddSeconds((DateTime.Now - PauseDateTime).TotalSeconds);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void TimerFromTemplate(Template Template)
        {
            try
            {
                MainPickedDuration = TimeSpan.FromSeconds(Template.FirstStepDurationSec);
                SecondaryPickedDuration = TimeSpan.FromSeconds(Template.SecondStepDurationSec);

                ApproveAction();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void SetAlarms()
        {
            try
            {
                MainDuration = TimeSpan.FromSeconds(MainPickedDuration.TotalSeconds).ToString(@"hh\:mm\:ss");

                if (SecondaryDurationIsVisible)
                {
                    SecondaryDuration = TimeSpan.FromSeconds(SecondaryPickedDuration.TotalSeconds).ToString(@"hh\:mm\:ss");
                    SecondaryProgress = 0;
                }

                MainProgress = 0;

                Duration Duration = new Duration
                {
                    MainDuration = MainPickedDuration.TotalSeconds,
                    SecondaryDuration = SecondaryPickedDuration.TotalSeconds,
                    Single = SecondaryDurationIsVisible ? false : true
                };

                MessagingCenter.Send(this, "SetAlarms", Duration);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void AlarmsResume()
        {
            try
            {
                Duration Duration = new Duration
                {
                    MainDuration = MainPickedDuration.TotalSeconds - ElapsedSeconds,
                    SecondaryDuration = SecondaryPickedDuration.TotalSeconds - ElapsedSeconds,
                    Single = SecondaryDurationIsVisible ? false : true
                };

                MessagingCenter.Send(this, "SetAlarms", Duration);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void EndMainTimer()
        {
            try
            {
                MainDuration = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                MainProgress = 1;

                NotificationRequest Notification = new NotificationRequest
                {
                    NotificationId = 1,
                    Title = "Koniec",
                    Description = "Odliczanie zakończone, możesz wyłączyć Smart Timer",
                };
                MessagingCenter.Instance.Send(this, "NewNotification", Notification);

                Notifications();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void EndSecondaryTimer()
        {
            try
            {
                SecondaryDuration = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                SecondaryProgress = 1;

                NotificationRequest Notification = new NotificationRequest
                {
                    NotificationId = 2,
                    Title = "Przystanek",
                    Description = "Odliczanie do przystanku zakończone",
                };
                MessagingCenter.Instance.Send(this, "NewNotification", Notification);

                Vibration.Vibrate(5000);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
}

        public void Notifications()
        {
            try
            {
                var currentContext = Android.App.Application.Context;
                var alarmUri = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
                Ringtone = RingtoneManager.GetRingtone(currentContext.ApplicationContext, alarmUri);
                Ringtone.Play();

                long[] pattern = { 0, 1500, 2000 };
                Vibrator = Vibrator.FromContext(currentContext);
                Vibrator.Vibrate(pattern, 0);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void EndAction()
        {
            try
            {
                Vibrator.Cancel();
                Ringtone.Stop();
                Ringtone.Dispose();

                ChangeButtonsVisibility();
                ChangeVisibility();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ChangeButtonsVisibility()
        {
            try
            {
                MainButtonsIsVisible = !MainButtonsIsVisible;
                EndButtonIsVisible = !EndButtonIsVisible;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ChangeStopResumeButtonsVisibility()
        {
            try
            {
                StopButtonIsVisible = !StopButtonIsVisible;
                ResumeButtonIsVisible = !ResumeButtonIsVisible;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void UpdateUI(int ElapsedSeconds)
        {
            try
            {
                MainDuration = TimeSpan.FromSeconds(MainPickedDuration.TotalSeconds - ElapsedSeconds).ToString(@"hh\:mm\:ss");
                MainProgress = ElapsedSeconds / MainPickedDuration.TotalSeconds;

                if (SecondaryDurationIsVisible && ElapsedSeconds < SecondaryPickedDuration.TotalSeconds)
                {
                    SecondaryDuration = TimeSpan.FromSeconds(SecondaryPickedDuration.TotalSeconds - ElapsedSeconds).ToString(@"hh\:mm\:ss");
                    SecondaryProgress = ElapsedSeconds / SecondaryPickedDuration.TotalSeconds;
                }
                else
                {
                    SecondaryDuration = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                    SecondaryProgress = 1;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ValidateSecondaryDurationVisibility()
        {
            try
            {
                SecondaryDurationIsVisible = SecondaryPickedDuration.TotalMilliseconds != 0 ? true : false;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ChangeVisibility()
        {
            try
            {
                FirstStepIsVisible = FirstStepIsVisible == true ? false : true;
                SecondStepIsVisible = SecondStepIsVisible == true ? false : true;

                MessagingCenter.Send(this, "StateChanged", SecondStepIsVisible);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ValidateApproveButton()
        {
            try
            {
                if (MainPickedDuration.TotalMilliseconds == 0)
                {
                    ApproveIsEnabled = false;
                    return;
                }

                if (SecondaryPickedDuration.TotalSeconds >= MainPickedDuration.TotalSeconds)
                {
                    ApproveIsEnabled = false;
                    return;
                }

                ApproveIsEnabled = true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void SetTimer()
        {
            try
            {
                TimerHelper = true;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    ElapsedSeconds += 1;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateUI(ElapsedSeconds);
                    });

                    return TimerHelper;
                });
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void RefreshTimer()
        {
            try
            {
                ElapsedSeconds = Convert.ToInt32((DateTime.Now - (MainTimerEnd - TimeSpan.FromSeconds(MainPickedDuration.TotalSeconds))).TotalSeconds);

                if (ElapsedSeconds > MainPickedDuration.TotalSeconds)
                {
                    MainDuration = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                    MainProgress = 1;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
