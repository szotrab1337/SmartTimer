using SmartTimer.ViewModels;
using SmartTimer.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SmartTimer
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            this.CurrentItem.CurrentItem = timerPage;
        }
    }
}
