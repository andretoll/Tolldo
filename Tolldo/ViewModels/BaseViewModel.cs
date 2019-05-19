using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// A base ViewModel that implements the <see cref="NotifyPropertyChanged(string)"/> interface.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
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
