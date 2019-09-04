using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace GW2_Addon_Manager
{
    class GenericUpdater
    {
        UpdatingViewModel viewModel;

        string addon_name;
        AddonInfo addon_info;
        config userConfig;

        string addon_zip_path;
        string addon_expanded_path;
        string addon_install_path;



        public GenericUpdater(string name, UpdatingViewModel aViewModel)
        {
            addon_name = name;
            addon_info = UpdateYamlReader.getBuiltInInfo(name);
            viewModel = aViewModel;
            userConfig = configuration.getConfigAsConfig();
            addon_zip_path = Path.Combine(Path.GetTempPath(), "addon.zip");
            addon_expanded_path = Path.Combine(Path.GetTempPath(), name);
            addon_install_path = Path.Combine(configuration.getConfig().game_path, "addons\\" + name);
        }


        public void Update()
        {
            if (!userConfig.disabled[addon_name])
            {
                if (addon_info.host_type == "github")
                {
                    GitCheckUpdate();
                }
                else
                {
                    StandaloneUpdate();
                }
            }
        }

        /// <summary>
        /// Downloads an add-on from the url specified in <paramref name="url"/> using the WebClient provided in <paramref name="client"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        private async void Download(string url, WebClient client)
        {
            if (File.Exists(addon_zip_path))
                File.Delete(addon_zip_path);

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(addon_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(addon_DownloadCompleted);
            await client.DownloadFileTaskAsync(new System.Uri(url), addon_zip_path);

            Install();
        }

        /// <summary>
        /// Performs archive extraction and file IO operations to install the downloaded addon.
        /// </summary>
        private void Install()
        {
            viewModel.label = "Installing " + addon_info.addon_name;

            if (Directory.Exists(addon_expanded_path))
                Directory.Delete(addon_expanded_path);

            ZipFile.ExtractToDirectory(addon_zip_path, addon_expanded_path);

            FileSystem.CopyDirectory(addon_expanded_path, addon_install_path, true);

            //if(d3d9 or whatever version is in bin folder)
            //copy plugin into bin folder

            //set "installed" and "version" fields of relevant property in config.yaml

        }

        /// <summary>
        /// Checks whether an update is required and performs it for an add-on hosted on Github.
        /// </summary>
        private void GitCheckUpdate()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            dynamic release_info = UpdateHelpers.GitReleaseInfo(addon_info.host_url);
            string latestRelease = release_info.tag_name;

            if (userConfig.version[addon_name] != null && latestRelease == userConfig.version[addon_name])
                return;

            viewModel.label = "Downloading " + addon_info.addon_name + " " + latestRelease;
            Download(release_info.assets[0].browser_download_url, client);
        }


        void addon_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            viewModel.showProgress = e.ProgressPercentage;
        }

        void addon_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }

        private void StandaloneUpdate()
        {
            var client = new WebClient();
            string latestVersion = client.DownloadString(addon_info.version_url);

            if (userConfig.version[addon_name] != null && latestVersion == userConfig.version[addon_name])
                return;

            viewModel.label = "Downloading " + addon_info.addon_name + " " + latestVersion;
            Download(addon_info.host_url, client);
        }

    }
}
