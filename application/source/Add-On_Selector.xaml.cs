using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        static string working_directory = Directory.GetCurrentDirectory();
        static string config_file_path = working_directory + "\\config.ini";

        string no_d912pxy_and_gw2hook_msg = "d912pxy and Gw2 Hook are currently not compatible. " +
            "Using this configuration will most likely result in the game crashing on launch. Are you sure you want to continue?";
        string no_d912pxy_and_gw2hook_title = "Incompatible Add-Ons Warning";


        public Add_On_Selector()
        {
            InitializeComponent();
            game_path.Text = get_default_gamepath();
        }

        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            
        }


        /***************************** Titlebar Window Drag *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }

        /***************************** Button Controls *****************************/
        private void close_clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void minimize_clicked(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).WindowState = WindowState.Minimized;
        }

        private void update_button_clicked(object sender, RoutedEventArgs e)
        {

            /* 
            if ((d912pxy.IsChecked ?? false) && (gw2hook.IsChecked ?? false))
            {
                MessageBoxResult decision = MessageBox.Show(no_d912pxy_and_gw2hook_msg, no_d912pxy_and_gw2hook_title, MessageBoxButton.OKCancel, MessageBoxImage.Error);
                switch (decision)
                {
                    case MessageBoxResult.OK:
                        
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }*/




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

            /*if (gw2hook.IsChecked ?? false)
            {
                Application.Current.Properties["gw2hook"] = true;
            }
            else
            {
                Application.Current.Properties["gw2hook"] = false;
            }*/

            this.NavigationService.Navigate(new Uri("Updating.xaml", UriKind.Relative));
        }

        private void Set_gamepath_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["game_path"] = game_path.Text.Replace("\\", "\\\\");
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            config_obj.game_path = Application.Current.Properties["game_path"].ToString().Replace("\\\\","\\");
            string edited_config_file = JsonConvert.SerializeObject(config_obj);
            File.WriteAllText(config_file_path, edited_config_file);
        }

        private string get_default_gamepath()
        {
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            return config_obj.game_path;
        }
    }
}
