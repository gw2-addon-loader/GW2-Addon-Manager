using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Windows;

namespace GW2_Addon_Manager
{
    class UpdateHelpers
    {
        public static dynamic GitReleaseInfo(string gitUrl)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string release_info_json = client.DownloadString(gitUrl);
            return JsonConvert.DeserializeObject(release_info_json);
        }

        public static async void UpdateAll(UpdatingViewModel viewModel)
        {
            List<AddonInfo> addons = (List<AddonInfo>)Application.Current.Properties["Selected"];
            
            //TODO: keep track of addons that need to have chainload set up
            foreach (AddonInfo addon in addons)
            {
                GenericUpdater updater = new GenericUpdater(addon.folder_name, viewModel);
                await updater.Update();
            }

            //need to do: chainloading renames
            //config file edit and set
        }

        //TODO: Completely overhaul configuration and pluginmanagement classes
    }
}
