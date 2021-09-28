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
        static readonly string updateFolder = "latestRelease";
        static readonly string updateName = "update.zip";

        /// <summary>
        /// 
        /// </summary>
        public event UpdateMessageChangedEventHandler UpdateMessageChanged;

        /// <summary>
        /// 
        /// </summary>
        public event UpdateProgressChangedEventHandler UpdateProgressChanged;

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
            if (_fileSystem.Directory.Exists(updateFolder))
                _fileSystem.Directory.Delete(updateFolder, true);

            //check application version
            dynamic latestInfo = UpdateHelpers.GitReleaseInfo(applicationRepoUrl);
            string downloadUrl = latestInfo.assets[0].browser_download_url;

            UpdateMessageChanged?.Invoke(this, $"{StaticText.Downloading} {latestInfo.tag_name}");

            _fileSystem.Directory.CreateDirectory(updateFolder);
            WebClient client = UpdateHelpers.OpenWebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(selfUpdate_DownloadProgress);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(selfUpdate_DownloadCompleted);
            await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), Path.Combine(updateFolder, updateName));
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
