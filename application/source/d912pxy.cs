using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Updater
{
    class d912pxy
    {
        UpdatingView currentView;
        string dll_name;
        string game_path;
        string latestRelease;
        string git_url = "https://api.github.com/repos/megai2/d912pxy/releases/latest";
        string d912pxy_zip_path;
        string d912pxy_expanded_path;
        Regex versionRegex = new Regex("v\\d+\\.\\d+\\.*\\d*");


        public d912pxy(string d912pxy_name, UpdatingView view)
        {
            game_path = (string)Application.Current.Properties["game_path"];
            currentView = view;
            dll_name = d912pxy_name;
            d912pxy_zip_path = Path.Combine(Path.GetTempPath(), "d912pxy.zip");
            d912pxy_expanded_path = Path.Combine(Path.GetTempPath(), "d912pxy");
        }

        /***************************** d912pxy *****************************/

        public async Task update()
        {
            currentView.label = "Updating d912pxy";

            string d912pxy_log_path = game_path + "\\d912pxy\\log.txt";
            //string currentVersion = versionRegex.Match(File.ReadAllText(d912pxy_log_path)).ToString();

            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(git_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            latestRelease = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;

            if (!File.Exists(d912pxy_log_path) || latestRelease != versionRegex.Match(File.ReadAllText(d912pxy_log_path)).ToString())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(d912pxy_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(d912pxy_DownloadCompleted);
                await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), d912pxy_zip_path);
            }
            else
            {
                Application.Current.Properties["d912pxy"] = false;
            }
        }
        void d912pxy_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentView.showProgress = e.ProgressPercentage;
        }

        void d912pxy_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["d912pxy"] = false;
        }

        public void install()
        {
            string d912pxy_log_path = game_path + "\\d912pxy\\log.txt";
            string dll_destination = game_path + "\\bin64\\" + dll_name;
            string dll_release_location = game_path + "\\d912pxy\\dll\\release\\d3d9.dll";
            string updated_log = Regex.Replace(File.ReadAllText(d912pxy_log_path), versionRegex.ToString(), latestRelease);

            if (File.Exists(d912pxy_expanded_path)) ;
                Directory.Delete(d912pxy_expanded_path, true);

            ZipFile.ExtractToDirectory(d912pxy_zip_path, d912pxy_expanded_path);
            FileSystem.CopyDirectory(d912pxy_expanded_path, game_path, true);

            File.Copy(dll_release_location, dll_destination, true);

            File.WriteAllText(d912pxy_log_path, updated_log);
        }
    }
}
