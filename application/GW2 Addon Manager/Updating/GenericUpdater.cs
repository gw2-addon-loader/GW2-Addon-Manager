using Microsoft.VisualBasic.FileIO;
using System;
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
        UserConfig userConfig;

        string fileName;
        string addon_expanded_path;
        string addon_install_path;

        string latestVersion;

        public GenericUpdater(AddonInfo addon, UpdatingViewModel aViewModel)
        {
            addon_name = addon.folder_name;
            addon_info = addon;
            viewModel = aViewModel;
            userConfig = Configuration.getConfigAsYAML();

            addon_expanded_path = Path.Combine(Path.GetTempPath(), addon_name);
            addon_install_path = Path.Combine(Configuration.getConfigAsYAML().game_path, "addons\\");
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
            string downloadURL = addon_info.host_url;

            //TODO: Add comparison for dll names if version_url is null
            if (addon_info.version_url != null)
                latestVersion = client.DownloadString(addon_info.version_url);
            else //for redirect links
            {
                HttpWebRequest getRedir = (HttpWebRequest)WebRequest.Create(addon_info.host_url);
                getRedir.AllowAutoRedirect = false;
                HttpWebResponse redirResponse = (HttpWebResponse)getRedir.GetResponse();
                downloadURL = redirResponse.Headers.Get("Location");
                latestVersion = Path.GetFileName(downloadURL);
            }

            if (userConfig.version.ContainsKey(addon_name) && userConfig.version[addon_name] != null && latestVersion == userConfig.version[addon_name])
                return;


            
            viewModel.label = "Downloading " + addon_info.addon_name + " " + latestVersion;
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


                if (addon_info.install_mode != "arc")
                {
                    FileSystem.CopyDirectory(addon_expanded_path, addon_install_path, true);
                }
                else
                {
                    if (!Directory.Exists(Path.Combine(addon_install_path, "arcdps")))
                    {
                        Directory.CreateDirectory(Path.Combine(addon_install_path, "arcdps"));
                    }
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
                    {
                        Directory.CreateDirectory(Path.Combine(addon_install_path, "arcdps"));
                    }
                    FileSystem.CopyFile(fileName, Path.Combine(Path.Combine(addon_install_path, "arcdps"), Path.GetFileName(fileName)));
                }
                
            }

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
        public static void disable(AddonInfo addon_info)
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

                        File.Move(
                            Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name), 
                            Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name)
                            );
                    }

                    info.disabled[addon_info.folder_name] = true;
                    Configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** ENABLE *****/
        public static void enable(AddonInfo addon_info)
        {
            UserConfig info = Configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {

                    if (addon_info.install_mode != "arc")
                    {
                        Directory.Move(
                        Path.Combine("Disabled Plugins", addon_info.folder_name),
                        Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name)
                        );
                    }
                    else
                    {
                        if (!Directory.Exists(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps")))
                        {
                            Directory.CreateDirectory(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"));
                        }

                        File.Move(
                            Path.Combine(Path.Combine("Disabled Plugins", addon_info.folder_name), addon_info.plugin_name),
                            Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name)
                            );
                    }
                        
                    info.disabled[addon_info.folder_name] = false;
                    Configuration.setConfigAsYAML(info);
                }
            }
        }

        /***** DELETE *****/
        public static void delete(AddonInfo addon_info)
        {
            UserConfig info = Configuration.getConfigAsYAML();
            if (info.installed.ContainsKey(addon_info.folder_name) && info.installed[addon_info.folder_name] != null)
            {
                if (info.disabled.ContainsKey(addon_info.folder_name) && info.disabled[addon_info.folder_name])
                {
                    Directory.Delete(Path.Combine("Disabled Plugins", addon_info.folder_name), true);
                    info.disabled.Remove(addon_info.folder_name);
                    info.installed.Remove(addon_info.folder_name);
                    info.version.Remove(addon_info.folder_name);
                    Configuration.setConfigAsYAML(info);
                }
                else
                {
                    if (addon_info.install_mode != "arc")
                    {
                        Directory.Delete(Path.Combine(Path.Combine(info.game_path, "addons"), addon_info.folder_name), true);
                        if (info.disabled.ContainsKey(addon_info.folder_name))
                            info.disabled.Remove(addon_info.folder_name);
                        info.installed.Remove(addon_info.folder_name);
                        info.version.Remove(addon_info.folder_name);

                        //deleting arcdps will delete other addons as well
                        if (addon_info.folder_name == "arcdps")
                        {
                            foreach (AddonInfo adj_info in ApprovedList.GenerateAddonList())
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
                        File.Delete(Path.Combine(Path.Combine(Path.Combine(info.game_path, "addons"), "arcdps"), addon_info.plugin_name));
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
            viewModel.showProgress = e.ProgressPercentage;
        }

        void addon_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }
    }
}
