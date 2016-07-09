using System.Collections.ObjectModel;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Contains a collection of imported CSV records along with other metadata.
    /// </summary>
    public class ImportCollection
    {
        /// <summary>
        /// The list of entries.
        /// </summary>
        public Collection<ImportRecord> Entries { get; } = new Collection<ImportRecord>();

        /// <summary>
        /// Whether the CSV file has a category column.
        /// </summary>
        public bool HasCategory { get; internal set; }

        /// <summary>
        /// The legacy format if detected.
        /// </summary>
        public string LegacyFormat { get; internal set; }
    }
}
