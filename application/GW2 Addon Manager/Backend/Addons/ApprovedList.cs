using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    class ApprovedList
    {
        private const string AddonFolder = "resources\\addons";
        //Approved-addons repository
        private const string RepoUrl = "https://api.github.com/repositories/206052865";

        /// <summary>
        /// Check current version of addon list against remote repo for changes and fetch them
        /// </summary>
        /// <param name="cfg">User's application configuration found in config.yaml</param>
        public static void FetchListFromRepo(UserConfig cfg)
        {
            const string tempFileName = "addonlist";
            var client = new WebClient();
            client.Headers.Add("User-Agent", "Gw2 Addon Manager");
            
            var raw = client.DownloadString(RepoUrl + "/branches");
            var result = JsonConvert.DeserializeObject<BranchInfo[]>(raw);
            string master = null;

            //checking for changes
            foreach (var info in result)
            {
                if (info.Name != "master") continue;

                if (info.Commit.Sha == cfg.current_addon_list) return;

                master = info.Commit.Sha;
                break;
            }

            //deleting old version
            try
            {
                Directory.Delete(AddonFolder, true);
                File.Delete(tempFileName);
            }
            catch (Exception)
            {
                // ignored
            }

            //fetching new version
            client = new WebClient();
            client.Headers.Add("User-Agent", "Gw2 Addon Manager");
            client.DownloadFile(RepoUrl + "/zipball", tempFileName);
            ZipFile.ExtractToDirectory(tempFileName, AddonFolder);
            var downloaded = Directory.EnumerateDirectories(AddonFolder).First();
            foreach (var entry in Directory.EnumerateFileSystemEntries(downloaded))
            {
                Directory.Move(entry, AddonFolder + "\\" + Path.GetFileName(entry));
            }

            //updating version in config file
            cfg.current_addon_list = master;
            Configuration.setConfigAsYAML(cfg);

            //cleanup
            Directory.Delete(downloaded, true);
            File.Delete(tempFileName);
        }


        /// <summary>
        /// Scans resources/addons directory to populate a collection used for displaying the list of available addons on the UI.
        /// </summary>
        /// <returns>A list of AddonInfo objects representing all approved add-ons.</returns>
        public static ObservableCollection<AddonInfoFromYaml> GenerateAddonList()
        {
            UserConfig userConfig = Configuration.getConfigAsYAML();
            FetchListFromRepo(userConfig);
            ObservableCollection<AddonInfoFromYaml> Addons = new ObservableCollection<AddonInfoFromYaml>(); //List of AddonInfo objects

            string[] AddonDirectories = Directory.GetDirectories(AddonFolder);  //Names of addon subdirectories in /resources/addons
            foreach (string addonFolderName in AddonDirectories)
            {
                if (addonFolderName != "resources\\addons\\d3d9_wrapper")
                {
                    AddonInfoFromYaml temp = AddonYamlReader.getAddonInInfo(addonFolderName.Replace(AddonFolder + "\\", ""));
                    temp.folder_name = addonFolderName.Replace(AddonFolder + "\\", "");
                    if (userConfig.default_configuration.ContainsKey(temp.folder_name) && userConfig.default_configuration[temp.folder_name])
                        temp.IsSelected = true;
                    Addons.Add(temp);       //retrieving info from each addon subdirectory's update.yaml file and adding it to the list
                }
            }

            return Addons;
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
