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
        string arc_md5_path;

        string arc_name;
        string arc_templates_name;
        string d912pxy_name;
        string gw2radial_name;
        bool arc_download_complete = false;
        bool arc_buildTemplates_download_complete = false;

        /* arc URLs */
        string arc_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll";
        string buildtemplates_url = "https://www.deltaconnected.com/arcdps/x64/buildtemplates/d3d9_arcdps_buildtemplates.dll";
        string md5_hash_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll.md5sum";

        /* gw2 radial github release url */
        string git_radial_url = "https://api.github.com/repos/Friendly0Fire/Gw2Radial/releases/latest";

        /* gw2radial variables */
        string version_path;
        string zip_path;
        string expanded_path;
        string radial_releaseNo;
        bool radial_download_complete = false;

        /* d912pxy github release url */
        string git_d912pxy_url = "https://api.github.com/repos/megai2/d912pxy/releases/latest";

        /* d912pxy variables */
        string d912pxy_release_dll = "\\d912pxy\\dll\\release\\d3d9.dll";
        Regex d912pxy_versionRegex = new Regex("v\\d+\\.\\d+\\.*\\d*");
        string d912pxy_releaseNo;
        bool d912pxy_download_complete = false;
        string d912pxy_zip_path;
        string d912pxy_expanded_path;


        public Updating()
        {
            InitializeComponent();
            getPreferences();
            Update();
        }

        public void getPreferences()
        {
            string config_file = File.ReadAllText(config_file_path);
            dynamic config_obj = JsonConvert.DeserializeObject(config_file);
            game_path = config_obj.game_path;
            arc_md5_path = game_path + "\\addons\\arcdps\\d3d9.dll.md5sum";
            arc_name = config_obj.arcDPS;
            arc_templates_name = config_obj.arcDPS_buildTemplates;
            d912pxy_name = config_obj.d912pxy;
            gw2radial_name = config_obj.gw2Radial;

            //determining dll names if arc is installed
            if ((bool)Application.Current.Properties["ArcDPS"])
            {
                arc_name = "d3d9.dll";

                if ((bool)Application.Current.Properties["GW2Radial"])
                {
                    gw2radial_name = "d3d9_chainload.dll";
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
            }
            else if ((bool)Application.Current.Properties["d912pxy"])
            {
                d912pxy_name = "d3d9.dll";
            }

        }

        public void Update()
        {
            if ((bool)Application.Current.Properties["ArcDPS"])
            {
                if (arc_download_complete)
                {
                    currentTask.Content = "Updating ArcDPS Build Templates";
                    update_arc_buildTemplates();
                }
                else
                {
                    currentTask.Content = "Updating ArcDPS";
                    update_arc();
                }


            }
            else if ((bool)Application.Current.Properties["GW2Radial"])
            {
                currentTask.Content = "Updating GW2Radial";
                /* setting gw2Radial variables */
                version_path = game_path + "\\addons\\gw2radial\\version.txt";
                zip_path = Path.Combine(Path.GetTempPath(), "gw2radial.zip");
                expanded_path = Path.Combine(Path.GetTempPath(), "gw2radial");

                download_radial();
            }
            else if ((bool)Application.Current.Properties["d912pxy"])
            {
                currentTask.Content = "Updating d912pxy";
                d912pxy_zip_path = Path.Combine(Path.GetTempPath(), "d912pxy.zip");
                d912pxy_expanded_path = Path.Combine(game_path, "d912pxy");
                update_d912pxy();
            }
            else
            {
                currentTask.Content = "Complete";
                overall_progressBar.Value = 100;
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



        /***************************** ArcDPS *****************************/
        public void update_arc()
        {
            /* download md5 file from arc website */
            var client = new WebClient();
            string md5 = client.DownloadString(md5_hash_url);
            /* if md5 files do not match or there is no md5 file at path, download new arcDPS version */
            if (!File.Exists(arc_md5_path) || md5 != File.ReadAllText(arc_md5_path))
            {
                client.DownloadProgressChanged += arc_DownloadProgressChanged;
                client.DownloadFileCompleted += arc_DownloadCompleted;
                client.DownloadFileAsync(new System.Uri(arc_url), game_path + "\\bin64\\" + arc_name);
                File.WriteAllText(arc_md5_path, md5);
                
            }
            else
            {
                arc_download_complete = true;
                Application.Current.Properties["ArcDPS"] = false;
                Update();
            }
        }

        void arc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            overall_progressBar.Value = e.ProgressPercentage;
        }

        void arc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            arc_download_complete = true;
            Update();
        }

        /***************************** ArcDPS Build Templates *****************************/
        public void update_arc_buildTemplates()
        {
            var client = new WebClient();
            client.DownloadProgressChanged += arc_buildTemplates_DownloadProgressChanged;
            client.DownloadFileCompleted += arc_buildTemplates_DownloadCompleted;
            client.DownloadFileAsync(new System.Uri(buildtemplates_url), game_path + "\\bin64\\" + arc_templates_name);
        }

        void arc_buildTemplates_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            overall_progressBar.Value = e.ProgressPercentage;
        }

        void arc_buildTemplates_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            arc_buildTemplates_download_complete = true;
            Application.Current.Properties["ArcDPS"] = false;
            Update();
        }


        /***************************** GW2Radial *****************************/
        public void download_radial()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(git_radial_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            radial_releaseNo = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;
            if (!File.Exists(version_path) || File.ReadAllText(version_path) != radial_releaseNo)
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(gw2radial_DownloadCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(gw2radial_DownloadProgressChanged);
                client.DownloadFileAsync(new System.Uri(downloadUrl), zip_path);
            }
            else
            {
                radial_download_complete = true;
                Application.Current.Properties["GW2Radial"] = false;
                Update();
            }
        }

        public void install_radial()
        {
            if (File.Exists(expanded_path));
                Directory.Delete(expanded_path, true);

            ZipFile.ExtractToDirectory(zip_path, expanded_path);

            File.Copy(expanded_path + "\\d3d9.dll", game_path + "\\bin64\\" + gw2radial_name, true);
            File.WriteAllText(version_path, radial_releaseNo);
        }

        void gw2radial_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            overall_progressBar.Value = e.ProgressPercentage;
        }

        void gw2radial_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            radial_download_complete = true;
            install_radial();
            Application.Current.Properties["GW2Radial"] = false;
            Update();
        }



        /***************************** d912pxy *****************************/

        public void update_d912pxy()
        {
            string d912pxy_log_path = game_path + "\\d912pxy\\log.txt";
            //string currentVersion = d912pxy_versionRegex.Match(File.ReadAllText(d912pxy_log_path)).ToString();

            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(git_d912pxy_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            d912pxy_releaseNo = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;

            if (!File.Exists(d912pxy_log_path) || d912pxy_releaseNo != d912pxy_versionRegex.Match(File.ReadAllText(d912pxy_log_path)).ToString())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(d912pxy_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(d912pxy_DownloadCompleted);
                client.DownloadFileAsync(new System.Uri(downloadUrl), d912pxy_zip_path);
            }
            else
            {
                d912pxy_download_complete = true;
                Application.Current.Properties["d912pxy"] = false;
                Update();
            }
        }
        void d912pxy_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            overall_progressBar.Value = e.ProgressPercentage;
        }

        void d912pxy_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            d912pxy_download_complete = true;
            Application.Current.Properties["d912pxy"] = false;
            install_d912pxy();
            Update();
        }

        public void install_d912pxy()
        {
            string d912pxy_log_path = game_path + "\\d912pxy\\log.txt";
            string dll_destination = game_path + "\\bin64\\" + d912pxy_name;
            string dll_release_location = game_path +  "\\d912pxy\\dll\\release\\d3d9.dll";
            string updated_log = Regex.Replace(File.ReadAllText(d912pxy_log_path), d912pxy_versionRegex.ToString(), d912pxy_releaseNo);

            //change to archive extraction/overwrite instead of deletion
            if (Directory.Exists(d912pxy_expanded_path))
            {
                Directory.Delete(d912pxy_expanded_path, true);
            }
                
            ZipFile.ExtractToDirectory(d912pxy_zip_path, game_path);
            //FileSystem.CopyDirectory(d912pxy_expanded_path, game_path);
            File.Copy(dll_release_location, dll_destination, true);
            
            File.WriteAllText(d912pxy_log_path, updated_log);
        }


        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

