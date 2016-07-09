using Flavordex.Models;
using System.Collections.ObjectModel;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Contains data for a record parsed from a CSV file.
    /// </summary>
    public class ImportRecord
    {
        /// <summary>
        /// Gets or sets the journal entry data.
        /// </summary>
        public Entry Entry { get; internal set; }

        /// <summary>
        /// Gets the extra fields for the journal entry.
        /// </summary>
        public Collection<EntryExtra> Extras { get; } = new Collection<EntryExtra>();

        /// <summary>
        /// Gets the flavors for the journal entry.
        /// </summary>
        public Collection<EntryFlavor> Flavors { get; } = new Collection<EntryFlavor>();

        /// <summary>
        /// Gets the photos for the journal entry.
        /// </summary>
        public Collection<Photo> Photos { get; } = new Collection<Photo>();

        /// <summary>
        /// Whether this record has been detected as a duplicate.
        /// </summary>
        public bool IsDuplicate { get; internal set; }
    }
}
