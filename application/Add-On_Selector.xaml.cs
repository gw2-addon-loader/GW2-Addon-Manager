using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GW2_Addon_Updater
{
    /// <summary>
    /// Interaction logic for Add_On_Selector.xaml
    /// </summary>
    public partial class Add_On_Selector : Page
    {

        public Add_On_Selector()
        {
            InitializeComponent();
        }



        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void update_button_clicked(object sender, RoutedEventArgs e)
        {
            if (ArcDPS.IsChecked ?? false)
            {
                Application.Current.Properties["ArcDPS"] = true;
            }
            else
            {
                Application.Current.Properties["ArcDPS"] = false;
            }

            if (GW2Radial.IsChecked ?? false)
            {
                Application.Current.Properties["GW2Radial"] = true;
            }
            else
            {
                Application.Current.Properties["GW2Radial"] = false;
            }

            if (d912pxy.IsChecked ?? false)
            {
                Application.Current.Properties["d912pxy"] = true;
            }
            else
            {
                Application.Current.Properties["d912pxy"] = false;
            }

            this.NavigationService.Navigate(new Uri("Updating.xaml", UriKind.Relative));
        }
    }
}
