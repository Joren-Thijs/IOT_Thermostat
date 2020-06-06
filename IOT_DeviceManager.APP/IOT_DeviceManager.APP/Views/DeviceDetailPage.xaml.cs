﻿using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using IOT_DeviceManager.APP.Models;
using IOT_DeviceManager.APP.ViewModels;

namespace IOT_DeviceManager.APP.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DeviceDetailPage : ContentPage
    {
        DeviceDetailViewModel viewModel;

        public DeviceDetailPage(DeviceDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public DeviceDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                Text = "Item 1",
                Description = "This is an item description."
            };

            viewModel = new DeviceDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}