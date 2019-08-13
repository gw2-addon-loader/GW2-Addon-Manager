using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Path = System.IO.Path;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GW2_Addon_Updater
{
    /// <summary>
    /// Interaction logic for Updating.xaml
    /// </summary>
    public partial class Updating : Page
    {
        

        static string working_directory = Directory.GetCurrentDirectory();
        static string config_file_path = working_directory + "\\config.ini";

        string game_path;

        string arc_name;
        string arc_templates_name;
        string d912pxy_name;
        string gw2radial_name;

        /* arc URLs */
        string arc_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll";
        string buildtemplates_url = "https://www.deltaconnected.com/arcdps/x64/buildtemplates/d3d9_arcdps_buildtemplates.dll";
        string md5_hash_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll.md5sum";

       

        /* gw2radial variables */
        string version_path;
        string zip_path;
        string expanded_path;
        string radial_releaseNo;
        bool radial_download_complete = false;

        /* d912pxy github release url */
        

        /* d912pxy variables */
        
        string d912pxy_releaseNo;
        bool d912pxy_download_complete = false;
        string d912pxy_zip_path;
        string d912pxy_expanded_path;

        

        UpdatingView view;

        public Updating()
        {
            InitializeComponent();
            view = new UpdatingView { label = "Updating Add-Ons" };
            DataContext = view;
            getPreferences();
            Update();
        }

        public void getPreferences()
        {
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            game_path = config_obj.game_path;

            Application.Current.Properties["game_path"] = game_path;

            arc_name = config_obj.arcDPS;
            arc_templates_name = config_obj.arcDPS_buildTemplates;
            d912pxy_name = config_obj.d912pxy;
            gw2radial_name = config_obj.gw2Radial;

            //determining dll names
            if ((bool)Application.Current.Properties["ArcDPS"])
            {
                arc_name = "d3d9.dll";

                if ((bool)Application.Current.Properties["GW2Radial"])
                {
                    gw2radial_name = "d3d9_chainload.dll";
                    d912pxy_name = "d912pxy.dll";
                    //gw2hook_name = "ReShade64.dll";
                }
                else if ((bool)Application.Current.Properties["d912pxy"])
                {
                    d912pxy_name = "d3d9_chainload.dll";
                }
            }
            else if ((bool)Application.Current.Properties["GW2Radial"])
            {
                gw2radial_name = "d3d9.dll";
                d912pxy_name = "d912pxy.dll";
                //gw2hook_name = "ReShade64.dll";
            }
            else if ((bool)Application.Current.Properties["d912pxy"])
            {
                d912pxy_name = "d3d9.dll";
            }
            /*else if ((bool)Application.Current.Properties["gw2hook"])
            {
                gw2hook_name = "d3d9.dll";
            }*/

        }

        public async void Update()
        {
            if ((bool)Application.Current.Properties["ArcDPS"])
            {
                arcdps arc = new arcdps(arc_name, arc_templates_name, view);
                await arc.update();
                await arc.update_templates();
                Update();
            }
            else if ((bool)Application.Current.Properties["GW2Radial"])
            {
                gw2radial radial = new gw2radial(gw2radial_name, view);
                await radial.update();
                radial.install();
                Update();
            }
            else if ((bool)Application.Current.Properties["d912pxy"])
            {
                d912pxy d912 = new d912pxy(d912pxy_name, view);
                await d912.update();
                d912.install();
                Update();
            }
            /*else if ((bool)Application.Current.Properties["gw2hook"])
            {
                view.label = "Updating Gw2 Hook";
                Update();
            }*/
            else
            {
                view.label = "Complete";
                view.showProgress = 100;
                //enable "finish" button
                closeProgram.IsEnabled = true;
            }
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

        public void finish_button_clicked(Object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        


        

        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

