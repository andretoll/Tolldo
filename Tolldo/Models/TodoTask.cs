using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tolldo.Models
{
    public class TodoTask : INotifyPropertyChanged
    {
        #region Private Members

        private bool _completed;

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name { get; set; }

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
    }
}
