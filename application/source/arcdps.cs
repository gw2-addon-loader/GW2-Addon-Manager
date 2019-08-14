using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GW2_Addon_Updater
{
    public class arcdps
    {
        string game_path;

        string arc_md5_path;
        string arc_templates_name;
        string arc_name;

        /* arc URLs */
        string arc_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll";
        string buildtemplates_url = "https://www.deltaconnected.com/arcdps/x64/buildtemplates/d3d9_arcdps_buildtemplates.dll";
        string md5_hash_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll.md5sum";

        UpdatingView theView;

        public arcdps(string arc_name, string arc_templates_name, UpdatingView view)
        {
            theView = view;
            game_path = (string)Application.Current.Properties["game_path"];
            this.arc_name = arc_name;
            this.arc_templates_name = arc_templates_name;
            arc_md5_path = game_path + "\\addons\\arcdps\\d3d9.dll.md5sum";
        }

       

        /***************************** ArcDPS *****************************/
        public async Task update()
        {
            /* display message over progress bar */
            theView.label = "Updating ArcDPS";  

            /* download md5 file from arc website */
            var client = new WebClient();
            string md5 = client.DownloadString(md5_hash_url);
            /* if md5 files do not match or there is no md5 file at path, download new arcDPS version */
            if (!File.Exists(arc_md5_path) || md5 != File.ReadAllText(arc_md5_path))
            {
                client.DownloadProgressChanged += arc_DownloadProgressChanged;
                client.DownloadFileCompleted += arc_DownloadCompleted;
                await client.DownloadFileTaskAsync(new System.Uri(arc_url), game_path + "\\bin64\\" + arc_name);
                File.WriteAllText(arc_md5_path, md5);
            }
            else
            {
                Application.Current.Properties["ArcDPS"] = false;
                theView.showProgress = 100;
            }
        }

        void arc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            theView.showProgress = e.ProgressPercentage;
        }

        void arc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["ArcDPS"] = false;
        }

        /***************************** ArcDPS Build Templates *****************************/

        ///////////////NEEDS DIFFERENT VERSION CHECKING

        public async Task update_templates()
        {
            theView.label = "Updating ArcDPS Build Templates";
            var client = new WebClient();
            string md5 = client.DownloadString(md5_hash_url);
            /* if md5 files do not match or there is no md5 file at path, download new arcDPS version */
            if (!File.Exists(arc_md5_path) || md5 != File.ReadAllText(arc_md5_path))
            {
                client.DownloadProgressChanged += arc_buildTemplates_DownloadProgressChanged;
                client.DownloadFileCompleted += arc_buildTemplates_DownloadCompleted;
                await client.DownloadFileTaskAsync(new System.Uri(buildtemplates_url), game_path + "\\bin64\\" + arc_templates_name);
                File.WriteAllText(arc_md5_path, md5);
            }
            else
            {
                Application.Current.Properties["ArcDPS"] = false;
            }
        }

        void arc_buildTemplates_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            theView.showProgress = e.ProgressPercentage;
        }

        void arc_buildTemplates_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["ArcDPS"] = false;
            theView.showProgress = 100;
        }

    }
}
