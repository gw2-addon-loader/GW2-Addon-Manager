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
    //GW2Hook only works on my system as d3d9.dll itself or as ReShade64 with GW2Radial as the ONLY other add-on. It did run with d912pxy but I assume the latter didn't work
    //(it also killed the radial menu - I'll have to check to see if it improved performance at all).

    class gw2hook
    {
        UpdatingView currentView;
        string game_path;
        string dll_name;

        
        string gw2hook_name;
        string gw2hook_url = "https://04348.github.io/Gw2Hook/download";
        string gw2hook_version_path;
        string gw2hook_zip_path = Path.Combine(Path.GetTempPath(), "gw2hook.zip");
        string gw2hook_expanded_path;
        string gw2hook_version = "";

        public gw2hook(string gw2hook_name, UpdatingView view)
        {
            game_path = (string)Application.Current.Properties["game_path"];
            dll_name = gw2hook_name;
            currentView = view;
        }

        /***************************** GW2 Hook *****************************/
        public void update()
        {
            var client = new WebClient();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(gw2hook_url);
            request.AllowAutoRedirect = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string downloadlink = response.Headers["Location"];
            string fileName = response.Headers["Content-Disposition"];
            response.Close();

            if (!File.Exists(gw2hook_version_path) || File.ReadAllText(gw2hook_version_path) != fileName)
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(gw2hook_DownloadCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(gw2hook_DownloadProgressChanged);
                client.DownloadFileAsync(new System.Uri(gw2hook_url), gw2hook_zip_path);
            }
            else
            {
                Application.Current.Properties["gw2hook"] = false;
            }
        }

        void gw2hook_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentView.showProgress = e.ProgressPercentage;
        }

        void gw2hook_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["gw2hook"] = false;
        }

        public void install()
        {
            gw2hook_version_path = game_path + "\\addons\\Gw2Hook\\version.txt";
            ZipArchive gw2Archive = ZipFile.OpenRead(gw2hook_zip_path);
            foreach (ZipArchiveEntry entry in gw2Archive.Entries)
            {
                if (entry.Name != "d3d9.dll")
                    File.Copy(Path.Combine(gw2hook_zip_path, entry.Name), game_path, true);
                else
                    File.Copy(Path.Combine(gw2hook_zip_path, entry.Name), game_path + "\\bin64\\" + gw2hook_name, true);
            }

            File.WriteAllText(gw2hook_version_path, gw2hook_version);
        }

    }
}
