using GW2_Addon_Manager;
using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using File = System.IO.File;

namespace GW2_Addon_Updater
{
    /// <summary>
    /// Interaction logic for Add_On_Selector.xaml
    /// </summary>
    public partial class Add_On_Selector : Page
    {
        static string config_file_path = "config.ini";

        string no_d912pxy_and_gw2hook_msg = "d912pxy and Gw2 Hook are currently not compatible. " +
            "Using this configuration will most likely result in the game crashing on launch. Are you sure you want to continue?";
        string no_d912pxy_and_gw2hook_title = "Incompatible Add-Ons Warning";

        /* page initialization */
        public Add_On_Selector()
        {
            InitializeComponent();
            game_path.Text = get_default_gamepath();
            apply_default_config();
        }

        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        /***************************** Reading Default Settings *****************************/
        private void apply_default_config()
        {
            dynamic config_obj = configuration.getConfig();

            if ((bool)config_obj.default_configuration.arcdps)
                ArcDPS.IsChecked = true;
            if ((bool)config_obj.default_configuration.gw2radial)
                GW2Radial.IsChecked = true;
            if ((bool)config_obj.default_configuration.d912pxy)
                D912PXY.IsChecked = true;

            if (config_obj.version.arcdps != null)
                ArcDPS.Content = "ArcDPS (installed)";

            if (config_obj.version.gw2radial != null)
                GW2Radial.Content = "GW2 Radial (" + config_obj.version.gw2radial + " installed)";

            if (config_obj.version.d912pxy != null)
                D912PXY.Content = "d912pxy (" + config_obj.version.d912pxy + " installed)";
        }


        /***************************** NAV BAR *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }

        
        private void close_clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void minimize_clicked(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).WindowState = WindowState.Minimized;
        }

        /*****************************   ***   *****************************/





        /******************* BUTTON HANDLERS *******************/

        /***** set addon config as default *****/
        private void Set_default_addons_Click(object sender, RoutedEventArgs e)
        {
            dynamic config_obj = configuration.getConfig();

            if (ArcDPS.IsChecked ?? false)
                config_obj.default_configuration.arcdps = true;
            else
                config_obj.default_configuration.arcdps = false;

            if (GW2Radial.IsChecked ?? false)
                config_obj.default_configuration.gw2radial = true;
            else
                config_obj.default_configuration.gw2radial = false;

            if (D912PXY.IsChecked ?? false)
                config_obj.default_configuration.d912pxy = true;
            else
                config_obj.default_configuration.d912pxy = false;

            configuration.setConfig(config_obj);
        }

        /***** set default gamepath *****/
        private void Set_gamepath_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["game_path"] = game_path.Text.Replace("\\", "\\\\");
            dynamic config_obj = configuration.getConfig();
            config_obj.game_path = Application.Current.Properties["game_path"].ToString().Replace("\\\\", "\\");
            configuration.setConfig(config_obj);
        }

        /***** delete selected add-ons ******/
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            String deletemsg = "This will delete the selected add-ons and all associated data! Are you sure you wish to continue?";
            if (MessageBox.Show(deletemsg, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                string gamePath = get_default_gamepath();
                if (ArcDPS.IsChecked ?? false)
                    arcdps.delete(gamePath);
                if (GW2Radial.IsChecked ?? false)
                    gw2radial.delete(gamePath);
                if (D912PXY.IsChecked ?? false)
                    d912pxy.delete(gamePath);
            }
        }


        /***** add shortcut to start menu *****/
        //see the accepted answer at https://stackoverflow.com/questions/25024785/how-to-create-start-menu-shortcut
        private void Add_to_startmenu_Click(object sender, RoutedEventArgs e)
        {
            string appPath = Directory.GetCurrentDirectory();
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string appShortcutPath = Path.Combine(startMenuPath, @"GW2-UOAOM");
            string shortcutNamePath = Path.Combine(appShortcutPath, "GW2 Addon Manager" + ".lnk");

            if (!Directory.Exists(appShortcutPath))
                Directory.CreateDirectory(appShortcutPath);

            if (!File.Exists(shortcutNamePath))
            {
                WshShell quickShell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)quickShell.CreateShortcut(shortcutNamePath);
                shortcut.Description = "The Guild Wars 2 Unofficial Add-On Manager";
                shortcut.IconLocation = Path.Combine(appPath, "resources\\logo.ico");
                shortcut.WorkingDirectory = appPath;
                shortcut.TargetPath = Path.Combine(appPath, "GW2 Addon Manager" + ".exe");
                shortcut.Save();
            }
        }


        /***** UPDATE button *****/
        private void update_button_clicked(object sender, RoutedEventArgs e)
        {

            /* 
            if ((D912PXY.IsChecked ?? false) && (gw2hook.IsChecked ?? false))
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

            if (D912PXY.IsChecked ?? false)
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

        /******************* *************** *******************/


        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /***** config.ini helpers *****/
        private string get_default_gamepath()
        {
            dynamic config_obj = configuration.getConfig();
            return config_obj.game_path;
        }
    }
}
