using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class AddonsList : IEnumerable<AddonData>
    {
        private readonly Dictionary<string, AddonData> _internalCollection = new Dictionary<string, AddonData>();

        public AddonData this[string addonName] => _internalCollection[addonName];

        public string Hash { get; set; }

        public IEnumerator<AddonData> GetEnumerator() => _internalCollection.Select(a => a.Value).GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(AddonData newAddon) => _internalCollection.Add(newAddon.Name, newAddon);

        public void Remove(string addonToRemoveName) => _internalCollection.Remove(addonToRemoveName);
    }
}