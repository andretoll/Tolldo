using System.Windows.Input;
using Tolldo.Models;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Task-object. Based on the TodoTask class.
    /// </summary>
    public class TaskViewModel : TodoTask
    {
        #region Private Members

        // Indicates if renaming task mode is active
        private bool _renameActive;

        #endregion

        #region Public Properties

        // Indicates if renaming task mode is active
        public bool RenameActive
        {
            get
            {
                return _renameActive;
            }
            set
            {
                _renameActive = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand ToggleRenameCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TaskViewModel()
        {
            // Commands
            ToggleRenameCommand = new RelayCommand.RelayCommand(p => { RenameActive = !RenameActive; });
        }

        #endregion
    }
}
