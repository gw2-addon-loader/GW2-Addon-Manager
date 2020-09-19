using System.Configuration;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [ConfigurationCollection(typeof(AddonData), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName = "AddonData")]

    public class AddonsList : ConfigurationElementCollection
    {
        [ConfigurationProperty(nameof(Hash))]
        public string Hash
        {
            get => (string) this[nameof(Hash)];
            set => this[nameof(Hash)] = value;
        }

        protected override ConfigurationElement CreateNewElement() => new AddonData();

        protected override object GetElementKey(ConfigurationElement element) => ((AddonData) element).Name;
    }
}