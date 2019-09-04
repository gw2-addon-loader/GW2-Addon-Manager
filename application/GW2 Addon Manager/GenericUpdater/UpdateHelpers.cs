using Newtonsoft.Json;
using System.Net;

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
    }
}
