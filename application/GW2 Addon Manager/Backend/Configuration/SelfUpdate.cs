using Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using GW2_Addon_Manager.Dependencies.WebClient;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Handles downloading a new version of the application.
    /// </summary>
    class SelfUpdate
    {
        static readonly string applicationRepoUrl = "https://api.github.com/repos/fmmmlee/GW2-Addon-Manager/releases/latest";
        static readonly string update_folder = "latestRelease";
        static readonly string update_name = "update.zip";

        OpeningViewModel viewModel;

        public static void Update()
        {
            new SelfUpdate();
        }

        /// <summary>
        /// Sets the viewmodel and starts the download of the latest release.
        /// </summary>
        private SelfUpdate()
        {
            viewModel = OpeningViewModel.GetInstance;
            viewModel.UpdateProgressVisibility = Visibility.Visible;
            viewModel.UpdateLinkVisibility = Visibility.Hidden;
            Task.Run(() => downloadLatestRelease());
        }

        /// <summary>
        /// Downloads the latest application release.
        /// </summary>
        /// <returns></returns>
        public async Task downloadLatestRelease()
        {
            //perhaps change this to check if downloaded update is latest or not
            if (Directory.Exists(update_folder))
                Directory.Delete(update_folder, true);

            //check application version
            dynamic latestInfo = new UpdateHelper(new WebClientWrapper()).GitReleaseInfo(applicationRepoUrl);
            if (latestInfo == null)
                return;

            string downloadUrl = latestInfo.assets[0].browser_download_url;
            viewModel.UpdateAvailable = $"{StaticText.Downloading} {latestInfo.tag_name}";

            Directory.CreateDirectory(update_folder);
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(selfUpdate_DownloadProgress);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(selfUpdate_DownloadCompleted);
            await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), Path.Combine(update_folder, update_name));
        }

        /* updating download status on UI */
        private void selfUpdate_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            viewModel.UpdateAvailable = $"{StaticText.DownloadComplete}!";
            Application.Current.Properties["update_self"] = true;
        }
        private void selfUpdate_DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            viewModel.UpdateDownloadProgress = e.ProgressPercentage;
        }


        /// <summary>
        /// Starts the self-updater if an application update has been downloaded.
        /// </summary>
        public static void startUpdater()
        {
            if(Application.Current.Properties["update_self"] != null && (bool)Application.Current.Properties["update_self"])
                Process.Start("UOAOM Updater.exe");
        }
    }
}
