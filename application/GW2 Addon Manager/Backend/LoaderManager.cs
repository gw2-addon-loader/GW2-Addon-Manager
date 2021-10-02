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
        private readonly IConfigurationProvider _configurationManager;
        private readonly IAddonRepository _addonRepository;
        private readonly IAddonManager _addonManager;
        private readonly IFileSystem _fileSystem;
        private readonly IHttpClientProvider _httpClientProvider;

        public string InstallPath => _configurationManager.UserConfig.GamePath;
        private string DownloadPath => _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "addon-loader.zip");

        public LoaderManager(IConfigurationProvider configurationManager, IAddonRepository addonRepository, IAddonManager addonManager, IFileSystem fileSystem, IHttpClientProvider httpClientProvider)
        {
            _configurationManager = configurationManager;
            _addonRepository = addonRepository;
            _addonManager = addonManager;
            _fileSystem = fileSystem;
            _httpClientProvider = httpClientProvider;
        }

        public async Task Update()
        {
            await _addonManager.Install(_addonRepository.Loader.Wrapper);

            if (_addonRepository.Loader.VersionId == _configurationManager.UserConfig.LoaderVersion)
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

            Utils.RemoveFiles(_fileSystem, _configurationManager.UserConfig.LoaderInstalledFiles, InstallPath);

            var relFiles = Utils.ExtractArchiveWithFilesList(fileName, InstallPath, _fileSystem);

            _configurationManager.UserConfig = _configurationManager.UserConfig with {
                LoaderVersion = _addonRepository.Loader.VersionId,
                LoaderInstalledFiles = relFiles
            };
        }

        public void Uninstall()
        {
            if(_configurationManager.UserConfig.LoaderInstalledFiles?.Count > 0) {
                foreach(var f in _configurationManager.UserConfig.LoaderInstalledFiles)
                    _fileSystem.File.Delete(_fileSystem.Path.Combine(InstallPath, f));
            } else {
                // We don't know for sure what might be installed by the loader, but three files are consistently necessary, so remove those at least
                _fileSystem.File.Delete(_fileSystem.Path.Combine(InstallPath, "dxgi.dll"));
                _fileSystem.File.Delete(_fileSystem.Path.Combine(InstallPath, "d3d11.dll"));
                _fileSystem.File.Delete(_fileSystem.Path.Combine(InstallPath, "bin64", "d3d9.dll"));
            }

            _configurationManager.UserConfig = _configurationManager.UserConfig with {
                LoaderInstalledFiles = new List<string>(),
                LoaderVersion = null
            };
        }
    }
}
