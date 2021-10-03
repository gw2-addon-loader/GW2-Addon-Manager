using GW2AddonManager.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System;
using System.Threading.Tasks;
using System.IO.Abstractions;

namespace GW2AddonManager
{
    public interface IAddonManager : IUpdateChangedEvents
    {
        Task Delete(params AddonInfo[] addons);
        Task Disable(params AddonInfo[]  addons);
        Task Enable(params AddonInfo[] addons);
        Task Install(params AddonInfo[] addons);

        string AddonsFolder { get; }
    }

    public class AddonManager : UpdateChangedEvents, IAddonManager
    {
        public string AddonsFolder => _fileSystem.Path.Combine(_configurationProvider.UserConfig.GamePath, "addons");
        private const string AddonPrefix = "gw2addon_";
        private const string ArcDPSFolder = "arcdps";
        private const string DisabledExtension = ".dll_disabled";
        private const string EnabledExtension = ".dll";

        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IHttpClientProvider _httpClientProvider;

        public AddonManager(IConfigurationProvider configurationProvider, IFileSystem fileSystem, IHttpClientProvider httpClientProvider)
        {
            _configurationProvider = configurationProvider;
            _fileSystem = fileSystem;
            _httpClientProvider = httpClientProvider;
        }

        private string FolderPath(AddonInfo addon)
        {
            return _fileSystem.Path.Combine(AddonsFolder, addon.InstallMode == InstallMode.Binary ? addon.Nickname : ArcDPSFolder);
        }

        private string DLLPath(AddonInfo addon, AddonState state)
        {
            var folderPath = FolderPath(addon);
            var extension = state.Disabled ? DisabledExtension : EnabledExtension;
            if(addon.InstallMode == InstallMode.Binary)
                return _fileSystem.Path.Combine(folderPath, AddonPrefix + addon.Nickname + extension);
            else if(addon.InstallMode == InstallMode.ArcDPSAddon) {
                if(addon.PluginName != null)
                    return _fileSystem.Path.Combine(folderPath, addon.PluginName + extension);
                else {
                    var files = _fileSystem.Directory.GetFiles(folderPath, addon.PluginNamePattern + extension);
                    if(files.Length > 0)
                        return files[0];
                    else
                        return null;
                }
            }
            else
                throw new ArgumentException();
        }

        private void DisableEnable(AddonInfo addon, Dictionary<string, AddonState> states, bool enable)
        {
            var state = states[addon.Nickname];
            if (!state.Installed || state.Disabled == !enable)
                return;

            var path = DLLPath(addon, state);
            if(_fileSystem.File.Exists(path)) {
                var newPath = _fileSystem.Path.ChangeExtension(path, enable ? EnabledExtension : DisabledExtension);
                _fileSystem.File.Move(path, newPath);
            }
            else
                throw new ArgumentException();

            states[addon.Nickname] = state with
            {
                Disabled = !enable
            };
        }

        private void Delete(AddonInfo addon, Dictionary<string, AddonState> states)
        {
            var state = states[addon.Nickname];
            if(!state.Installed)
                return;

            var folderPath = FolderPath(addon);
            var files = new List<string>
            {
                DLLPath(addon, state)
            };
            foreach (var f in addon.Files)
                files.Add(_fileSystem.Path.Combine(folderPath, f));
            foreach (var f in state.InstalledFiles)
                files.Add(_fileSystem.Path.Combine(folderPath, f));

            foreach (var f in files) {
                if (_fileSystem.File.Exists(f))
                    _fileSystem.File.Delete(f);
            }

            foreach(var dir in _fileSystem.Directory.EnumerateDirectories(folderPath, "*", SearchOption.AllDirectories).OrderByDescending(x => x.Length).Append(folderPath)) {
                try {
                    _fileSystem.Directory.Delete(dir);
                } catch(Exception) { }
            }

            states[addon.Nickname] = AddonState.Default(state.Nickname);
        }

