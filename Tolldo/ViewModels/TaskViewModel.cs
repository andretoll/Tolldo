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

        // Indicates if expanded mode is active
        private bool _expandedMode;

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

        // Indicates if expanded mode is active
        public bool ExpandedActive
        {
            get
            {
                return _expandedMode;
            }
            set
            {
                _expandedMode = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand ToggleRenameCommand { get; set; }
        public ICommand ToggleExpandedCommand { get; set; }
        public ICommand CheckTaskCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TaskViewModel()
        {
            // Commands
            ToggleRenameCommand = new RelayCommand.RelayCommand(p => { RenameActive = !RenameActive; });
            ToggleExpandedCommand = new RelayCommand.RelayCommand(p => { ExpandedActive = !ExpandedActive; });
            CheckTaskCommand = new RelayCommand.RelayCommand(p => { this.Completed = !this.Completed; });
        }

        #endregion
    }
}
