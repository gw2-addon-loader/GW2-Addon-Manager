using Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Handles downloading a new version of the application.
    /// </summary>
    public class SelfUpdate
    {
        static readonly string applicationRepoUrl = "https://api.github.com/repos/gw2-addon-loader/GW2-Addon-Manager/releases/latest";
        static readonly string update_folder = "latestRelease";
        static readonly string update_name = "update.zip";

        /// <summary>
        /// 
        /// </summary>
        public event UpdateMessageChangedEventHandler UpdateMessageChanged;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateProgressChangedEventHandler UpdateProgressChanged;

        /// <summary>
        /// Sets the viewmodel and starts the download of the latest release.
        /// </summary>
        public SelfUpdate()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
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
            dynamic latestInfo = UpdateHelpers.GitReleaseInfo(applicationRepoUrl);
            string downloadUrl = latestInfo.assets[0].browser_download_url;

            UpdateMessageChanged?.Invoke(this, $"{StaticText.Downloading} {latestInfo.tag_name}");

            Directory.CreateDirectory(update_folder);
            WebClient client = UpdateHelpers.OpenWebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(selfUpdate_DownloadProgress);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(selfUpdate_DownloadCompleted);
            await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), Path.Combine(update_folder, update_name));
        }

        /* updating download status on UI */
        private void selfUpdate_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateMessageChanged?.Invoke(this, $"{StaticText.DownloadComplete}!");
            Application.Current.Properties["update_self"] = true;
        }
        private void selfUpdate_DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateProgressChanged?.Invoke(this, e.ProgressPercentage);
        }


        /// <summary>
        /// Starts the self-updater if an application update has been downloaded.
        /// </summary>
        public void LaunchUpdater()
        {
            if(Application.Current.Properties["update_self"] != null && (bool)Application.Current.Properties["update_self"])
                Process.Start("UOAOM Updater.exe");
        }
    }
}
