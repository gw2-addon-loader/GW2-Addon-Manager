using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GW2AddonManager
{
    public partial class OpeningView
    {
        public OpeningView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<OpeningViewModel>();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void addons_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape) {
                addons.SelectedItems.Clear();
            }
        }
    }
}
