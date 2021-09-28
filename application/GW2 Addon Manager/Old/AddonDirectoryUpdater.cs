using System.Net;
using System.Threading.Tasks;
using GW2_Addon_Manager.Backend.Configuration;

namespace GW2_Addon_Manager.Backend.Configuration
{
    //should be called on application startup

    //need to add "addon_list" entry to version list in config_template.yaml

    class AddonDirectoryUpdater
    {

        private static readonly string ADDONS_REPO_URL = "https://api.github.com/repos/gw2-addon-loader/Approved-Addons/releases/latest";

        private async Task CheckUpdate()
        {
            UserConfig userConfig = Configuration.getConfigAsYAML();

            var client = new WebClient();
            client.Headers.Add("User-Agent", "request");

            dynamic release_info = UpdateHelpers.GitReleaseInfo(ADDONS_REPO_URL);
            string latestVersion = release_info.tag_name;

            if (latestVersion == userConfig.version["addon_list"])
                return;

            string download_link = release_info.assets[0].browser_download_url;
            await Download(download_link, client);
        }




        /***** DOWNLOAD *****/

        /// <summary>
        /// </summary>
        private async Task Download(string url, WebClient client)
        {
            //download archive to temp
            
            //delete addons list dir
            
            //extract downloaded zip to that dir

            //delete archive in temp

            //update field in config file



            await client.DownloadFileTaskAsync(new System.Uri(url), fileName);
        }
    }
}
