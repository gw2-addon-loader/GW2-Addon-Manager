using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using GW2_Addon_Manager.App.Configuration;
using System;
using System.Text;

namespace GW2_Addon_Manager
{
    class UpdateHelpers
    {
        public static WebClient OpenWebClient()
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            #if DEBUG
            var token = Environment.GetEnvironmentVariable("GW2UAM_TOKEN");
            if(token != null) {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
                client.Headers[HttpRequestHeader.Authorization] = string.Format(
                    "Basic {0}", credentials);
            }
            #endif

            return client;
        }

        public static dynamic GitReleaseInfo(string gitUrl)
        {
            var client = OpenWebClient();
            try
            {
                string release_info_json = client.DownloadString(gitUrl);
                return JsonConvert.DeserializeObject(release_info_json);

            }
            catch (WebException)
            {
                return null;
            }
            
        }

        public static async void UpdateAll()
        {
            UpdatingViewModel viewModel = UpdatingViewModel.GetInstance;

            LoaderSetup settingUp = new LoaderSetup(new ConfigurationManager());
            await settingUp.HandleLoaderUpdate();

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
