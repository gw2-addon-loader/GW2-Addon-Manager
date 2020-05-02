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
        AddonInfoFromYaml addon_info;
        UserConfig userConfig;

        string fileName;
        string addon_expanded_path;
        string addon_install_path;

        string latestVersion;

        public GenericUpdater(AddonInfoFromYaml addon)
        {
            addon_name = addon.folder_name;
            addon_info = addon;
            viewModel = UpdatingViewModel.GetInstance;
            userConfig = Configuration.getConfigAsYAML();

            addon_expanded_path = Path.Combine(Path.GetTempPath(), addon_name);
            addon_install_path = Path.Combine(Configuration.getConfigAsYAML().game_path, "addons\\");
        }


        public async Task Update()
        {
            if ((userConfig.disabled.ContainsKey(addon_name) && !userConfig.disabled[addon_name]) || !userConfig.disabled.ContainsKey(addon_name))
                if (addon_info.host_type == "github")
                    await GitCheckUpdate();
                else
                    await StandaloneCheckUpdate();
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

            if (userConfig.version.ContainsKey(addon_name) && userConfig.version[addon_name] != null && latestVersion == userConfig.version[addon_name])
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
                if (!string.IsNullOrEmpty(resp.Headers["Location"]))
                {
                    result = resp.Headers["Location"];
                    result = Path.GetFileName(result);
                }
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

            if (userConfig.version.ContainsKey(addon_info.folder_name))
                userConfig.version[addon_info.folder_name] = latestVersion;
            else
                userConfig.version.Add(addon_info.folder_name, latestVersion);


            if (userConfig.installed.ContainsKey(addon_info.folder_name))
                userConfig.installed[addon_info.folder_name] = addon_info.folder_name;
            else
                userConfig.installed.Add(addon_info.folder_name, addon_info.folder_name);

            if (!userConfig.disabled.ContainsKey(addon_info.folder_name))
                userConfig.disabled.Add(addon_info.folder_name, false);

            //set config.yaml
            Configuration.setConfigAsYAML(userConfig);
        }


        /***** DISABLE *****/
        //TODO: Note to self May 1 2020: consider making some vanity methods to clean up all the Path.Combine()s in here; the code's a bit of a chore to read.
        public static void Disable(AddonInfoFromYaml addon_info)
        {
            UserConfig info = Configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && !info.disabled[addon_info.folder_name])
                {
                    if (addon_info.install_mode != "arc")
                    {
                        Directory.Move(
                            Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name),
                            Path.Combine("Disabled Plugins", addon_info.folder_name)
                            );
                    }
                    else
                    {
                        //probably broken
                        if (!Directory.Exists(Path.Combine("Disabled Plugins", addon_info.folder_name)))
                            Directory.CreateDirectory(Path.Combine("Disabled Plugins", addon_info.folder_name));

                        if (addon_info.addon_name == "BuildPad (Installed)")
                        {
                            string buildPadFileName = "";
                            string[] arcFiles = Directory.GetFiles(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"));

                            //search for plugin name in arc folder
                            //TODO: Should break out of operation and give message if the plugin is not found.
                            foreach (string arcFileName in arcFiles)
                                if (arcFileName.Contains("buildpad"))
                                    buildPadFileName = arcFileName;

                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), buildPadFileName),
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), buildPadFileName)
                                );
                        }
                        else
                        {
                            File.Move(
                                Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name),
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name)
                                );
                        }
                    }

                    info.disabled[addon_info.folder_name] = true;
                    Configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** ENABLE *****/
        public static void enable(AddonInfoFromYaml addon_info)
        {
            UserConfig info = Configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {

                    if (addon_info.install_mode != "arc")
                    {
                        //non-arc
                        Directory.Move(
                        Path.Combine("Disabled Plugins", addon_info.folder_name),
                        Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name)
                        );
                    }
                    else
                    {
                        //arc
                        if (!Directory.Exists(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps")))
                            Directory.CreateDirectory(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"));

                        //buildpad compatibility check
                        if (!addon_info.addon_name.Contains("BuildPad"))
                        {
                            //non-buildpad
                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name),
                                Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name)
                                );
                        }
                        else
                        {
                            //buildpad
                            string buildPadFileName = "";
                            string[] buildPadFiles = Directory.GetFiles(Path.Combine("Disabled Plugins", addon_info.folder_name));

                            foreach (string someFileName in buildPadFiles)
                                if (someFileName.Contains("buildpad"))
                                    buildPadFileName = someFileName;

                            File.Move(
                                Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), buildPadFileName),
                                Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), buildPadFileName)
                                );
                        }

                            
                    }
                        
                    info.disabled[addon_info.folder_name] = false;
                    Configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** DELETE *****/
        public static void delete(AddonInfoFromYaml addon_info)
        {
            UserConfig info = Configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {
                    FileSystem.DeleteDirectory(Path.Combine("Disabled Plugins", addon_info.folder_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    info.disabled.Remove(addon_info.folder_name);
                    info.installed.Remove(addon_info.folder_name);
                    info.version.Remove(addon_info.folder_name);
                    Configuration.setConfigAsYAML(info);
                }
                else
                {
                    if (addon_info.install_mode != "arc")
                    {
                        FileSystem.DeleteDirectory(Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        if (info.disabled.ContainsKey(addon_info.folder_name))
                            info.disabled.Remove(addon_info.folder_name);
                        info.installed.Remove(addon_info.folder_name);
                        info.version.Remove(addon_info.folder_name);

                        //deleting arcdps will delete other addons as well
                        if (addon_info.folder_name == "arcdps")
                        {
                            foreach (AddonInfoFromYaml adj_info in ApprovedList.GenerateAddonList())
                            {
                                if (adj_info.install_mode == "arc")
                                {
                                    //if arc-dependent plugin is disabled, it won't get deleted since it's not in the /addons/arcdps folder
                                    if (info.disabled.ContainsKey(adj_info.folder_name) && !info.disabled[adj_info.folder_name])
                                    {
                                        info.disabled.Remove(adj_info.folder_name);
                                        info.installed.Remove(adj_info.folder_name);
                                        info.version.Remove(adj_info.folder_name);
                                    }
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        //buildpad check
                        if (!addon_info.addon_name.Contains("BuildPad"))
                        {
                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        else
                        {
                            string buildPadFileName = "";
                            string[] arcFiles = Directory.GetFiles(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"));

                            //search for plugin name in arc folder
                            //TODO: Should break out of operation and give message if the plugin is not found.
                            foreach (string arcFileName in arcFiles)
                                if (arcFileName.Contains("buildpad"))
                                    buildPadFileName = arcFileName;

                            FileSystem.DeleteFile(Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), buildPadFileName), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }


                        if (info.disabled.ContainsKey(addon_info.folder_name))
                            info.disabled.Remove(addon_info.folder_name);
                        info.installed.Remove(addon_info.folder_name);
                        info.version.Remove(addon_info.folder_name);
                    }

                    Configuration.setConfigAsYAML(info);
                }

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
