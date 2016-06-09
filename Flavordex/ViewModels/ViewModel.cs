using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// The base class for all ViewModels.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a mutable property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Triggers the PropertyChanged event for the indicated property.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
