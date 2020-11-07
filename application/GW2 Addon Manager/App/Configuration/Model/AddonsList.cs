using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GW2_Addon_Manager.App.Configuration.Model
{
    /// <summary>
    ///     Addons list model.
    ///     It is custom dictionary-shaped enumerable, that serializes to flat list in xml.
    ///     Key is the addon name, and value is addon data <see cref="AddonData" />.
    /// </summary>
    [Serializable]
    public class AddonsList : IEnumerable<AddonData>, IXmlSerializable
    {
        private readonly Dictionary<string, AddonData> _internalCollection = new Dictionary<string, AddonData>();

        /// <summary>
        ///     Indexer to retrive addon data by name.
        /// </summary>
        /// <param name="addonName">Addon name.</param>
        /// <returns></returns>
        public AddonData this[string addonName]
        {
            get
            {
                var valueExists = _internalCollection.TryGetValue(addonName, out var addonData);
                return valueExists ? addonData : null;
            }
        }

        /// <summary>
        ///     Current addon list hash. Used to check for updates.
        /// </summary>
        public string Hash { get; set; }

        /// <inheritdoc cref="IEnumerable.GetEnumerator" />
        public IEnumerator<AddonData> GetEnumerator() => _internalCollection.Select(a => a.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc cref="IXmlSerializable.GetSchema" />
        public XmlSchema GetSchema() => null;

        /// <inheritdoc cref="IXmlSerializable.ReadXml" />
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

        /// <summary>
        ///     Used to serialize class into xml as list.
        ///     <inheritdoc cref="IXmlSerializable.WriteXml" />
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Hash), Hash);

            foreach (var addonData in _internalCollection.Values)
            {
                var serializer = new XmlSerializer(typeof(AddonData));
                serializer.Serialize(writer, addonData);
            }
        }

        /// <summary>
        ///     Adds addon data to list.
        /// </summary>
        /// <param name="newAddon">Addon data.</param>
        public void Add(AddonData newAddon) => _internalCollection.Add(newAddon.Name, newAddon);

        /// <summary>
        ///     Removes addon from list.
        /// </summary>
        /// <param name="addonToRemoveName">Addon name.</param>
        public void Remove(string addonToRemoveName) => _internalCollection.Remove(addonToRemoveName);
    }
}