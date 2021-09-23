using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using GW2_Addon_Manager.App.Configuration;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    public class AvailableAddonList
    {
        private record BranchInfo(
            string Name,
            HeadInfo Commit,
            bool Protected);

        private record HeadInfo(
            string Sha,
            string Url);

        //Approved-addons repository
        private const string RepoUrl = "https://api.github.com/repositories/206052865";

        private readonly IConfigurationManager _configManager;
        private readonly AddonYamlReader _yaml;

        public AvailableAddonList(IConfigurationManager configManager, AddonYamlReader yaml)
        {
            _configManager = configManager;
            _yaml = yaml;
        }

        /// <summary>
        /// Check current version of addon list against remote repo for changes and fetch them
        /// </summary>
        public void Update()
        {
            string tempFileName = Path.GetTempFileName();
            var client = UpdateHelpers.OpenWebClient();

            var raw = client.DownloadString($"{RepoUrl}/branches");
            var result = JsonConvert.DeserializeObject<BranchInfo[]>(raw);
            var masterHash = result.Single(r => r.Name == "master").Commit.Sha;

            if (masterHash == _configManager.UserConfig.AddonsHash) return;

            if (Directory.Exists(Constants.AddonFolder))
                Directory.Delete(Constants.AddonFolder, true);
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

            client.DownloadFile($"{RepoUrl}/zipball", tempFileName);
            ZipFile.ExtractToDirectory(tempFileName, Constants.AddonFolder);
            var downloaded = Directory.EnumerateDirectories(Constants.AddonFolder).First();
            foreach (var entry in Directory.EnumerateFileSystemEntries(downloaded))
            {
                Directory.Move(entry, Path.Combine(Constants.AddonFolder, Path.GetFileName(entry)));
            }

            _configManager.UserConfig = _configManager.UserConfig with
            {
                AddonsHash = masterHash
            };
            _configManager.Save();

            Directory.Delete(downloaded, true);
            File.Delete(tempFileName);
        }

        public List<AddonInfo> GetCurrent()
        {
            var addons = new List<AddonInfo>(); //List of AddonInfo objects
            var addonDirectories = Directory.GetDirectories(Constants.AddonFolder);  //Names of addon subdirectories in /resources/addons
            foreach (var addonFolderName in addonDirectories)
            {
                if (addonFolderName == "resources\\addons\\d3d9_wrapper") continue;
                var addonInfo = _yaml.GetAddonInfo(Path.GetRelativePath(Constants.AddonFolder, addonFolderName));
                addons.Add(addonInfo);
            }

            return addons;
        }
    }
}
