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

            foreach (string addonFolderName in AddonDirectories)
                Addons.Add(UpdateYamlReader.getBuiltInInfo(addonFolderName));       //retrieving info from each addon subdirectory's update.yaml file and adding it to the list

            return Addons;
        }
    }
}
