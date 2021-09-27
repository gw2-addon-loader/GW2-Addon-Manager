using Localization;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System;

namespace GW2_Addon_Manager
{
    public interface IAddonManager
    {
        void Delete(IEnumerable<AddonInfo> addons);
        void Disable(IEnumerable<AddonInfo> addons);
        void Enable(IEnumerable<AddonInfo> addons);
    }

    public class AddonManager : IAddonManager
    {
        private const string AddonsFolder = "addons";
        private const string AddonPrefix = "gw2addon_";
        private const string ArcDPSFolder = "arcdps";
        private const string DisabledExtension = ".dll_disabled";
        private const string EnabledExtension = ".dll";

        private readonly IConfigurationProvider _configurationProvider;

        public AddonManager(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        private string FolderPath(AddonInfo addon)
        {
            return Path.Combine(_configurationProvider.UserConfig.GamePath, AddonsFolder, addon.InstallMode == InstallMode.Binary ? addon.Nickname : ArcDPSFolder);
        }

        private string DLLPath(AddonInfo addon, AddonState state)
        {
            var folderPath = FolderPath(addon);
            var extension = state.Disabled ? DisabledExtension : EnabledExtension;
            if(addon.InstallMode == InstallMode.Binary)
                return Path.Combine(folderPath, AddonPrefix + addon.Nickname + extension);
            else if(addon.InstallMode == InstallMode.ArcDPSAddon) {
                if(addon.PluginName != null)
                    return Path.Combine(folderPath, addon.PluginName + extension);
                else {
                    var files = Directory.GetFiles(folderPath, addon.PluginNamePattern + extension);
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
            if(File.Exists(path)) {
                var newPath = Path.ChangeExtension(path, enable ? EnabledExtension : DisabledExtension);
                File.Move(path, newPath);
            }
            else
                throw new ArgumentException();
        }

        private void Delete(AddonInfo addon, Dictionary<string, AddonState> states)
        {
            var state = states[addon.Nickname];
            if(!state.Installed)
                return;

            if(addon.InstallMode == InstallMode.ArcDPSAddon) {
                var path = DLLPath(addon, state);
                if(path != null) File.Delete(path);
                if(addon.Files.Count > 0) {
                    var folderPath = FolderPath(addon);
                    foreach (var f in addon.Files) {
                        var filePath = Path.Combine(folderPath, f);
                        if(File.Exists(filePath))
                            File.Delete(filePath);
                        else if(Directory.Exists(filePath))
                            Directory.Delete(filePath, true);
                    }
                }
            }

            states[addon.Nickname] = AddonState.Default(state.Nickname);
        }

        public void Delete(IEnumerable<AddonInfo> addons)
        {
            if (DelayedMessageBox.Show(3, StaticText.DeleteAddonsPrompt, StaticText.DeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
                try {
                    foreach (AddonInfo addon in addons)
                        Delete(addon, states);
                }
                catch (Exception ex) {
                    // TODO: Logging
                    MessageBox.Show("Error while deleting some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _configurationProvider.UserConfig = _configurationProvider.UserConfig with { AddonsState = states };
            }
        }

        private void Install(AddonInfo addon, Dictionary<string, AddonState> states)
        {
            var state = states[addon.Nickname];
            if (state.Installed)
                return;

            // TODO: Finish
        }

        public void Install(IEnumerable<AddonInfo> addons)
        {
            if (DelayedMessageBox.Show(3, StaticText.DeleteAddonsPrompt, StaticText.DeleteTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
                try {
                    foreach (AddonInfo addon in addons)
                        Install(addon, states);
                }
                catch (Exception ex) {
                    // TODO: Logging
                    MessageBox.Show("Error while installing some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _configurationProvider.UserConfig = _configurationProvider.UserConfig with { AddonsState = states };
            }
        }

        private void DisableEnable(IEnumerable<AddonInfo> addons, bool enable)
        {
            var states = new Dictionary<string, AddonState>(_configurationProvider.UserConfig.AddonsState);
            try {
                foreach (AddonInfo addon in addons)
                    DisableEnable(addon, states, enable);
            }
            catch (Exception ex) {
                // TODO: Logging
                MessageBox.Show($"Error while {(enable ? "enabling" : "disabling")} some addons: " + ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Disable(IEnumerable<AddonInfo> addons)
        {
            if (MessageBox.Show(StaticText.DisableAddonsPrompt, StaticText.DisableTitle, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
                DisableEnable(addons, false);
            }
        }

        public void Enable(IEnumerable<AddonInfo> addons)
        {
            if (MessageBox.Show(StaticText.EnableAddonsPrompt, StaticText.EnableTitle, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
                DisableEnable(addons, true);
            }
        }
    }
}
