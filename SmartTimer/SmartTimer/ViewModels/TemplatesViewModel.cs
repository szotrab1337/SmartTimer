using Acr.UserDialogs;
using SmartTimer.Models;
using SmartTimer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartTimer.ViewModels
{
    public class TemplatesViewModel : INotifyPropertyChanged
    {
        public TemplatesViewModel(INavigation Navigation)
        {
            this.Navigation = Navigation;

            Templates = new ObservableCollection<Template>();

            LoadTemplates();

            AddCommand = new Command(AddAction);
            EditCommand = new Command(EditAction);
            DeleteCommand = new Command(DeleteAction);
            ClickCommand = new Command(ClickAction);

            MessagingCenter.Subscribe<AddEditTemplateViewModel>(this, "RefreshTemplates", (sender) =>
            {
                LoadTemplates();
            });

            MessagingCenter.Subscribe<TimerViewModel, bool>(this, "StateChanged", (sender, State) =>
            {
                TimerIsBusy = State;
            });
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClickCommand { get; set; }

        public INavigation Navigation { get; set; }

        public ObservableCollection<Template> Templates
        {
            get { return _Templates; }
            set { _Templates = value; OnPropertyChanged("Templates"); }
        }
        private ObservableCollection<Template> _Templates;

        public bool TimerIsBusy
        {
            get { return _TimerIsBusy; }
            set { _TimerIsBusy = value; OnPropertyChanged("TimerIsBusy"); }
        }
        private bool _TimerIsBusy;

        public async void AddAction()
        {
            try
            {
                await Navigation.PushAsync(new AddEditTemplatePage(null));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void EditAction(object sender)
        {
            try
            {
                Template Template = (Template)sender;

                await Navigation.PushAsync(new AddEditTemplatePage(Template));
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void LoadTemplates()
        {
            try
            {
                Templates.Clear();
                List<Template> rawTemplates = await App.Database.GetAllTemplates();

                AssignNumbers(rawTemplates);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void DeleteAction(object sender)
        {
            try
            {
                Template Template = (Template)sender;

                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Message = "Czy na pewno chcesz usunąć szablon " + ((char)34) + Template.Name + ((char)34) + "?", //char34 == "
                    OkText = "Tak",
                    CancelText = "Nie",
                    Title = "Potwierdzenie"
                });

                if (!result)
                    return;

                UserDialogs.Instance.ShowLoading("Usuwanie...", MaskType.Black);
                await App.Database.RemoveTemplate(Template);
                Templates.Remove(Template);
                RenumberTemplates();
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public void AssignNumbers(List<Template> TemplatesToRenumber)
        {
            try
            {
                for (int i = 0; i < TemplatesToRenumber.Count; i++)
                {
                    TemplatesToRenumber[i].Number = i + 1;
                    Templates.Add(TemplatesToRenumber[i]);
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }
        
        public void RenumberTemplates()
        {
            try
            {
                for (int i = 0; i < Templates.Count; i++)
                {
                    Templates[i].Number = i + 1;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert("Bład!\r\n\r\n" + ex.ToString(), "Błąd", "OK");
            }
        }

        public async void ClickAction(object sender)
        {
            try
            {
                if (TimerIsBusy)
                {
                    UserDialogs.Instance.Alert("Minutnik jest włączony. Anuluj odliczanie i spróbuj ponownie.", "Błąd", "OK");
                    return;
                }

                Template Template = (Template)sender;

                Template.NumberOfUses++;
                await App.Database.UpdateTemplate(Template);

                MessagingCenter.Send(this, "SwitchToTimerPage");
                MessagingCenter.Send(this, "NewTimerFromTemplate", Template);
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
