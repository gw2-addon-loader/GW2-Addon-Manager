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
        const string loaderGitUrl = "https://api.github.com/repos/gw2-addon-loader/loader-core/releases/latest";

        private readonly IConfigurationProvider _configurationManager;
        string loaderGamePath;

        string fileName;
        string latestLoaderVersion;
        string loaderDestination;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateMessageChangedEventHandler UpdateMessageChanged;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateProgressChangedEventHandler UpdateProgressChanged;

        /// <summary>
        /// Constructor; also sets some UI text to indicate that the addon loader is having an update check
        /// </summary>
        public LoaderSetup(IConfigurationProvider configurationManager)
        {
            _configurationManager = configurationManager;
            loaderGamePath = Path.Combine(_configurationManager.UserConfig.GamePath, _configurationManager.UserConfig.BinFolder);
        }

        /// <summary>
        /// Checks for update to addon loader and downloads if a new release is available
        /// </summary>
        /// <returns></returns>
        public async Task HandleLoaderUpdate()
        {
            dynamic releaseInfo = UpdateHelpers.GitReleaseInfo(loaderGitUrl);

            loaderDestination = Path.Combine(loaderGamePath, "d3d9.dll");

            latestLoaderVersion = releaseInfo.tag_name;

            if (File.Exists(loaderDestination) && _configurationManager.UserConfig.LoaderVersion == latestLoaderVersion)
                return;

            string downloadLink = releaseInfo.assets[0].browser_download_url;
            await Download(downloadLink);
        }
        private async Task Download(string url)
        {
            //_viewModel.ProgBarLabel = "Downloading Addon Loader";
            UpdateMessageChanged?.Invoke(this, "Downloading Addon Loader");
            var client = UpdateHelpers.OpenWebClient();

            fileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(url));

            if (File.Exists(fileName))
                File.Delete(fileName);

            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(loader_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(loader_DownloadCompleted);

            await client.DownloadFileTaskAsync(new System.Uri(url), fileName);
            Install();
        }
        private void Install()
        {
            //_viewModel.ProgBarLabel = "Installing Addon Loader";
            UpdateMessageChanged?.Invoke(this, "Installing Addon Loader");

            if (File.Exists(loaderDestination))
                File.Delete(loaderDestination);
            
            ZipFile.ExtractToDirectory(fileName, loaderGamePath);

            _configurationManager.UserConfig.LoaderVersion = latestLoaderVersion;
            _configurationManager.Save();
        }


        /***** DOWNLOAD EVENTS *****/
        void loader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //_viewModel.DownloadProgress = e.ProgressPercentage;
            UpdateProgressChanged?.Invoke(this, e.ProgressPercentage);
        }

        void loader_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }

    }
}
