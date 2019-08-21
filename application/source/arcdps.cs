using GW2_Addon_Manager;
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
            theView.showProgress = 0;
            /* display message over progress bar */
            theView.label = "Checking for ArcDPS updates";
            game_path = (string)Application.Current.Properties["game_path"];
            this.arc_name = arc_name;
            this.arc_templates_name = arc_templates_name;
            arc_md5_path = game_path + "\\addons\\arcdps\\d3d9.dll.md5sum";
        }



        /***************************** DELETING *****************************/
        public static void delete(string game_path)
        {
            /* if the /addons/ subdirectory exists for this add-on, delete it */
            if(Directory.Exists(game_path + "\\addons\\arcdps"))
                Directory.Delete(game_path + "\\addons\\arcdps", true);

            dynamic config_obj = configuration.getConfig();

            /* if a .dll is associated with the add-on, delete it */
            if (config_obj.installed.arcdps != null)
                File.Delete(game_path + "\\bin64\\" + config_obj.installed.arcdps);

            config_obj.version.arcdps = null;       //no installed version
            config_obj.installed.arcdps = null;     //no installed .dll name
            configuration.setConfig(config_obj);    //writing to config.ini
        }


        /***************************** UPDATING *****************************/
        public async Task update()
        {
            /* download md5 file from arc website */
            var client = new WebClient();
            string md5 = client.DownloadString(md5_hash_url);
            /* if arc is not installed or recorded timestamp is different from latest release, download it */
            dynamic config_obj = configuration.getConfig();
            if (config_obj.version.arcdps == null || config_obj.version.arcdps != md5)
            {
                theView.label = "Downloading ArcDPS";
                client.DownloadProgressChanged += arc_DownloadProgressChanged;
                client.DownloadFileCompleted += arc_DownloadCompleted;
                await client.DownloadFileTaskAsync(new System.Uri(arc_url), game_path + "\\bin64\\" + arc_name);
                File.WriteAllText(arc_md5_path, md5);

                config_obj.installed.arcdps = arc_name;
                config_obj.version.arcdps = md5;
                configuration.setConfig(config_obj);
            }
            else
            {
                Application.Current.Properties["ArcDPS"] = false;
                theView.showProgress = 100;
            }

            /* build templates - temporary, just checks for file existence */
            if (!File.Exists(game_path + "\\bin64\\" + arc_templates_name))
            {
                await update_templates();
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
        //currently doesn't version check, just downloads
        public async Task update_templates()
        {
            theView.label = "Updating ArcDPS Build Templates";
            var client = new WebClient();

            client.DownloadProgressChanged += arc_buildTemplates_DownloadProgressChanged;
            client.DownloadFileCompleted += arc_buildTemplates_DownloadCompleted;
            await client.DownloadFileTaskAsync(new System.Uri(buildtemplates_url), game_path + "\\bin64\\" + arc_templates_name);
            Application.Current.Properties["ArcDPS"] = false;
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
