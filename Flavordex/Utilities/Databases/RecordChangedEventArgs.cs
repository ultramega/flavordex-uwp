using System;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// The arguments for the RecordChanged event.
    /// </summary>
    public class RecordChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The type of change that occurred.
        /// </summary>
        public RecordChangedAction Action { get; }

        /// <summary>
        /// The Model representing the record that changed.
        /// </summary>
        public Model Model { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">The type of change that occurred.</param>
        /// <param name="model">The Model representing the record that changed.</param>
        internal RecordChangedEventArgs(RecordChangedAction action, Model model)
        {
            Action = action;
            Model = model;
        }
    }
}
