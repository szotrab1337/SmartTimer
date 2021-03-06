﻿using SmartTimer.Models;
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
    public partial class AddEditTemplatePage : ContentPage
    {
        AddEditTemplateViewModel viewModel;
        public AddEditTemplatePage(Template Template)
        {
            InitializeComponent();

            this.BindingContext = viewModel = new AddEditTemplateViewModel(Navigation, Template);
        }
    }
}