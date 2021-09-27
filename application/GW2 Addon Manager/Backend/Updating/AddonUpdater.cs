using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GW2_Addon_Manager
{
    public interface IAddonUpdaterFactory
    {
        AddonUpdater Create(AddonInfo addon);
    }

    public class AddonUpdaterFactory : IAddonUpdaterFactory
    {
        private readonly IConfigurationProvider _configurationManager;
        private readonly AddonRepository _list;

        public AddonUpdaterFactory(IConfigurationProvider configurationManager, AddonRepository list)
        {
            _configurationManager = configurationManager;
            _list = list;
        }

        public AddonUpdater Create(AddonInfo addon)
        {
            return new AddonUpdater(addon, _configurationManager, _list);
        }
    }

    public class AddonUpdater
    {
        private readonly IConfigurationProvider _configurationManager;

        AddonInfo _addonInfo;
        IAddonRepository _addonRepository;

        string fileName;
        string addonExpandedPath;
        string addonInstallPath;

        string latestVersion;

        public AddonUpdater(AddonInfo addon, IConfigurationProvider configurationManager, IAddonRepository addonRepository)
        {
            _addonInfo = addon;
            _configurationManager = configurationManager;
            _addonRepository = addonRepository;

            addonExpandedPath = Path.Combine(Path.GetTempPath(), _addonInfo.AddonName);
            addonInstallPath = Path.Combine(_configurationManager.UserConfig.GamePath, "addons\\");
        }


        public async Task Update()
        {
            var disabledAddonsNames =
                _configurationManager.UserConfig.AddonsList.Where(a => a.Disabled).Select(a => a.Name);
            if (!disabledAddonsNames.Contains(addon_name))
            {
                if (_addonInfo.HostType == "github")
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

            dynamic release_info = UpdateHelpers.GitReleaseInfo(_addonInfo.HostUrl);
            latestVersion = release_info.tag_name;

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            string download_link = release_info.assets[0].browser_download_url;
            viewModel.ProgBarLabel = "Downloading " + _addonInfo.AddonName + " " + latestVersion;
            await Download(download_link, client);
        }

        private async Task StandaloneCheckUpdate()
        {
            var client = UpdateHelpers.OpenWebClient();
            string downloadURL = _addonInfo.HostUrl;

            if (_addonInfo.VersionUrl != null)
                latestVersion = client.DownloadString(_addonInfo.VersionUrl);
            else
            {
                //for self-updating addons' first installation
                viewModel.ProgBarLabel = "Downloading " + _addonInfo.AddonName;
                await Download(downloadURL, client);
                return;
            }

            var currentAddonVersion = _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon_name);
            if (currentAddonVersion != null && currentAddonVersion.Version == latestVersion)
                return;

            viewModel.ProgBarLabel = "Downloading " + _addonInfo.AddonName + " " + latestVersion;
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
                (_addonInfo.AdditionalFlags != null && _addonInfo.AdditionalFlags.Contains("obscured-filename")) ? GetFilenameFromWebServer(url) : Path.GetFileName(url)
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
            viewModel.ProgBarLabel = "Installing " + _addonInfo.AddonName;

            if (_addonInfo.DownloadType == "archive")
            {
                if (Directory.Exists(addonExpandedPath))
                    Directory.Delete(addonExpandedPath, true);

                ZipFile.ExtractToDirectory(fileName, addonExpandedPath);


                if (_addonInfo.InstallMode != "arc")
                {
                    FileSystem.CopyDirectory(addonExpandedPath, addonInstallPath, true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addonInstallPath, "arcdps")))
                        Directory.CreateDirectory(Path.Combine(addonInstallPath, "arcdps"));

                    File.Copy(Path.Combine(addonExpandedPath, _addonInfo.PluginName), Path.Combine(Path.Combine(addonInstallPath, "arcdps"), _addonInfo.PluginName), true);
                }

                
            }
            else
            {
                if (_addonInfo.InstallMode != "arc")
                {
                    if (!Directory.Exists(Path.Combine(addonInstallPath, _addonInfo.FolderName)))
                        Directory.CreateDirectory(Path.Combine(addonInstallPath, _addonInfo.FolderName));

                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addonInstallPath, _addonInfo.FolderName), Path.GetFileName(fileName)), true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addonInstallPath, "arcdps")))
                        Directory.CreateDirectory(Path.Combine(addonInstallPath, "arcdps"));

                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addonInstallPath, "arcdps"), Path.GetFileName(fileName)), true);
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
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == _addonInfo.AddonName);
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (!Directory.Exists("Disabled Plugins"))
                    Directory.CreateDirectory("Disabled Plugins");

                if (!addonConfiguration.Disabled)
                {
                    if (_addonInfo.InstallMode != "arc")
                    {
                        Directory.Move(
                            Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), _addonInfo.FolderName),
                            Path.Combine("Disabled Plugins", _addonInfo.FolderName)
                            );
                    }
                    else
                    {
                        //probably broken
                        if (!Directory.Exists(Path.Combine("Disabled Plugins", _addonInfo.FolderName)))
                            Directory.CreateDirectory(Path.Combine("Disabled Plugins", _addonInfo.FolderName));

                        if (_addonInfo.AddonName.Contains("BuildPad"))
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
                                Path.Combine(Path.Combine("Disabled Plugins", _addonInfo.FolderName), buildPadFileName)
                                );
                        }
                        else
                        {
                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), _addonInfo.PluginName),
                                Path.Combine(Path.Combine("Disabled Plugins", _addonInfo.FolderName), _addonInfo.PluginName)
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
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == _addonInfo.AddonName);
            if (addonConfiguration != null && addonConfiguration.Installed)
            {
                if (addonConfiguration.Disabled)
                {

                    if (_addonInfo.InstallMode != "arc")
                    {
                        //non-arc
                        Directory.Move(
                        Path.Combine("Disabled Plugins", _addonInfo.FolderName),
                        Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), _addonInfo.FolderName)
                        );
                    }
                    else
                    {
                        //arc
                        if (!Directory.Exists(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps")))
                            Directory.CreateDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"));

                        //buildpad compatibility check
                        if (!_addonInfo.AddonName.Contains("BuildPad"))
                        {
                            //non-buildpad
                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", _addonInfo.FolderName), _addonInfo.PluginName),
                                Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), _addonInfo.PluginName)
                                );
                        }
                        else
                        {
                            //buildpad
                            string buildPadFileName = "";
                            string[] buildPadFiles = Directory.GetFiles(Path.Combine("Disabled Plugins", _addonInfo.FolderName));

                            foreach (string someFileName in buildPadFiles)
                                if (someFileName.Contains("buildpad"))
                                    buildPadFileName = Path.GetFileName(someFileName);

                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", _addonInfo.FolderName), buildPadFileName),
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
                _configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == _addonInfo.AddonName);
            if (addonConfiguration !=  null && addonConfiguration.Installed)
            {
                _configurationManager.UserConfig.AddonsList.Remove(addon_name);

                if (addonConfiguration.Disabled)
                {
                    FileSystem.DeleteDirectory(Path.Combine("Disabled Plugins", _addonInfo.FolderName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                else
                {
                    if (_addonInfo.InstallMode != "arc")
                    {
                        FileSystem.DeleteDirectory(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), _addonInfo.FolderName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                        //deleting arcdps will delete other addons as well
                        if (_addonInfo.FolderName == "arcdps")
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
                        if (!_addonInfo.AddonName.Contains("BuildPad"))
                        {
                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(_configurationManager.UserConfig.GamePath, "addons"), "arcdps"), _addonInfo.PluginName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
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
