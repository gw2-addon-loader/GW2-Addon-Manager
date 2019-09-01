using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    class arcdps_mechanics
    {
        UpdatingViewModel currentView;
        static string dll_name = "d3d9_arcdps_mechanics.dll";
        string game_path;
        string bin64;
        string downloadLocation = "http://martionlabs.com/wp-content/uploads/d3d9_arcdps_mechanics.dll";
        string md5sumLocation = "http://martionlabs.com/wp-content/uploads/d3d9_arcdps_mechanics.dll.md5sum";

        /// <summary>
        /// The constructor sets several values to be used and also updates the UI to indicate that arcdps_mechanics is the current task.
        /// </summary>
        /// <param name="view">An instance of the <typeparamref>UpdatingViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public arcdps_mechanics(UpdatingViewModel view)
        {
            game_path = (string)Application.Current.Properties["game_path"];
            bin64 = game_path + "\\" + configuration.getConfig().bin_folder + "\\";
            currentView = view;

            /* emptying progress bar */
            currentView.showProgress = 0;

            currentView.label = "Checking for updates to the ArcDPS Mechanics plugin";
        }

        /// <summary>
        /// Disables arcdps mechanics by moving its plugin into the 'Disabled Plugins' application subfolder.
        /// </summary>
        public static void disable()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";
            if (config_obj.installed.arcdps_mechanics != null)
                File.Move(bin64 + config_obj.installed.arcdps_mechanics, "Disabled Plugins\\" + dll_name);

            config_obj.disabled.arcdps_mechanics = true;
            configuration.setConfig(config_obj);
        }

        /// <summary>
        /// Enables arcdps mechanics by moving its plugin back into the game's /bin64/ or /bin/ folder.
        /// </summary>
        public static void enable()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            if ((bool)config_obj.disabled.arcdps_mechanics && config_obj.installed.arcdps_mechanics != null)
            {
                File.Move("Disabled Plugins\\" + dll_name, bin64 + dll_name);
            }

            config_obj.disabled.arcdps_mechanics = false;
            configuration.setConfig(config_obj);
        }

        /***************************** DELETING *****************************/
        /// <summary>
        /// Deletes the arcdps_mechanics plugin as well as the /arcdps_mechanics/ game subfolder.
        /// </summary>
        public static void delete()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            /* if a .dll is associated with the add-on, delete it */
            if((bool)config_obj.disabled.arcdps_mechanics)
                File.Delete("Disabled Plugins\\" + dll_name);
            else if (config_obj.installed.arcdps_mechanics != null)
                File.Delete(bin64 + config_obj.installed.arcdps_mechanics);



            config_obj.version.arcdps_mechanics = null;      //no installed version
            config_obj.installed.arcdps_mechanics = null;    //no installed .dll name
            configuration.setConfig(config_obj);    //writing to config.ini
        }


        /***************************** UPDATING *****************************/
        /// <summary>
        /// Asynchronously checks for and installs updates for the ArcDPS Mechanics plugin.
        /// </summary>
        /// <returns>A <c>Task</c> that can be awaited.</returns>
        public async Task update()
        {
            var client = new WebClient();
            string md5 = client.DownloadString(md5sumLocation);

            dynamic config_obj = configuration.getConfig();
            if (config_obj.version.arcdps_mechanics == null || config_obj.version.arcdps_mechanics != md5)
            {
                currentView.label = "Downloading ArcDPS Mechanics plugin";
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(arcdps_mechanics_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(arcdps_mechanics_DownloadCompleted);
                await client.DownloadFileTaskAsync(new System.Uri(downloadLocation), bin64 + dll_name);

                config_obj.installed.arcdps_mechanics = dll_name;
                config_obj.version.arcdps_mechanics = md5;
                configuration.setConfig(config_obj);
            }
            else
            {
                Application.Current.Properties["arcdps_mechanics"] = false;
            }
        }
        void arcdps_mechanics_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentView.showProgress = e.ProgressPercentage;
        }

        void arcdps_mechanics_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["arcdps_mechanics"] = false;
        }
    }
}