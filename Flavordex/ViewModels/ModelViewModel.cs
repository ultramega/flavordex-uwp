using Flavordex.Utilities.Databases;
using System;

namespace Flavordex.ViewModels
{
    /// <summary>
    /// The base class for all ViewModels with a backing Model.
    /// </summary>
    /// <typeparam name="T">The Type of Model to back this ViewModel implementation.</typeparam>
    public abstract class ModelViewModel<T> : ViewModel where T : Model
    {
        /// <summary>
        /// Gets the Model backing this ViewModel.
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The Model to back this ViewModel.</param>
        public ModelViewModel(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Model = model;
            Model.RecordChanged += OnRecordChanged;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ModelViewModel()
        {
            Model.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Raises the PropertyChanged for all properties when the Model changes.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRecordChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(null);
        }
    }
}
