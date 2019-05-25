using GongSolutions.Wpf.DragDrop;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Tolldo.Models;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Task-object. Based on the TodoTask class.
    /// </summary>
    public class TaskViewModel : TodoTask, IDropTarget
    {
        #region Private Members

        // Indicates if renaming task mode is active
        private bool _renameActive;

        // Indicates if expanded mode is active
        private bool _expandedActive;

        // New subtask name
        private string _newSubtaskName;

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
                return _expandedActive;
            }
            set
            {
                _expandedActive = value;
                NotifyPropertyChanged();
            }
        }

        private bool _newSubtaskAdded;
        public bool NewSubtaskAdded
        {
            get
            {
                return _newSubtaskAdded;
            }
            set
            {
                _newSubtaskAdded = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if new subtask mode is active
        public string NewSubtaskName
        {
            get
            {
                return _newSubtaskName == null ? "" : _newSubtaskName;
            }
            set
            {
                _newSubtaskName = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that fires when a subtask's property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Subtask_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Determine task completion if the completed property of any subtask changed
            if (e.PropertyName == "Completed")
            {
                // Indicates if all subtasks are completed
                bool subtasksComplete = true;

                foreach (var subtask in Subtasks)
                {
                    if (!subtask.Completed)
                    {
                        // If any subtask is incomplete, set false
                        subtasksComplete = false;
                        break;
                    }
                }

                // If value has changed, set completed status indirectly to avoid infinite loop
                if (_completed != subtasksComplete)
                {
                    _completed = subtasksComplete;
                    NotifyPropertyChanged(nameof(Completed)); 
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ToggleRenameCommand { get; set; }
        public ICommand ToggleExpandedCommand { get; set; }
        public ICommand CheckTaskCommand { get; set; }
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand DeleteSubtaskCommand { get; set; }


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
            AddSubtaskCommand = new RelayCommand.RelayCommand(p => { AddSubtask(); }, p => NewSubtaskName.Length > 0);
            DeleteSubtaskCommand = new RelayCommand.RelayCommand(p => { DeleteSubtask(p as Subtask); });
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Adds the current new subtask to the collection.
        /// </summary>
        private void AddSubtask()
        {
            Subtask subtask = new Subtask()
            {
                Id = 1,
                Completed = true,
                Name = this.NewSubtaskName
            };

            // Subscribe to property changed event for new subtask
            subtask.PropertyChanged += Subtask_PropertyChanged;
            subtask.Completed = this.Completed;

            Subtasks.Add(subtask);

            // Reset new subtask
            NewSubtaskName = null;
            NewSubtaskAdded = true;
            NewSubtaskAdded = false;
        }

        /// <summary>
        /// Deletes the specified subtask from the collection.
        /// </summary>
        /// <param name="subtask">The subtask to delete.</param>
        private void DeleteSubtask(Subtask subtask)
        {
            if (subtask != null)
                Subtasks.Remove(subtask);
        }

        #endregion

        #region DragDrop Interface

        /// <summary>
        /// Method called when dragging an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Get source and target
            Subtask sourceItem = dropInfo.Data as Subtask;
            Subtask targetItem = dropInfo.TargetItem as Subtask;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Method called when dropping an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Get source and target
            Subtask sourceItem = dropInfo.Data as Subtask;
            Subtask targetItem = dropInfo.TargetItem as Subtask;

            // Let the tasks switch places
            Subtasks.Move(Subtasks.IndexOf(sourceItem), Subtasks.IndexOf(targetItem));
        }

        #endregion
    }
}
