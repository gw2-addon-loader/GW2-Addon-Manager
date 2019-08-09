using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;
using Path = System.IO.Path;

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
        }

        public void Update()
        {
            if ((bool) Application.Current.Properties["ArcDPS"])
            {
                update_arc();
                
            }

            if ((bool)Application.Current.Properties["GW2Radial"])
            {
                /* setting gw2Radial variables */
                version_path = game_path + "\\addons\\gw2radial\\version.txt";
                zip_path = Path.Combine(Path.GetTempPath(), "gw2radial.zip");
                //TODO: Fix below to be Path.GetTempPath() again - the current value is for testing purposes only
                expanded_path = @"C:\\Users\\Matthew Lee\\AppData\\Local\\Temp\\gw2radial";

                download_radial();
            }

            if ((bool)Application.Current.Properties["d912pxy"])
            {
                d912pxy_zip_path = Path.Combine(Path.GetTempPath(), "d912pxy.zip");
                d912pxy_expanded_path = Path.Combine(Path.GetTempPath(), "d912pxy");
                update_d912pxy();
            }
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
                update_arc_buildTemplates();
                client.DownloadProgressChanged += arc_DownloadProgressChanged;
                client.DownloadDataCompleted += arc_DownloadCompleted;
                client.DownloadFileAsync(new System.Uri(arc_url), game_path + "\\bin64\\" + arc_name);
                File.WriteAllText(arc_md5_path, md5);
            }
            else
            {
                arcDPS_buildTemplates_progressBar.Value = 100;
                arcDPS_progressBar.Value = 100;
                arc_download_complete = true;
            }
        }

        void arc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            arcDPS_progressBar.Value = e.ProgressPercentage;
        }

        void arc_DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            arc_download_complete = true;
        }

        /***************************** ArcDPS Build Templates *****************************/
        public void update_arc_buildTemplates()
        {
            var client = new WebClient();
            client.DownloadProgressChanged += arc_buildTemplates_DownloadProgressChanged;
            client.DownloadDataCompleted += arc_buildTemplates_DownloadCompleted;
            client.DownloadFileAsync(new System.Uri(arc_url), game_path + "\\bin64\\" + arc_templates_name);
        }

        void arc_buildTemplates_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            arcDPS_buildTemplates_progressBar.Value = e.ProgressPercentage;
        }

        void arc_buildTemplates_DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            arc_buildTemplates_download_complete = true;
        }


        /***************************** GW2Radial *****************************/
        public void download_radial()
        {
            gw2Radial_progressBar.Maximum = 100;
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(git_radial_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            radial_releaseNo = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;
            if (!File.Exists(version_path) || File.ReadAllText(version_path) != radial_releaseNo)
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(gw2radial_DownloadProgressChanged);
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(gw2radial_DownloadCompleted);
                client.DownloadFileAsync(new System.Uri(downloadUrl), zip_path);
            }
            else
            {
                gw2Radial_progressBar.Value = 100;
                radial_download_complete = true;
            }
        }

        public void install_radial()
        {
            ZipFile.ExtractToDirectory(zip_path, expanded_path);
            File.Copy(expanded_path + "\\d3d9.dll", game_path + "\\bin64\\" + gw2radial_name, true);
            File.WriteAllText(version_path, radial_releaseNo);
        }

        void gw2radial_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            gw2Radial_progressBar.Value = e.ProgressPercentage;
        }

        void gw2radial_DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            radial_download_complete = true;
            install_radial();
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
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(d912pxy_DownloadCompleted);
                client.DownloadFileAsync(new System.Uri(downloadUrl), d912pxy_zip_path);
            }
            else
            {
                d912pxy_progressBar.Value = 100;
                d912pxy_download_complete = true;
            }
        }
        void d912pxy_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            d912pxy_progressBar.Value = e.ProgressPercentage;
        }

        void d912pxy_DownloadCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            d912pxy_download_complete = true;
            install_d912pxy();
        }

        public void install_d912pxy()
        {
            string d912pxy_log_path = game_path + "\\d912pxy\\log.txt";
            ZipFile.ExtractToDirectory(d912pxy_zip_path, d912pxy_expanded_path);
            FileSystem.CopyDirectory(d912pxy_expanded_path, game_path);
            File.Copy(game_path + "\\d912pxy\\dll\release\\d3d9.dll", game_path + "\\bin64\\" + d912pxy_name, true);
            string updated_log = Regex.Replace(File.ReadAllText(d912pxy_log_path), d912pxy_versionRegex.ToString(), d912pxy_releaseNo);
            File.WriteAllText(d912pxy_log_path, updated_log);
            File.WriteAllText(d912pxy_log_path, updated_log);
        }
    }
}

