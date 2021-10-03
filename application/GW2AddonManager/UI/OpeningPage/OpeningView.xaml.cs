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
    public partial class OpeningView : Page, IHyperlinkHandler
    {
        public OpeningView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<OpeningViewModel>();
        }

        private void AddonsList_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape) {
                AddonsList.SelectedItems.Clear();
            }
        }
    }
}
