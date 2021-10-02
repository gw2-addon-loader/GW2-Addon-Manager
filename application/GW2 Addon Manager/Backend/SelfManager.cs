using GW2AddonManager.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Threading;
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
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly IConfigurationProvider _configurationProvider;

        private string DownloadPath => _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "addon-manager.zip");

        public SelfManager(IFileSystem fileSystem, IAddonRepository addonRepository, IHttpClientProvider httpClientProvider, IConfigurationProvider configurationProvider)
        {
            _fileSystem = fileSystem;
            _addonRepository = addonRepository;
            _httpClientProvider = httpClientProvider;
            _configurationProvider = configurationProvider;
        }

        public async Task Update()
        {
            if(_fileSystem.File.Exists(DownloadPath))
                _fileSystem.File.Delete(DownloadPath);

            if(_configurationProvider.ApplicationVersion == _addonRepository.Manager.VersionId)
                return;

            OnMessageChanged($"{StaticText.Downloading} {_addonRepository.Manager.VersionId}");

            using(var fs = new FileStream(DownloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await _httpClientProvider.Client.DownloadAsync(_addonRepository.Manager.DownloadUrl, fs, this);
            }

            OnMessageChanged($"{StaticText.DownloadComplete}!");
            Thread.Sleep(1000);

            _ = Process.Start("UOAOM Updater.exe");
            Application.Current.Shutdown();
        }
    }
}
