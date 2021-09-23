using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GW2_Addon_Manager.App.Configuration;
using GW2_Addon_Manager.App.Configuration.Model;

namespace GW2_Addon_Manager
{
    class GenericUpdaterFactory
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly AvailableAddonList _list;

        public GenericUpdaterFactory(IConfigurationManager configurationManager, AvailableAddonList list)
        {
            _configurationManager = configurationManager;
            _list = list;
        }

        public GenericUpdater Create(AddonInfo addon)
        {
            return new GenericUpdater(addon, _configurationManager, _list);
        }
    }

    class GenericUpdater
    {
        private readonly IConfigurationManager _configurationManager;

        //UpdatingViewModel viewModel;

        string addon_name;
        AddonInfo addonInfo;
        AvailableAddonList _list;

        string fileName;
        string addon_expanded_path;
        string addon_install_path;

        string latestVersion;

        public GenericUpdater(AddonInfo addon, IConfigurationManager configurationManager, AvailableAddonList list)
        {
            addon_name = addon.FolderName;
            addonInfo = addon;
            _configurationManager = configurationManager;
            _list = list;

            addon_expanded_path = Path.Combine(Path.GetTempPath(), addon_name);
            addon_install_path = Path.Combine(configurationManager.UserConfig.GamePath, "addons\\");
        }


        public async Task Update()
        {
            var disabledAddonsNames =
                _configurationManager.UserConfig.AddonsList.Where(a => a.Disabled).Select(a => a.Name);
            if (!disabledAddonsNames.Contains(addon_name))
            {
                if (addonInfo.HostType == "github")
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
            var client = UpdateHelpers.OpenWebClient();

            dynamic release_info = UpdateHelpers.GitReleaseInfo(addonInfo.HostUrl);
            latestVersion = release_info.tag_name;

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            string download_link = release_info.assets[0].browser_download_url;
            viewModel.ProgBarLabel = "Downloading " + addonInfo.AddonName + " " + latestVersion;
            await Download(download_link, client);
        }

        private async Task StandaloneCheckUpdate()
        {
            var client = UpdateHelpers.OpenWebClient();
            string downloadURL = addonInfo.HostUrl;

            if (addonInfo.VersionUrl != null)
                latestVersion = client.DownloadString(addonInfo.VersionUrl);
            else
            {
                //for self-updating addons' first installation
                viewModel.ProgBarLabel = "Downloading " + addonInfo.AddonName;
                await Download(downloadURL, client);
                return;
            }

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            viewModel.ProgBarLabel = "Downloading " + addonInfo.AddonName + " " + latestVersion;
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
                (addonInfo.AdditionalFlags != null && addonInfo.AdditionalFlags.Contains("obscured-filename")) ? GetFilenameFromWebServer(url) : Path.GetFileName(url)
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
            viewModel.ProgBarLabel = "Installing " + addonInfo.AddonName;

            if (addonInfo.DownloadType == "archive")
            {
                if (Directory.Exists(addon_expanded_path))
                    Directory.Delete(addon_expanded_path, true);

                ZipFile.ExtractToDirectory(fileName, addon_expanded_path);


                if (addonInfo.InstallMode != "arc")
                {
                    FileSystem.CopyDirectory(addon_expanded_path, addon_install_path, true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, "arcdps")))
                        Directory.CreateDirectory(Path.Combine(addon_install_path, "arcdps"));

                    File.Copy(Path.Combine(addon_expanded_path, addonInfo.PluginName), Path.Combine(Path.Combine(addon_install_path, "arcdps"), addonInfo.PluginName), true);
                }

                
            }
            else
            {
                if (addonInfo.InstallMode != "arc")
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, addonInfo.FolderName)))
                        Directory.CreateDirectory(Path.Combine(addon_install_path, addonInfo.FolderName));

                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addon_install_path, addonInfo.FolderName), Path.GetFileName(fileName)), true);
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
            _configurationManager.Save();
        }


        /***** DISABLE *****/
        //TODO: Note to self May 1 2020: consider making some vanity methods to clean up all the Path.Combine()s in here; the code's a bit of a chore to read.
        public void Disable()
        {
            var addonConfiguration =
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addonInfo.AddonName);
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (!Directory.Exists("Disabled Plugins"))
                    Directory.CreateDirectory("Disabled Plugins");

                if (!addonConfiguration.Disabled)
                {
                    if (addonInfo.InstallMode != "arc")
                    {
                        Directory.Move(
                            Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addonInfo.FolderName),
                            Path.Combine("Disabled Plugins", addonInfo.FolderName)
                            );
                    }
                    else
                    {
                        //probably broken
                        if (!Directory.Exists(Path.Combine("Disabled Plugins", addonInfo.FolderName)))
                            Directory.CreateDirectory(Path.Combine("Disabled Plugins", addonInfo.FolderName));

                        if (addonInfo.AddonName.Contains("BuildPad"))
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
                                Path.Combine(Path.Combine("Disabled Plugins", addonInfo.FolderName), buildPadFileName)
                                );
                        }
                        else
                        {
                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addonInfo.PluginName),
                                Path.Combine(Path.Combine("Disabled Plugins", addonInfo.FolderName), addonInfo.PluginName)
                                );
                        }
                    }

                    addonConfiguration.Disabled = true;
                    _configurationManager.Save();
                }
            }
        }

        /***** ENABLE *****/
        public void Enable()
        {
            var addonConfiguration =
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addonInfo.AddonName);
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (addonConfiguration.Disabled)
                {

                    if (addonInfo.InstallMode != "arc")
                    {
                        //non-arc
                        Directory.Move(
                        Path.Combine("Disabled Plugins", addonInfo.FolderName),
                        Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addonInfo.FolderName)
                        );
                    }
                    else
                    {
                        //arc
                        if (!Directory.Exists(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps")))
                            Directory.CreateDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"));

                        //buildpad compatibility check
                        if (!addonInfo.AddonName.Contains("BuildPad"))
                        {
                            //non-buildpad
                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addonInfo.FolderName), addonInfo.PluginName),
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addonInfo.PluginName)
                                );
                        }
                        else
                        {
                            //buildpad
                            string buildPadFileName = "";
                            string[] buildPadFiles = Directory.GetFiles(Path.Combine("Disabled Plugins", addonInfo.FolderName));

                            foreach (string someFileName in buildPadFiles)
                                if (someFileName.Contains("buildpad"))
                                    buildPadFileName = Path.GetFileName(someFileName);

                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addonInfo.FolderName), buildPadFileName),
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), buildPadFileName)
                                );
                        }

                            
                    }
                        
                    addonConfiguration.Disabled = false;
                    _configurationManager.Save();
                }
            }
        }

        /***** DELETE *****/
        public void Delete()
        {
            var addonConfiguration =
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addonInfo.AddonName);
            if (addonConfiguration !=  null && addonConfiguration.Installed)
            {
                _configurationManager.UserConfig.AddonsList.Remove(addon_name);

                if (addonConfiguration.Disabled)
                {
                    FileSystem.DeleteDirectory(Path.Combine("Disabled Plugins", addonInfo.FolderName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    if (addonInfo.InstallMode != "arc")
                    {
                        FileSystem.DeleteDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), addonInfo.FolderName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        //deleting arcdps will delete other addons as well
                        if (addonInfo.FolderName == "arcdps")
                        {
                            foreach (AddonInfo adj_info in _list.GenerateAddonList())
                            {
                                if (adj_info.InstallMode == "arc")
                                {
                                    var arcDependantConfig =
                                        _configurationManager.UserConfig.AddonsList.First(a =>
                                            a.Name == adj_info.AddonName);
                                    //if arc-dependent plugin is disabled, it won't get deleted since it's not in the /addons/arcdps folder
                                    if (!arcDependantConfig.Disabled)
                                        _configurationManager.UserConfig.AddonsList.Remove(adj_info.AddonName);
                                }
                            }
                        }
                    }
                    else
                    {
                        //buildpad check
                        if (!addonInfo.AddonName.Contains("BuildPad"))
                        {
                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), addonInfo.PluginName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
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
                _configurationManager.Save();
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
