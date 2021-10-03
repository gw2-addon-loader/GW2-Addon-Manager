using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2AddonManager
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public record LoaderInfo(string VersionId, string DownloadUrl, string WrapperNickname)
    {
        [JsonIgnore]
        public AddonInfo Wrapper { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public record ManagerInfo(string VersionId, string DownloadUrl);

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    internal record AddonRepositoryInfo(Dictionary<string, AddonInfo> Addons, LoaderInfo Loader, ManagerInfo Manager);

    public interface IAddonRepository
    {
        IReadOnlyDictionary<string, AddonInfo> Addons { get; }
        LoaderInfo Loader { get; }
        ManagerInfo Manager { get; }
        Task Refresh();
    }

    public class AddonRepository : IAddonRepository
    {
        // Master URL
        private const string RepoUrl = "https://gw2-addon-loader.github.io/addon-repo/addons.json";
        private readonly IHttpClientProvider _httpClientProvider;
        AddonRepositoryInfo _info;
        public IReadOnlyDictionary<string, AddonInfo> Addons => _info.Addons;
        public LoaderInfo Loader => _info.Loader;
        public ManagerInfo Manager => _info.Manager;

        public AddonRepository(IHttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
            Task.Run(() => Refresh()).Wait();
        }

        public async Task Refresh()
        {
            var raw = await _httpClientProvider.Client.GetStringAsync(RepoUrl);
            _info = JsonConvert.DeserializeObject<AddonRepositoryInfo>(raw);
            _info.Loader.Wrapper = _info.Addons[_info.Loader.WrapperNickname];
        }
    }
}
