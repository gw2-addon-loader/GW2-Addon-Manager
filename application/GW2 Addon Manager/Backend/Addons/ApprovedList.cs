using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using GW2_Addon_Manager.App.Configuration;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    public class ApprovedList
    {
        private const string AddonFolder = "resources\\addons";
        //Approved-addons repository
        private const string RepoUrl = "https://api.github.com/repositories/206052865";

        private readonly IConfigurationManager _configManager;
        private readonly AddonYamlReader _yaml;

        public ApprovedList(IConfigurationManager configManager, AddonYamlReader yaml)
        {
            _configManager = configManager;
            _yaml = yaml;
        }

        /// <summary>
        /// Check current version of addon list against remote repo for changes and fetch them
        /// </summary>
        public void FetchListFromRepo()
        {
            const string tempFileName = "addonlist";
            var client = UpdateHelpers.OpenWebClient();

            var raw = client.DownloadString(RepoUrl + "/branches");
            var result = JsonConvert.DeserializeObject<BranchInfo[]>(raw);
            var master = result.Single(r => r.Name == "master").Commit.Sha;

            if (master == _configManager.UserConfig.AddonsList.Hash) return;

            if (Directory.Exists(AddonFolder))
                Directory.Delete(AddonFolder, true);
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);

            //fetching new version
            client = UpdateHelpers.OpenWebClient();
            client.DownloadFile(RepoUrl + "/zipball", tempFileName);
            ZipFile.ExtractToDirectory(tempFileName, AddonFolder);
            var downloaded = Directory.EnumerateDirectories(AddonFolder).First();
            foreach (var entry in Directory.EnumerateFileSystemEntries(downloaded))
            {
                Directory.Move(entry, AddonFolder + "\\" + Path.GetFileName(entry));
            }

            //updating version in config file
            _configManager.UserConfig.AddonsList.Hash = master;
            _configManager.SaveConfiguration();

            //cleanup
            Directory.Delete(downloaded, true);
            File.Delete(tempFileName);
        }

        /// <summary>
        /// Scans resources/addons directory to populate a collection used for displaying the list of available addons on the UI.
        /// </summary>
        /// <returns>A list of AddonInfo objects representing all approved add-ons.</returns>
        public ObservableCollection<AddonInfo> GenerateAddonList()
        {
            FetchListFromRepo();

            var addons = new ObservableCollection<AddonInfo>(); //List of AddonInfo objects
            var addonDirectories = Directory.GetDirectories(AddonFolder);  //Names of addon subdirectories in /resources/addons
            foreach (var addonFolderName in addonDirectories)
            {
                if (addonFolderName == "resources\\addons\\d3d9_wrapper") continue;

                var addonInfo = _yaml.GetAddonInfo(addonFolderName.Replace(AddonFolder + "\\", ""));
                addonInfo.FolderName = addonFolderName.Replace(AddonFolder + "\\", "");
                addonInfo.IsSelected = _configManager.UserConfig.AddonsList[addonInfo.FolderName]?.Installed ?? false;
                addons.Add(addonInfo);       //retrieving info from each addon subdirectory's update.yaml file and adding it to the list
            }

            return addons;
        }

        private struct BranchInfo
        {
            public string Name;
            public HeadInfo Commit;
            public bool Protected;
        }

        private struct HeadInfo
        {
            public string Sha;
            public string Url;
        }
    }
}
