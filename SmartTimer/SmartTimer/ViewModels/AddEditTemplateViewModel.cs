using Acr.UserDialogs;
using SmartTimer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartTimer.ViewModels
{
    public class AddEditTemplateViewModel : INotifyPropertyChanged
    {
        public AddEditTemplateViewModel(INavigation Navigation, Template Template)
        {
            this.Navigation = Navigation;
            this.Template = Template;

            SaveTemplateCommand = new Command(SaveTemplateAction);

            PrepareView(Template);
            ValidateSaveButton();
        }

        public INavigation Navigation { get; set; }
        public Template Template { get; set; }

        public ICommand SaveTemplateCommand { get; set; }

        public TimeSpan MainPickedDuration
        {
            get { return _MainPickedDuration; }
            set { _MainPickedDuration = value; OnPropertyChanged("MainPickedDuration"); ValidateSaveButton(); }
        }
        private TimeSpan _MainPickedDuration;

        public TimeSpan SecondaryPickedDuration
        {
            get { return _SecondaryPickedDuration; }
            set { _SecondaryPickedDuration = value; OnPropertyChanged("SecondaryPickedDuration"); ValidateSaveButton(); }
        }
        private TimeSpan _SecondaryPickedDuration;

        public bool SaveButtonIsEnabled
        {
            get { return _SaveButtonIsEnabled; }
            set { _SaveButtonIsEnabled = value; OnPropertyChanged("SaveButtonIsEnabled"); }
        }
        private bool _SaveButtonIsEnabled;
        
        public string TemplateName
        {
            get { return _TemplateName; }
            set { _TemplateName = value; OnPropertyChanged("TemplateName"); ValidateSaveButton(); }
        }
        private string _TemplateName;

        public async void SaveTemplateAction()
        {
            try
            {
                if(Template != null)
                {
                    Template.Name = TemplateName;
                    Template.FirstStepDurationSec = Convert.ToInt32(MainPickedDuration.TotalSeconds);
                    Template.SecondStepDurationSec = Convert.ToInt32(SecondaryPickedDuration.TotalSeconds);

                    await App.Database.UpdateTemplate(Template);
                }
                else
                {
                    await App.Database.AddNewTemplate(new Template
                    {
                        Name = TemplateName,
                        FirstStepDurationSec = Convert.ToInt32(MainPickedDuration.TotalSeconds),
                        SecondStepDurationSec = Convert.ToInt32(SecondaryPickedDuration.TotalSeconds),
                        NumberOfUses = 0
                    });
                }
                MessagingCenter.Send(this, "RefreshTemplates");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void PrepareView(Template Template)
        {
            try
            {
                if (Template == null)
                    return;

                TemplateName = Template.Name;
                MainPickedDuration = TimeSpan.FromSeconds(Template.FirstStepDurationSec);
                SecondaryPickedDuration = TimeSpan.FromSeconds(Template.SecondStepDurationSec);

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void ValidateSaveButton()
        {
            try
            {
                if (string.IsNullOrEmpty(TemplateName))
                {
                    SaveButtonIsEnabled = false;
                    return;
                }

                if (MainPickedDuration.TotalSeconds == 0)
                {
                    SaveButtonIsEnabled = false;
                    return;
                }

                if (SecondaryPickedDuration.TotalSeconds >= MainPickedDuration.TotalSeconds)
                {
                    SaveButtonIsEnabled = false;
                    return;
                }

                SaveButtonIsEnabled = true;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

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
