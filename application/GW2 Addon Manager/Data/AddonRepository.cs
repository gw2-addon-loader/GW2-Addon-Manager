using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GW2_Addon_Manager
{
    public record LoaderInfo();
    public record ManagerInfo();

    internal record AddonRepositoryInfo(Dictionary<string, AddonInfo> Addons, LoaderInfo Loader, ManagerInfo Manager);

    public interface IAddonRepository
    {
        IReadOnlyDictionary<string, AddonInfo> Addons { get; }
        LoaderInfo Loader { get; }
        ManagerInfo Manager { get; }
        void Refresh();
    }

    public class AddonRepository : IAddonRepository
    {
        // Master URL
        private const string RepoUrl = "https://gw2-addon-loader.github.io/addon-repo/addons.json";

        AddonRepositoryInfo _info;
        public IReadOnlyDictionary<string, AddonInfo> Addons => _info.Addons;
        public LoaderInfo Loader => _info.Loader;
        public ManagerInfo Manager => _info.Manager;

        public AddonRepository()
        {
            Refresh();
        }

        public void Refresh()
        {
            var client = Utils.OpenWebClient();
            var raw = client.DownloadString(RepoUrl);
            _info = JsonConvert.DeserializeObject<AddonRepositoryInfo>(raw);
        }
    }
}
