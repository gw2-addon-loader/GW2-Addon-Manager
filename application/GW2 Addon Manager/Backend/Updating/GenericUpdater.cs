using System;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GW2_Addon_Manager.App.Configuration;
using GW2_Addon_Manager.App.Configuration.Model;
using GW2_Addon_Manager.Dependencies.WebClient;

namespace GW2_Addon_Manager
{
    class GenericUpdater
    {
        private readonly IConfigurationManager _configurationManager;

        UpdatingViewModel viewModel;

        string addon_name;
        AddonInfoFromYaml addon_info;

        string fileName;
        string addon_expanded_path;
        string addon_install_path;

        string latestVersion;

        public GenericUpdater(AddonInfoFromYaml addon, IConfigurationManager configurationManager)
        {
            addon_name = addon.folder_name;
            addon_info = addon;
            _configurationManager = configurationManager;
            viewModel = UpdatingViewModel.GetInstance;

            addon_expanded_path = Path.Combine(Path.GetTempPath(), addon_name);
            addon_install_path = Path.Combine(configurationManager.UserConfig.GamePath, "addons\\");
        }


        public async Task Update()
        {
            var disabledAddonsNames =
                _configurationManager.UserConfig.AddonsList.Where(a => a.Disabled).Select(a => a.Name);
            if (!disabledAddonsNames.Contains(addon_name))
            {
                if (addon_info.host_type == "github")
                    await GitCheckUpdate();
                else
                    await StandaloneCheckUpdate();
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

            var releaseInfo = new UpdateHelper(new WebClientWrapper()).GitReleaseInfo(addon_info.host_url);
            if (releaseInfo == null)
                return;
            latestVersion = releaseInfo.tag_name;

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            string download_link = releaseInfo.assets[0].browser_download_url;
            viewModel.ProgBarLabel = "Downloading " + addon_info.addon_name + " " + latestVersion;
            await Download(download_link, client);
        }

        private async Task StandaloneCheckUpdate()
        {
            var client = new WebClient();
            string downloadURL = addon_info.host_url;

            if (addon_info.version_url != null)
                latestVersion = client.DownloadString(addon_info.version_url);
            else
            {
                //for self-updating addons' first installation
                viewModel.ProgBarLabel = "Downloading " + addon_info.addon_name;
                await Download(downloadURL, client);
                return;
            }

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            viewModel.ProgBarLabel = "Downloading " + addon_info.addon_name + " " + latestVersion;
            await Download(downloadURL, client);
        }


        /***** DOWNLOAD *****/

        /// <summary>
        /// Downloads an add-on from the url specified in <paramref name="url"/> using the WebClient provided in <paramref name="client"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        private async Task Download(string url, WebClient client)
        {

            //this calls helper method to fetch filename if it is not exposed in URL
            fileName = Path.Combine(
                Path.GetTempPath(), 
                ((addon_info.additional_flags != null && addon_info.additional_flags.Contains("obscured-filename")) ? GetFilenameFromWebServer(url) : Path.GetFileName(url))
                );             

            if (File.Exists(fileName))
                File.Delete(fileName);

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(addon_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(addon_DownloadCompleted);            
            
            await client.DownloadFileTaskAsync(new System.Uri(url), fileName);
            Install();
        }

        /* helper method */
        /* credit: Fidel @ stackexchange
         * modified version if their answer at https://stackoverflow.com/a/54616044/9170673
         */
        public string GetFilenameFromWebServer(string url)
        {
            string result = "";

            var req = System.Net.WebRequest.Create(url);
            req.Method = "GET";
            using (System.Net.WebResponse resp = req.GetResponse())
            {
                result = Path.GetFileName(resp.ResponseUri.AbsoluteUri);
            }
            return result;
        }

        /***** INSTALL *****/

        /// <summary>
        /// Performs archive extraction and file IO operations to install the downloaded addon.
        /// </summary>
        private void Install()
        {
            viewModel.ProgBarLabel = "Installing " + addon_info.addon_name;

            if (addon_info.download_type == "archive")
            {
                if (Directory.Exists(addon_expanded_path))
                    Directory.Delete(addon_expanded_path, true);

                ZipFile.ExtractToDirectory(fileName, addon_expanded_path);


                if (addon_info.install_mode != "arc")
                {
                    FileSystem.CopyDirectory(addon_expanded_path, addon_install_path, true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, "arcdps")))
                        Directory.CreateDirectory(Path.Combine(addon_install_path, "arcdps"));

                    File.Copy(Path.Combine(addon_expanded_path, addon_info.plugin_name), Path.Combine(Path.Combine(addon_install_path, "arcdps"), addon_info.plugin_name), true);
                }

                
            }
            else
            {
                if (addon_info.install_mode != "arc")
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, addon_info.folder_name)))
                        Directory.CreateDirectory(Path.Combine(addon_install_path, addon_info.folder_name));

                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addon_install_path, addon_info.folder_name), Path.GetFileName(fileName)), true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, "arcdps")))
                        Directory.CreateDirectory(Path.Combine(addon_install_path, "arcdps"));

                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addon_install_path, "arcdps"), Path.GetFileName(fileName)), true);
                }

                
            }

            //removing download from temp folder to avoid naming clashes
            FileSystem.DeleteFile(fileName);

            var addonConfig = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (addonConfig != null)
                addonConfig.Version = latestVersion;
            else
            {
                var newAddonConfig = new AddonData {Name = addon_name, Installed = true, Version = latestVersion};
                _configurationManager.UserConfig.AddonsList.Add(newAddonConfig);
            }
            _configurationManager.SaveConfiguration();
        }


        /***** DISABLE *****/
        //TODO: Note to self May 1 2020: consider making some vanity methods to clean up all the Path.Combine()s in here; the code's a bit of a chore to read.
        public void Disable()
        {
            var addonConfiguration =
                GetAddonConfig();
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (!Directory.Exists("Disabled Plugins"))
                    Directory.CreateDirectory("Disabled Plugins");

                if (!addonConfiguration.Disabled)
                {
                    if (addon_info.install_mode != "arc")
                    {
                        Directory.Move(
                            Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addon_info.folder_name),
                            Path.Combine("Disabled Plugins", addon_info.folder_name)
                            );
                    }
                    else
                    {
                        //probably broken
                        if (!Directory.Exists(Path.Combine("Disabled Plugins", addon_info.folder_name)))
                            Directory.CreateDirectory(Path.Combine("Disabled Plugins", addon_info.folder_name));

                        if (addon_info.addon_name.Contains("BuildPad"))
                        {
                            string buildPadFileName = "";
                            string[] arcFiles = Directory.GetFiles(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"));

                            //search for plugin name in arc folder
                            //TODO: Should break out of operation and give message if the plugin is not found.
                            foreach (string arcFileName in arcFiles)
                                if (arcFileName.Contains("buildpad"))
                                    buildPadFileName = Path.GetFileName(arcFileName);

                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), buildPadFileName),
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), buildPadFileName)
                                );
                        }
                        else
                        {
                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addon_info.plugin_name),
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name)
                                );
                        }
                    }

                    addonConfiguration.Disabled = true;
                    _configurationManager.SaveConfiguration();
                }
            }
        }

        private AddonData GetAddonConfig() => _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => string.Compare(a.Name, addon_info.addon_name, StringComparison.InvariantCultureIgnoreCase) == 0);

        /***** ENABLE *****/
        public void Enable()
        {
            var addonConfiguration =
                GetAddonConfig();
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (addonConfiguration.Disabled)
                {

                    if (addon_info.install_mode != "arc")
                    {
                        //non-arc
                        Directory.Move(
                        Path.Combine("Disabled Plugins", addon_info.folder_name),
                        Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addon_info.folder_name)
                        );
                    }
                    else
                    {
                        //arc
                        if (!Directory.Exists(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps")))
                            Directory.CreateDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"));

                        //buildpad compatibility check
                        if (!addon_info.addon_name.Contains("BuildPad"))
                        {
                            //non-buildpad
                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name),
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addon_info.plugin_name)
                                );
                        }
                        else
                        {
                            //buildpad
                            string buildPadFileName = "";
                            string[] buildPadFiles = Directory.GetFiles(Path.Combine("Disabled Plugins", addon_info.folder_name));

                            foreach (string someFileName in buildPadFiles)
                                if (someFileName.Contains("buildpad"))
                                    buildPadFileName = Path.GetFileName(someFileName);

                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), buildPadFileName),
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), buildPadFileName)
                                );
                        }

                            
                    }
                        
                    addonConfiguration.Disabled = false;
                    _configurationManager.SaveConfiguration();
                }
            }
        }

        /***** DELETE *****/
        public void Delete()
        {
            var addonConfiguration =
                GetAddonConfig();
            if (addonConfiguration !=  null && addonConfiguration.Installed)
            {
                _configurationManager.UserConfig.AddonsList.Remove(addon_name);

                if (addonConfiguration.Disabled)
                {
                    FileSystem.DeleteDirectory(Path.Combine("Disabled Plugins", addon_info.folder_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    if (addon_info.install_mode != "arc")
                    {
                        FileSystem.DeleteDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addon_info.folder_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        //deleting arcdps will delete other addons as well
                        if (addon_info.folder_name == "arcdps")
                        {
                            foreach (AddonInfoFromYaml adj_info in new ApprovedList(_configurationManager).GenerateAddonList())
                            {
                                if (adj_info.install_mode == "arc")
                                {
                                    var arcDependantConfig =
                                        _configurationManager.UserConfig.AddonsList.First(a =>
                                            a.Name == adj_info.addon_name);
                                    //if arc-dependent plugin is disabled, it won't get deleted since it's not in the /addons/arcdps folder
                                    if (!arcDependantConfig.Disabled)
                                        _configurationManager.UserConfig.AddonsList.Remove(adj_info.addon_name);
                                }
                            }
                        }
                    }
                    else
                    {
                        //buildpad check
                        if (!addon_info.addon_name.Contains("BuildPad"))
                        {
                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addon_info.plugin_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        else
                        {
                            string buildPadFileName = "";
                            string[] arcFiles = Directory.GetFiles(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"));

                            //search for plugin name in arc folder
                            //TODO: Should break out of operation and give message if the plugin is not found.
                            foreach (string arcFileName in arcFiles)
                                if (arcFileName.Contains("buildpad"))
                                    buildPadFileName = Path.GetFileName(arcFileName);

                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), buildPadFileName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }

                        _configurationManager.UserConfig.AddonsList.Remove(addon_name);
                    }
                }
                _configurationManager.SaveConfiguration();
            }
        }

        /***** DOWNLOAD EVENTS *****/
        void addon_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            viewModel.DownloadProgress = e.ProgressPercentage;
        }

        void addon_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }
    }
}
