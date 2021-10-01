using GW2AddonManager.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GW2AddonManager
{
    public interface ISelfManager : IUpdateChangedEvents
    {
        Task Update();
    }

    public class SelfManager : UpdateChangedEvents, ISelfManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAddonRepository _addonRepository;

        private string DownloadPath => _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "addon-manager.zip");

        public SelfManager(IFileSystem fileSystem, IAddonRepository addonRepository)
        {
            _fileSystem = fileSystem;
            _addonRepository = addonRepository;
        }

        public async Task Update()
        {
            if(_fileSystem.File.Exists(DownloadPath))
                _fileSystem.File.Delete(DownloadPath);

            if (_fileSystem.Directory.Exists(updateFolder))
                _fileSystem.Directory.Delete(updateFolder, true);

            OnMessageChanged($"{StaticText.Downloading} {_addonRepository.Manager.VersionId}");

            _fileSystem.Directory.CreateDirectory(updateFolder);
            WebClient client = UpdateHelpers.OpenWebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((_, e) => OnProgressChanged(e.ProgressPercentage, 100));
            client.DownloadFileCompleted += new AsyncCompletedEventHandler((_, _) => {
                OnProgressChanged(100, 100);
                OnMessageChanged($"{StaticText.DownloadComplete}!");
                Process.Start("UOAOM Updater.exe");
                Application.Current.Shutdown();
            });

            await client.DownloadFileTaskAsync(new System.Uri(downloadUrl), Path.Combine(updateFolder, updateName));
        }
    }
}
