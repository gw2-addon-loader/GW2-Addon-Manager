using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using GW2_Addon_Manager.App.Configuration;

namespace GW2_Addon_Manager
{
    class LoaderSetup
    {
        const string loader_git_url = "https://api.github.com/repos/gw2-addon-loader/loader-core/releases/latest";

        private readonly IConfigurationManager _configurationManager;
        string loader_game_path;

        UpdatingViewModel viewModel;
        string fileName;
        string latestLoaderVersion;
        string loader_dxgi_destination;
        string loader_d3d11_destination;
        string loader_self_destination;

        /// <summary>
        /// Constructor; also sets some UI text to indicate that the addon loader is having an update check
        /// </summary>
        public LoaderSetup(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
            viewModel = UpdatingViewModel.GetInstance;
            viewModel.ProgBarLabel = "Checking for updates to Addon Loader";
            loader_game_path = configurationManager.UserConfig.GamePath;
        }

        /// <summary>
        /// Checks for update to addon loader and downloads if a new release is available
        /// </summary>
        /// <returns></returns>
        public async Task HandleLoaderUpdate()
        {
            dynamic releaseInfo = UpdateHelpers.GitReleaseInfo(loader_git_url);

            loader_d3d11_destination = Path.Combine(loader_game_path, "d3d11.dll");
            loader_dxgi_destination = Path.Combine(loader_game_path, "bin64/cef/dxgi.dll");
            loader_self_destination = Path.Combine(loader_game_path, "addonLoader.dll");

            latestLoaderVersion = releaseInfo.tag_name;

            if (File.Exists(loader_d3d11_destination) && 
               File.Exists(loader_dxgi_destination) && 
               File.Exists(loader_self_destination) &&
               _configurationManager.UserConfig.LoaderVersion == latestLoaderVersion)
                return;

            string downloadLink = releaseInfo.assets[0].browser_download_url;
            await Download(downloadLink);
        }
        private async Task Download(string url)
        {
            viewModel.ProgBarLabel = "Downloading Addon Loader";

            fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));

            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var client = UpdateHelpers.GetClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(loader_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(loader_DownloadCompleted);

                await client.DownloadFileTaskAsync(new System.Uri(url), fileName);
            }
            Install();
        }
        private void Install()
        {
            viewModel.ProgBarLabel = "Installing Addon Loader";

            if (File.Exists(loader_d3d11_destination))
                File.Delete(loader_d3d11_destination);

            if (File.Exists(loader_dxgi_destination))
                File.Delete(loader_dxgi_destination);

            if (File.Exists(loader_self_destination))
                File.Delete(loader_self_destination);

            ZipFile.ExtractToDirectory(fileName, loader_game_path);

            _configurationManager.UserConfig.LoaderVersion = latestLoaderVersion;
            _configurationManager.SaveConfiguration();
        }


        /***** DOWNLOAD EVENTS *****/
        void loader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            viewModel.DownloadProgress = e.ProgressPercentage;
        }

        void loader_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }

    }
}
