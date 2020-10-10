using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    [Serializable]
    public class AddonsList : IEnumerable<AddonData>, IXmlSerializable
    {
        private readonly Dictionary<string, AddonData> _internalCollection = new Dictionary<string, AddonData>();
        
        public AddonData this[string addonName]
        {
            get
            {
                var valueExists = _internalCollection.TryGetValue(addonName, out var addonData);
                return valueExists ? addonData : null;
            }
        }

        public string Hash { get; set; }

        public IEnumerator<AddonData> GetEnumerator() => _internalCollection.Select(a => a.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() != XmlNodeType.Element || reader.LocalName != nameof(AddonsList)) return;
            
            Hash = reader[nameof(Hash)];

            if (!reader.ReadToDescendant(nameof(AddonData))) return;
            
            while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(AddonData))
            {
                var serializer = new XmlSerializer(typeof(AddonData));
                var addonData = (AddonData) serializer.Deserialize(reader);
                _internalCollection.Add(addonData.Name, addonData);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Hash), Hash);

            foreach (var addonData in _internalCollection.Values)
            {
                var serializer = new XmlSerializer(typeof(AddonData));
                serializer.Serialize(writer, addonData);
            }
        }

        public void Add(AddonData newAddon) => _internalCollection.Add(newAddon.Name, newAddon);

        public void Remove(string addonToRemoveName) => _internalCollection.Remove(addonToRemoveName);
    }
}