using System;

namespace Tolldo.Services
{
    /// <summary>
    /// Dialog service that handles message input and output.
    /// </summary>
    public interface IDialogService
    {
        event EventHandler MessageChanged;

        /// <summary>
        /// Set the specified message.
        /// </summary>
        /// <param name="message">Message to set.</param>
        void SetMessage(string message);

        /// <summary>
        /// Get the current message.
        /// </summary>
        /// <returns>Message.</returns>
        string GetMessage();
    }
}
