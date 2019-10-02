using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace GW2_Addon_Manager
{
    class GenericUpdater
    {
        UpdatingViewModel viewModel;

        string addon_name;
        AddonInfo addon_info;
        config userConfig;

        string fileName;
        string addon_expanded_path;
        string addon_install_path;

        string latestVersion;

        public GenericUpdater(AddonInfo addon, UpdatingViewModel aViewModel)
        {
            addon_name = addon.folder_name;
            addon_info = addon;
            viewModel = aViewModel;
            userConfig = configuration.getConfigAsYAML();

            addon_expanded_path = Path.Combine(Path.GetTempPath(), addon_name);
            addon_install_path = Path.Combine(configuration.getConfigAsYAML().game_path, "addons\\");
        }


        public async Task Update()
        {
            if ((userConfig.disabled.ContainsKey(addon_name) && !userConfig.disabled[addon_name]) || !userConfig.disabled.ContainsKey(addon_name))
            {
                if (addon_info.host_type == "github")
                {
                    await GitCheckUpdate();
                }
                else
                {
                    await StandaloneCheckUpdate();
                }
            }
        }

        /***** UPDATE CHECK *****/

        /// <summary>
        /// Checks whether an update is required and performs it for an add-on hosted on Github.
        /// </summary>
        private async Task GitCheckUpdate()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            dynamic release_info = UpdateHelpers.GitReleaseInfo(addon_info.host_url);
            latestVersion = release_info.tag_name;

            if (userConfig.version.ContainsKey(addon_name) && userConfig.version[addon_name] != null && latestVersion == userConfig.version[addon_name])
                return;

            string download_link = release_info.assets[0].browser_download_url;
            viewModel.label = "Downloading " + addon_info.addon_name + " " + latestVersion;
            await Download(download_link, client);
        }

        private async Task StandaloneCheckUpdate()
        {
            var client = new WebClient();
            latestVersion = client.DownloadString(addon_info.version_url);

            if (userConfig.version.ContainsKey(addon_name) && userConfig.version[addon_name] != null && latestVersion == userConfig.version[addon_name])
                return;

            viewModel.label = "Downloading " + addon_info.addon_name + " " + latestVersion;
            await Download(addon_info.host_url, client);
        }



        /***** DOWNLOAD *****/

        /// <summary>
        /// Downloads an add-on from the url specified in <paramref name="url"/> using the WebClient provided in <paramref name="client"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        private async Task Download(string url, WebClient client)
        {
            fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));

            if (File.Exists(fileName))
                File.Delete(fileName);

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(addon_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(addon_DownloadCompleted);            
            
            await client.DownloadFileTaskAsync(new System.Uri(url), fileName);
            Install();
        }


        /***** INSTALL *****/

        /// <summary>
        /// Performs archive extraction and file IO operations to install the downloaded addon.
        /// </summary>
        private void Install()
        {
            viewModel.label = "Installing " + addon_info.addon_name;

            if (addon_info.download_type == "archive")
            {
                if (Directory.Exists(addon_expanded_path))
                    Directory.Delete(addon_expanded_path, true);

                ZipFile.ExtractToDirectory(fileName, addon_expanded_path);

                FileSystem.CopyDirectory(addon_expanded_path, addon_install_path, true);
            }
            else
            {
                FileSystem.CopyFile(fileName, Path.Combine(addon_install_path, Path.GetFileName(fileName)), true);
            }

            if (userConfig.version.ContainsKey(addon_info.folder_name))
                userConfig.version[addon_info.folder_name] = latestVersion;
            else
                userConfig.version.Add(addon_info.folder_name, latestVersion);


            if (userConfig.installed.ContainsKey(addon_info.folder_name))
                userConfig.installed[addon_info.folder_name] = addon_info.folder_name;
            else
                userConfig.installed.Add(addon_info.folder_name, addon_info.folder_name);

            //set config.yaml
            configuration.setConfigAsYAML(userConfig);
        }


        /***** DISABLE *****/
        public static void disable(AddonInfo addon_info)
        {
            config info = configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && !info.disabled[addon_info.folder_name])
                {
                    Directory.Move(
                        Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name),
                        Path.Combine("Disabled Plugins")
                        );
                    info.disabled[addon_info.folder_name] = true;
                    configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** ENABLE *****/
        public static void enable(AddonInfo addon_info)
        {
            config info = configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {
                    Directory.Move(
                        Path.Combine("Disabled Plugins", addon_info.folder_name),
                        Path.Combine(Path.Combine(info.game_path, "addons"))
                        );
                    info.disabled[addon_info.folder_name] = false;
                    configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** DELETE *****/
        public static void delete(AddonInfo addon_info)
        {
            config info = configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {
                    Directory.Delete(Path.Combine("Disabled Plugins", addon_info.folder_name), true);
                    info.disabled.Remove(addon_info.folder_name);
                    info.installed.Remove(addon_info.folder_name);
                    info.version.Remove(addon_info.folder_name);
                    configuration.setConfigAsYAML(info);
                }
                else
                {
                    Directory.Delete(Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name), true);
                    if(info.disabled.ContainsKey(addon_info.folder_name))
                        info.disabled.Remove(addon_info.folder_name);
                    info.installed.Remove(addon_info.folder_name);
                    info.version.Remove(addon_info.folder_name);
                    configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** DOWNLOAD EVENTS *****/
        void addon_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            viewModel.showProgress = e.ProgressPercentage;
        }

        void addon_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }
    }
}
