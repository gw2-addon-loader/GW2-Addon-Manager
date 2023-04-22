using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using GW2_Addon_Manager.App.Configuration;

namespace GW2_Addon_Manager
{
    static class UpdateHelpers
    {
        public static WebClient GetClient()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "Gw2 Addon Manager");

            return client;
        }

        public static string DownloadStringFromGithubAPI(this WebClient wc, string url)
        {
            try {
                return wc.DownloadString(url);
            }
            catch (WebException ex)
            {
                MessageBox.Show("Github servers returned an error; please try again in a few minutes.\n\nThe error was: " + ex.Message, "Github API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }
        }

        public static void DownloadFileFromGithubAPI(this WebClient wc, string url, string destPath)
        {
            try {
                wc.DownloadFile(url, destPath);
            }
            catch (WebException ex) {
                MessageBox.Show("Github servers returned an error; please try again in a few minutes.\n\nThe error was: " + ex.Message, "Github API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw ex;
            }
        }

        public static dynamic GitReleaseInfo(string gitUrl)
        {
            using (var client = UpdateHelpers.GetClient())
            {
                var release_info_json = client.DownloadStringFromGithubAPI(gitUrl);
                return JsonConvert.DeserializeObject(release_info_json);
            }

        }

        public static async void UpdateAll()
        {
            UpdatingViewModel viewModel = UpdatingViewModel.GetInstance;

            LoaderSetup settingUp = new LoaderSetup(new ConfigurationManager());
            await settingUp.HandleLoaderUpdate((bool)Application.Current.Properties["ForceLoader"]);

            List<AddonInfoFromYaml> addons = (List<AddonInfoFromYaml>)Application.Current.Properties["Selected"];
            
            var configurationManager = new ConfigurationManager();
            foreach (AddonInfoFromYaml addon in addons.Where(add => add != null))
            {
                GenericUpdater updater = new GenericUpdater(addon, configurationManager);
            
                if(!(addon.additional_flags != null && addon.additional_flags.Contains("self-updating") 
                     && configurationManager.UserConfig.AddonsList.FirstOrDefault(a => a.Name == addon.addon_name)?.Installed == true))
                    await updater.Update();
            }

            viewModel.ProgBarLabel = "Updates Complete";
            viewModel.DownloadProgress = 100;
            viewModel.CloseBtnEnabled = true;
            viewModel.BackBtnEnabled = true;
        }
    }
}
