using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tolldo.Models
{
    /// <summary>
    /// The model for Task-items. Implements the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public class TodoTask : INotifyPropertyChanged
    {
        #region Private Members

        private string _name;
        private bool _completed;

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                NotifyPropertyChanged();
            }
        }

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
