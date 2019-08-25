using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// The <c>d912pxy</c> class deals with all operations involving d912pxy add-on and its associated files.
    /// </summary>
    class d912pxy
    {
        UpdatingViewModel currentView;
        string dll_name;
        string game_path;
        string latestRelease;
        string git_url = "https://api.github.com/repos/megai2/d912pxy/releases/latest";
        string d912pxy_zip_path;
        string d912pxy_expanded_path;
        Regex versionRegex = new Regex("v\\d+\\.\\d+\\.*\\d*");

        /// <summary>
        /// The constructor sets several values to be used and also updates the UI to indicate that d912pxy is the current task.
        /// </summary>
        /// <param name="d912pxy_name">The name of the d912pxy plugin file.</param>
        /// <param name="view">An instance of the <typeparamref>UpdatingViewModel</typeparamref> class serving as the DataContext for the application UI.</param>
        public d912pxy(string d912pxy_name, UpdatingViewModel view)
        {
            game_path = (string)Application.Current.Properties["game_path"];
            currentView = view;

            /* emptying progress bar */
            currentView.showProgress = 0;

            dll_name = d912pxy_name;
            currentView.label = "Checking for d912pxy updates";
            d912pxy_zip_path = Path.Combine(Path.GetTempPath(), "d912pxy.zip");
            d912pxy_expanded_path = Path.Combine(Path.GetTempPath(), "d912pxy");
        }

        /// <summary>
        /// Disables d912pxy by moving its plugin into the 'Disabled Plugins' application subfolder.
        /// </summary>
        /// <param name="game_path">The Guild Wars 2 game path.</param>
        public static void disable(string game_path)
        {
            dynamic config_obj = configuration.getConfig();
            if (config_obj.installed.d912pxy != null)
                File.Move(game_path + "\\bin64\\" + config_obj.installed.d912pxy, "Disabled Plugins\\d912pxy.dll");

            config_obj.disabled.d912pxy = true;
            configuration.setConfig(config_obj);
        }

        /// <summary>
        /// Enables d912pxy by moving its plugin back into the game's /bin64/ folder.
        /// </summary>
        /// <param name="game_path">The Guild Wars 2 game path.</param>
        public static void enable(string game_path)
        {
            dynamic config_obj = configuration.getConfig();
            if ((bool)config_obj.disabled.d912pxy && config_obj.installed.d912pxy != null)
                File.Move("Disabled Plugins\\d912pxy.dll", game_path + "\\bin64\\" + config_obj.installed.d912pxy);

            config_obj.disabled.d912pxy = false;
            configuration.setConfig(config_obj);
        }

        /***************************** DELETING *****************************/
        /// <summary>
        /// Deletes the d912pxy plugin as well as the /d912pxy/ game subfolder.
        /// </summary>
        /// <param name="game_path">The Guild Wars 2 game path.</param>
        public static void delete(string game_path)
        {
            /* if the /d912pxy/ game subfolder exists, delete it */
            if (Directory.Exists(game_path + "\\d912pxy"))
                Directory.Delete(game_path + "\\d912pxy", true);

            dynamic config_obj = configuration.getConfig();

            /* if a .dll is associated with the add-on, delete it */
            if (config_obj.installed.d912pxy != null)
                File.Delete(game_path + "\\bin64\\" + config_obj.installed.d912pxy);

            config_obj.version.d912pxy = null;      //no installed version
            config_obj.installed.d912pxy = null;    //no installed .dll name
            configuration.setConfig(config_obj);    //writing to config.ini
        }


        /***************************** UPDATING *****************************/
        /// <summary>
        /// Asynchronously checks for and installs updates for d912pxy.
        /// </summary>
        /// <returns>A <c>Task</c> that can be awaited.</returns>
        public async Task update()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            string release_info_json = client.DownloadString(git_url);
            dynamic release_info = JsonConvert.DeserializeObject(release_info_json);
            latestRelease = release_info.tag_name;
            string downloadUrl = release_info.assets[0].browser_download_url;

            dynamic config_obj = configuration.getConfig();
            if (config_obj.version.d912pxy == null || config_obj.version.d912pxy != latestRelease)
            {
                currentView.label = "Downloading d912pxy " + latestRelease;
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(d912pxy_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(d912pxy_DownloadCompleted);
                await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), d912pxy_zip_path);
                install();
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

        /***************************** INSTALLING *****************************/
        /// <summary>
        /// Performs file operations such as archive extraction, directory copying, etc.
        /// </summary>
        public void install()
        {
            string dll_destination = game_path + "\\bin64\\" + dll_name;
            string dll_release_location = game_path + "\\d912pxy\\dll\\release\\d3d9.dll";

            currentView.label = "Installing d912pxy " + latestRelease;

            if (Directory.Exists(d912pxy_expanded_path))
                Directory.Delete(d912pxy_expanded_path, true);

            ZipFile.ExtractToDirectory(d912pxy_zip_path, d912pxy_expanded_path);
            FileSystem.CopyDirectory(d912pxy_expanded_path, game_path, true);

            File.Copy(dll_release_location, dll_destination, true);

            dynamic config_obj = configuration.getConfig();
            config_obj.installed.d912pxy = dll_name;
            config_obj.version.d912pxy = latestRelease;
            configuration.setConfig(config_obj);
        }
    }
}
