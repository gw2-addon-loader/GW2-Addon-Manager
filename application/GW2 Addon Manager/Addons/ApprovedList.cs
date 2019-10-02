using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace GW2_Addon_Manager
{
    class ApprovedList
    {

        //TODO
        public void CheckListVersion()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of AddonInfo objects representing all approved add-ons.</returns>
        public static ObservableCollection<AddonInfo> GenerateAddonList()
        {
            ObservableCollection<AddonInfo> Addons = new ObservableCollection<AddonInfo>(); //List of AddonInfo objects

            string[] AddonDirectories = Directory.GetDirectories("resources\\addons");  //Names of addon subdirectories in /resources/addons
            config userConfig = configuration.getConfigAsYAML();
            foreach (string addonFolderName in AddonDirectories)
            {
                if (addonFolderName != "resources\\addons\\d3d9_wrapper")
                {
                    AddonInfo temp = AddonYamlReader.getAddonInInfo(addonFolderName.Replace("resources\\addons\\", ""));
                    temp.folder_name = addonFolderName.Replace("resources\\addons\\", "");
                    if (userConfig.default_configuration.ContainsKey(temp.folder_name) && userConfig.default_configuration[temp.folder_name])
                        temp.IsSelected = true;
                    Addons.Add(temp);       //retrieving info from each addon subdirectory's update.yaml file and adding it to the list
                }
            }

            return Addons;
        }
    }
}
