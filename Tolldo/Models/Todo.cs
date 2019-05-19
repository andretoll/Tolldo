using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tolldo.Models
{
    /// <summary>
    /// The model for Todo-items. Implements the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public class Todo : INotifyPropertyChanged
    {
        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ObservableCollection<TodoTask> Tasks { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// The event that is fired when any child property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
