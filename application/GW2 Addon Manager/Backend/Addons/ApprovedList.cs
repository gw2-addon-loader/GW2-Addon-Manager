using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace GW2_Addon_Manager
{
    class ApprovedList
    {
        private const string AddonFolder = "resources\\addons";

        public static void FetchListFromRepo()
        {
            const string tempFileName = "addonlist";
            try
            {
                Directory.Delete(AddonFolder, true);
                File.Delete(tempFileName);
            }
            catch (Exception)
            {
                // ignored
            }

            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            client.DownloadFile("https://api.github.com/repositories/206052865/zipball", tempFileName);
            ZipFile.ExtractToDirectory(tempFileName, AddonFolder);
            var downloaded = Directory.EnumerateDirectories(AddonFolder).First();
            foreach (var entry in Directory.EnumerateFileSystemEntries(downloaded))
            {
                Directory.Move(entry, AddonFolder + "\\" + Path.GetFileName(entry));
            }
            Directory.Delete(downloaded, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of AddonInfo objects representing all approved add-ons.</returns>
        public static ObservableCollection<AddonInfoFromYaml> GenerateAddonList()
        {
            FetchListFromRepo();
            ObservableCollection<AddonInfoFromYaml> Addons = new ObservableCollection<AddonInfoFromYaml>(); //List of AddonInfo objects

            string[] AddonDirectories = Directory.GetDirectories(AddonFolder);  //Names of addon subdirectories in /resources/addons
            UserConfig userConfig = Configuration.getConfigAsYAML();
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
    }
}
