using System;

namespace Tolldo.Services
{
    /// <summary>
    /// Dialog service that implements the <see cref="IDialogService"/> interface.
    /// </summary>
    public class DialogService : IDialogService
    {
        #region Public Helpers

        /// <summary>
        /// Invokes the <see cref="MessageChanged"/> event.
        /// </summary>
        /// <param name="message">The message to be displayed. Null removes the message.</param>
        public void SetMessage(string message)
        {            
            // Invoke the message changed event and provide the new message
            MessageChanged.Invoke(message, EventArgs.Empty);
        }

        #endregion

        #region Events

        /// <summary>
        /// The event that is fired when the message changes.
        /// </summary>
        public event EventHandler MessageChanged;

        #endregion
    }
}