        public Task Delete(params AddonInfo[] addons)
        {
            if (MessageBox.Show(StaticText.DeleteAddonsPrompt, StaticText.DeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                int i = 0;
                int n = addons.Length;
                OnProgressChanged(i, n);

                var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
                try {
                    foreach (AddonInfo addon in addons) {
                        OnMessageChanged($"Deleting {addon.AddonName}...");
                        Delete(addon, states);
                        OnProgressChanged(++i, n);
                    }
                }
                catch (Exception ex) {
                    // TODO: Logging
                    MessageBox.Show("Error while deleting some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _configurationProvider.UserConfig = _configurationProvider.UserConfig with { AddonsState = states };
            }

            return Task.CompletedTask;
        }

        /* 
         * Credit: Fidel @ StackOverflow
         * Modified version of their answer at https://stackoverflow.com/a/54616044/9170673
         */
        private string GetFilenameFromUrl(string url)
        {
            string result = "";

            var req = System.Net.WebRequest.Create(url);
            req.Method = "GET";
            using (System.Net.WebResponse resp = req.GetResponse()) {
                result = _fileSystem.Path.GetFileName(resp.ResponseUri.AbsoluteUri);
            }
            return result;
        }

        private async Task Install(AddonInfo addon, Dictionary<string, AddonState> states)
        {
            var state = states[addon.Nickname];
            if (state.Installed && (state.VersionId == addon.VersionId || addon.SelfUpdate))
                return;

            var url = addon.DownloadUrl;
            var baseFolder = Path.GetTempPath();
            var destFolder = FolderPath(addon);
            var fileName = Path.Combine(baseFolder, GetFilenameFromUrl(url));

            if (_fileSystem.File.Exists(fileName))
                _fileSystem.File.Delete(fileName);

            using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await _httpClientProvider.Client.DownloadAsync(url, fs, this);
            }

            if (state.Installed)
                Utils.RemoveFiles(_fileSystem, state.InstalledFiles, destFolder);

            List<string> relFiles;

            if (addon.DownloadType == DownloadType.Archive)
                relFiles = Utils.ExtractArchiveWithFilesList(fileName, destFolder, _fileSystem);
            else
                relFiles = new List<string>{ _fileSystem.Path.GetRelativePath(baseFolder, fileName) };


            states[addon.Nickname] = state with
            {
                Installed = true,
                VersionId = addon.VersionId,
                InstalledFiles = relFiles
            };

            _fileSystem.File.Delete(fileName);
        }

        public async Task Install(params AddonInfo[] addons)
        {
            int i = 0;
            int n = addons.Length;
            OnProgressChanged(i, n);

            var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
            try {
                foreach (AddonInfo addon in addons) {
                    OnMessageChanged($"Installing {addon.AddonName}...");
                    await Install(addon, states);
                    OnProgressChanged(++i, n);
                }
            }
            catch (Exception ex) {
                // TODO: Logging
                MessageBox.Show("Error while installing some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _configurationProvider.UserConfig = _configurationProvider.UserConfig with { AddonsState = states };
        }

        private void DisableEnable(bool enable, params AddonInfo[] addons)
        {
            int i = 0;
            int n = addons.Length;
            OnProgressChanged(i, n);

            var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
            try {
                foreach (AddonInfo addon in addons) {
                    OnMessageChanged($"{(enable ? "Enabling" : "Disabling")} {addon.AddonName}...");
                    DisableEnable(addon, states, enable);
                    OnProgressChanged(++i, n);
                }
            }
            catch (Exception ex) {
                // TODO: Logging
                MessageBox.Show($"Error while {(enable ? "enabling" : "disabling")} some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Task Disable(params AddonInfo[] addons)
        {
            if (MessageBox.Show(StaticText.DisableAddonsPrompt, StaticText.DisableTitle, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
                DisableEnable(false, addons);
            }

            return Task.CompletedTask;
        }

        public Task Enable(params AddonInfo[] addons)
        {
            if (MessageBox.Show(StaticText.EnableAddonsPrompt, StaticText.EnableTitle, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
                DisableEnable(true, addons);
            }

            return Task.CompletedTask;
        }
    }
}
