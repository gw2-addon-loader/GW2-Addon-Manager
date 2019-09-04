using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    class arcdps_bhud
    {
        UpdatingViewModel currentView;
        string game_path;
        string bin64;
        string latestRelease;
        string gitUrl = "https://api.github.com/repositories/187708533/releases/latest";
        string arcdps_bhud_zip_path;
        string arcdps_bhud_expanded_path;

        /// <summary>
        /// The constructor sets several values to be used and also updates the UI to indicate that arcdps_bhud is the current task.
        /// </summary>
        /// <param name="view">An instance of the <typeparamref>UpdatingViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public arcdps_bhud(UpdatingViewModel view)
        {
            game_path = (string)Application.Current.Properties["game_path"];
            bin64 = game_path + "\\" + configuration.getConfig().bin_folder + "\\";
            currentView = view;

            /* emptying progress bar */
            currentView.showProgress = 0;

            currentView.label = "Checking for updates to ArcDPS for BlishHUD";
            arcdps_bhud_zip_path = Path.Combine(Path.GetTempPath(), "arcdps_bhud.zip");
            arcdps_bhud_expanded_path = Path.Combine(Path.GetTempPath(), "arcdps_bhud");
        }

        /// <summary>
        /// Disables arcdps bhud by moving its plugin into the 'Disabled Plugins' application subfolder.
        /// </summary>
        public static void disable()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";
            if (config_obj.installed.arcdps_bhud != null && File.Exists(bin64 + config_obj.installed.arcdps_bhud))
                File.Move(bin64 + config_obj.installed.arcdps_bhud, "Disabled Plugins\\arcdps_bhud.dll");

            config_obj.disabled.arcdps_bhud = true;
            configuration.setConfig(config_obj);
        }

        /// <summary>
        /// Enables arcdps bhud by moving its plugin back into the game's /bin64/ or /bin/ folder.
        /// </summary>
        public static void enable()
        {
            string dll_name = UpdateYamlReader.getBuiltInInfo("arcdps_bhud").plugin_name;
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            if ((bool)config_obj.disabled.arcdps_bhud && config_obj.installed.arcdps_bhud != null)
            {
                File.Move("Disabled Plugins\\arcdps_bhud.dll", bin64 + dll_name);
            }

            config_obj.disabled.arcdps_bhud = false;
            configuration.setConfig(config_obj);
        }

        /***************************** DELETING *****************************/
        /// <summary>
        /// Deletes the arcdps_bhud plugin as well as the /arcdps_bhud/ game subfolder.
        /// </summary>
        public static void delete()
        {
            string dll_name = UpdateYamlReader.getBuiltInInfo("arcdps_bhud").plugin_name;
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            /* if a .dll is associated with the add-on, delete it */
            if ((bool)config_obj.disabled.arcdps_bhud)
                File.Delete("Disabled Plugins\\" + dll_name);
            else if (config_obj.installed.arcdps_bhud != null)
                File.Delete(bin64 + config_obj.installed.arcdps_bhud);

            config_obj.version.arcdps_bhud = null;      //no installed version
            config_obj.installed.arcdps_bhud = null;    //no installed .dll name
            configuration.setConfig(config_obj);    //writing to config.ini
        }


        /***************************** UPDATING *****************************/
        /// <summary>
        /// Asynchronously checks for and installs updates for arcdps_bhud.
        /// </summary>
        /// <returns>A <c>Task</c> that can be awaited.</returns>
        public async Task update()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            dynamic release_info = UpdateHelpers.GitReleaseInfo(gitUrl);
            latestRelease = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;

            dynamic config_obj = configuration.getConfig();
            if (config_obj.version.arcdps_bhud == null || config_obj.version.arcdps_bhud != latestRelease)
            {
                currentView.label = "Downloading ArcDPS for BlishHUD " + latestRelease;
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(arcdps_bhud_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(arcdps_bhud_DownloadCompleted);
                await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), arcdps_bhud_zip_path);
                install();
            }
            else
            {
                Application.Current.Properties["arcdps_bhud"] = false;
            }
        }
        void arcdps_bhud_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            currentView.showProgress = e.ProgressPercentage;
        }

        void arcdps_bhud_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Application.Current.Properties["arcdps_bhud"] = false;
        }

        /***************************** INSTALLING *****************************/
        /// <summary>
        /// Performs file operations such as archive extraction, directory copying, etc.
        /// </summary>
        public void install()
        {
            string dll_name = UpdateYamlReader.getBuiltInInfo("arcdps_bhud").plugin_name;
            currentView.label = "Installing Arcdps bhud " + latestRelease;

            if (Directory.Exists(arcdps_bhud_expanded_path))
                Directory.Delete(arcdps_bhud_expanded_path, true);

            ZipFile.ExtractToDirectory(arcdps_bhud_zip_path, arcdps_bhud_expanded_path);

            UpdateYamlReader.CheckForUpdateYaml("arcdps_bhud", arcdps_bhud_expanded_path);

            File.Copy(Path.Combine(arcdps_bhud_expanded_path, dll_name), bin64 + dll_name, true);

            dynamic config_obj = configuration.getConfig();
            config_obj.installed.arcdps_bhud = dll_name;
            config_obj.version.arcdps_bhud = latestRelease;
            configuration.setConfig(config_obj);
        }
    }
}
