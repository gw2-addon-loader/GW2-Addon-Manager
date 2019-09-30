using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
            LoaderSetup settingUp = new LoaderSetup(viewModel);
            await settingUp.handleLoaderInstall();

            List<AddonInfo> addons = (List<AddonInfo>)Application.Current.Properties["Selected"];
            
            foreach (AddonInfo addon in addons.Where(add => add != null))
            {
                GenericUpdater updater = new GenericUpdater(addon, viewModel);
                await updater.Update();
            }

            //need to do: chainloading renames
            //config file edit and set
        }

        /// <summary>
        /// Self Updater
        /// </summary>
        /// <param name="viewModel"></param>
        public static void UpdateSelf(OpeningViewModel viewModel)
        {
            SelfUpdate update = new SelfUpdate(viewModel);
        }
    }
}
