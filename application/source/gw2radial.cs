using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Updater
{
    class gw2radial
    {
        UpdatingView currentView;     //view of "updating" screen
        string game_path;             //game folder
        string version_path;          //location of version.txt
        string zip_path;              //place to download zip
        string expanded_path;         //place to extract zip
        string latestRelease;         //version # of latest release
        string git_radial_url = "https://api.github.com/repos/Friendly0Fire/Gw2Radial/releases/latest"; // gw2radial github release url
        string dll_name;


        public gw2radial(String gw2radial_name, UpdatingView view)
        {
            dll_name = gw2radial_name;
            game_path = (string)Application.Current.Properties["game_path"];
            currentView = view;
            currentView.label = "Updating GW2Radial";
            version_path = game_path + "\\addons\\gw2radial\\version.txt";
            zip_path = Path.Combine(Path.GetTempPath(), "gw2radial.zip");
            expanded_path = Path.Combine(Path.GetTempPath(), "gw2radial");
        }


        /***************************** GW2Radial *****************************/
        public async Task update()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(git_radial_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            latestRelease = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;
            if (!File.Exists(version_path) || File.ReadAllText(version_path) != latestRelease)
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(gw2radial_DownloadCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(gw2radial_DownloadProgressChanged);
                await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), zip_path);
                install();
            }
            else
            {
                Application.Current.Properties["GW2Radial"] = false;
            }
        }

        public void install()
        {
            if (Directory.Exists(expanded_path))
                Directory.Delete(expanded_path, true);

            ZipFile.ExtractToDirectory(zip_path, expanded_path);

            File.Copy(expanded_path + "\\d3d9.dll", game_path + "\\bin64\\" + dll_name, true);
            File.WriteAllText(version_path, latestRelease);
        }

        void gw2radial_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentView.showProgress = e.ProgressPercentage;
        }

        void gw2radial_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["GW2Radial"] = false;
        }

    }
}
