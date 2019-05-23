using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tolldo.Services
{
    /// <summary>
    /// Dialog service that implements the <see cref="IDialogService"/> interface.
    /// </summary>
    public class DialogService : IDialogService, INotifyPropertyChanged
    {
        #region Private Members

        private string _message;

        #endregion

        #region Public Helpers

        /// <summary>
        /// Gets the current message.
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return _message;
        }

        /// <summary>
        /// Sets the specified message and invokes the <see cref="MessageChanged"/> event.
        /// </summary>
        /// <param name="message">The message to be displayed. Null removes the message.</param>
        public void SetMessage(string message)
        {            
            _message = message;
            MessageChanged.Invoke(message, null);
        }

        #endregion

        #region Events

        /// <summary>
        /// The event that is fired when the message changes.
        /// </summary>
        public event EventHandler MessageChanged;

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
