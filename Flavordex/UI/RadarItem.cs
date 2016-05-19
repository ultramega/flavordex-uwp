using System.ComponentModel;

namespace Flavordex.UI
{
    /// <summary>
    /// Interface for items in a RadarControl.
    /// </summary>
    public interface RadarItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        long Value { get; set; }
    }
}
