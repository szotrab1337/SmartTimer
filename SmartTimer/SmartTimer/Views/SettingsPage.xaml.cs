using SmartTimer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartTimer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel viewModel;
        public SettingsPage()
        {
            InitializeComponent();

            this.BindingContext = viewModel = new SettingsViewModel();
        }
    }
}