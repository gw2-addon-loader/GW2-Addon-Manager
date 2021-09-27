using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    public interface IAddonRepository
    {
        IReadOnlyDictionary<string, AddonInfo> Addons { get; }
        void Refresh();
    }

    public class AddonRepository : IAddonRepository
    {
        // Master URL
        private const string RepoUrl = "https://gw2-addon-loader.github.io/addon-repo/addons.json";

        Dictionary<string, AddonInfo> _addons;
        public IReadOnlyDictionary<string, AddonInfo> Addons => _addons;

        public AddonRepository()
        {
            Refresh();
        }

        public void Refresh()
        {
            var client = UpdateHelpers.OpenWebClient();
            var raw = client.DownloadString(RepoUrl);
            _addons = JsonConvert.DeserializeObject<Dictionary<string, AddonInfo>>(raw);
        }
    }
}
