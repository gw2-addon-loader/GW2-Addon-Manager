using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace GW2AddonManager
{
    public interface ILoaderManager : IUpdateChangedEvents
    {
        Task Update();
        void Uninstall();
    }

    public class LoaderManager : UpdateChangedEvents, ILoaderManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IAddonRepository _addonRepository;
        private readonly IAddonManager _addonManager;
        private readonly IFileSystem _fileSystem;
        private readonly IHttpClientProvider _httpClientProvider;

        public string InstallPath => _configurationProvider.UserConfig.GamePath;
        private string DownloadPath => _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "addon-loader.zip");

        public LoaderManager(IConfigurationProvider configurationProvider, IAddonRepository addonRepository, IAddonManager addonManager, IFileSystem fileSystem, IHttpClientProvider httpClientProvider, ICoreManager coreManager)
        {
            _configurationProvider = configurationProvider;
            _addonRepository = addonRepository;
            _addonManager = addonManager;
            _fileSystem = fileSystem;
            _httpClientProvider = httpClientProvider;
            coreManager.Uninstalling += (_, _) => Uninstall();
        }

        public async Task Update()
        {
            await _addonManager.Install(new List<AddonInfo> { _addonRepository.Loader.Wrapper });

            if (_addonRepository.Loader.VersionId == _configurationProvider.UserConfig.LoaderVersion)
                return;

            OnMessageChanged("Downloading Addon Loader");

            var fileName = DownloadPath;

            if (_fileSystem.File.Exists(fileName))
                _fileSystem.File.Delete(fileName);

            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await _httpClientProvider.Client.DownloadAsync(_addonRepository.Loader.DownloadUrl, fs, this);
            }

            Install(fileName);
        }

        private void Install(string fileName)
        {
            OnMessageChanged("Installing Addon Loader");

            Utils.RemoveFiles(_fileSystem, _configurationProvider.UserConfig.LoaderInstalledFiles, InstallPath);

            var relFiles = Utils.ExtractArchiveWithFilesList(fileName, InstallPath, _fileSystem);

            _configurationProvider.UserConfig = _configurationProvider.UserConfig with {
                LoaderVersion = _addonRepository.Loader.VersionId,
                LoaderInstalledFiles = relFiles
            };
        }

        public void Uninstall()
        {
            if (_configurationProvider.UserConfig.LoaderInstalledFiles?.Count > 0)
            {
                foreach (var f in _configurationProvider.UserConfig.LoaderInstalledFiles)
                    _fileSystem.File.DeleteIfExists(_fileSystem.Path.Combine(InstallPath, f));
            }
            else
            {
                // We don't know for sure what might be installed by the loader, but three files are consistently necessary, so remove those at least
                _fileSystem.File.DeleteIfExists(_fileSystem.Path.Combine(InstallPath, "dxgi.dll"));
                _fileSystem.File.DeleteIfExists(_fileSystem.Path.Combine(InstallPath, "d3d11.dll"));
                _fileSystem.File.DeleteIfExists(_fileSystem.Path.Combine(InstallPath, "bin64", "d3d9.dll"));
            }

            _configurationProvider.UserConfig = _configurationProvider.UserConfig with {
                LoaderInstalledFiles = new List<string>(),
                LoaderVersion = null
            };
        }
    }
}
