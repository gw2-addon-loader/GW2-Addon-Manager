using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>gw2radial</c> class deals with all operations involving the GW2 Radial add-on and its plugin.
    /// </summary>
    public class gw2radial
    {
        UpdatingViewModel currentView;     //view of "updating" screen
        string game_path;             //game folder
        string zip_path;              //place to download zip
        string expanded_path;         //place to extract zip
        string latestRelease;         //version # of latest release
        string gitUrl = "https://api.github.com/repos/Friendly0Fire/Gw2Radial/releases/latest"; // gw2radial github release url
        string dll_name;

        /// <summary>
        /// The constructor sets several values to be used and also updates the UI to indicate that GW2 Radial is the current task.
        /// </summary>
        /// <param name="gw2radial_name">The name of the GW2 Radial plugin file.</param>
        /// <param name="view">An instance of the <typeparamref>UpdatingViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public gw2radial(String gw2radial_name, UpdatingViewModel view)
        {
            dll_name = gw2radial_name;
            game_path = (string)Application.Current.Properties["game_path"];
            currentView = view;
            currentView.label = "Checking for GW2Radial updates";
            zip_path = Path.Combine(Path.GetTempPath(), "gw2radial.zip");
            expanded_path = Path.Combine(Path.GetTempPath(), "gw2radial");
        }

        /// <summary>
        /// Disables GW2 Radial by moving its plugin into the 'Disabled Plugins' application subfolder.
        /// </summary>
        public static void disable()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            if (config_obj.installed.gw2radial != null)
                File.Move(game_path + "\\" + config_obj.bin_folder + "\\" + config_obj.installed.gw2radial, "Disabled Plugins\\gw2radial.dll");

            config_obj.disabled.gw2radial = true;
            configuration.setConfig(config_obj);
        }

        /// <summary>
        /// Enables GW2 Radial by moving its plugin back into the game's /bin64/ or /bin/ folder.
        /// </summary>
        public static void enable()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            if ((bool)config_obj.disabled.gw2radial && config_obj.installed.gw2radial != null)
            {
                /*if arc is installed + not disabled  */
                if (!(bool)config_obj.disabled.arcdps && config_obj.installed.arcdps != null)
                {
                    /* chainloading gw2radial with arc */
                    config_obj.installed.gw2radial = "d3d9_chainload.dll";

                    /* moving d912pxy to be chainloaded by gw2radial */
                    if (!(bool)config_obj.disabled.d912pxy && config_obj.installed.d912pxy != null)
                    {
                        File.Move(bin64 + config_obj.installed.d912pxy, bin64 + "d912pxy.dll");
                        config_obj.installed.d912pxy = "d912pxy.dll";
                    }
                }
                else //if arc not installed, gw2radial becomes d3d9
                {
                    config_obj.installed.gw2radial = "d3d9.dll";

                    /* moving d912pxy to be chainloaded by gw2radial */
                    if (!(bool)config_obj.disabled.d912pxy && config_obj.installed.d912pxy != null)
                    {
                        File.Move(bin64 + config_obj.installed.d912pxy, bin64 + "d912pxy.dll");
                        config_obj.installed.d912pxy = "d912pxy.dll";
                    }
                }
                /* end chainload resolution section */
               
                File.Move("Disabled Plugins\\gw2radial.dll", bin64 + config_obj.installed.gw2radial);
            }

            config_obj.disabled.gw2radial = false;
            configuration.setConfig(config_obj);
        }


        /***************************** DELETING *****************************/
        /// <summary>
        /// Deletes the GW2 Radial plugin as well as the /addons/gw2radial game subfolder.
        /// </summary>
        public static void delete()
        {
            dynamic config_obj = configuration.getConfig();
            string game_path = config_obj.game_path;
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";

            /* if the /addons/ subdirectory exists for this add-on, delete it */
            if (Directory.Exists(game_path + "\\addons\\gw2radial"))
                Directory.Delete(game_path + "\\addons\\gw2radial", true);

            /* if a .dll is associated with the add-on, delete it */
            if (config_obj.installed.gw2radial != null)
                File.Delete(bin64 + config_obj.installed.gw2radial);

            config_obj.version.gw2radial = null;        //no installed version
            config_obj.installed.gw2radial = null;      //no installed .dll name
            configuration.setConfig(config_obj);        //writing to config.ini
        }


        /***************************** UPDATING *****************************/
        /// <summary>
        /// Asynchronously checks for and installs updates for GW2 Radial.
        /// </summary>
        /// <returns>A <c>Task</c> that can be awaited.</returns>
        public async Task update()
        {
            currentView.showProgress = 0;
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            dynamic release_info = UpdateHelpers.GitReleaseInfo(gitUrl);
            latestRelease = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;

            dynamic config_obj = configuration.getConfig();
            if (config_obj.version.gw2radial == null || config_obj.version.gw2radial != latestRelease)
            {
                currentView.label = "Downloading GW2Radial " + latestRelease;
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

        /***************************** INSTALLING *****************************/
        /// <summary>
        /// Performs file operations such as archive extraction and copying the plugin into the game folder.
        /// </summary>
        public void install()
        {
            currentView.label = "Installing GW2Radial " + latestRelease;

            if (Directory.Exists(expanded_path))
                Directory.Delete(expanded_path, true);

            ZipFile.ExtractToDirectory(zip_path, expanded_path);

            dynamic config_obj = configuration.getConfig();
            string bin64 = game_path + "\\" + config_obj.bin_folder + "\\";
            File.Copy(expanded_path + "\\d3d9.dll", bin64 + dll_name, true);

            config_obj.installed.gw2radial = dll_name;
            config_obj.version.gw2radial = latestRelease;
            configuration.setConfig(config_obj);
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
