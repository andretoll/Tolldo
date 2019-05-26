using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tolldo.Models;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Todo-object. Based on the <see cref="Todo"/> class and implements the <see cref="IDropTarget"/> interface.
    /// </summary>
    public class TodoViewModel : Todo, IDropTarget
    {
        #region Private Members

        // Dialog service
        private readonly IDialogService _dialogService;

        // Current progress
        private int _progress;

        // Last known progress
        private int _lastProgress;

        // Indicates if renaming mode is active
        private bool _renameActive;        

        // Indicates if new task is being created
        private bool _newTaskActive;

        // Selected task
        private TaskViewModel _selectedTask;

        // New task
        private TaskViewModel _newTask;

        // Tasks
        private ObservableCollection<TaskViewModel> _tasks;

        #endregion

        #region Public Properties

        // Indicates if renaming mode is active
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

        // The new name for renaming
        public string NewName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // Indicates if new task is being created
        public bool NewTaskActive
        {
            get
            {
                return _newTaskActive;
            }
            set
            {
                _newTaskActive = value;
                NotifyPropertyChanged();

                // Set text when beginning the new task
                if (value)
                {
                    NewTask = new TaskViewModel(_dialogService);
                    NewTask.Name = "";
                }
                else
                {
                    NewTask.Name = "Add new";
                }
            }
        }

        // Selected task
        public TaskViewModel SelectedTask
        {
            get
            {
                return _selectedTask;
            }
            set
            {
                if (_selectedTask != null)
                {
                    // Reset active modes
                    _selectedTask.RenameActive = false;
                }

                _selectedTask = value;
                NotifyPropertyChanged();
            }
        }

        // New task
        public TaskViewModel NewTask
        {
            get
            {
                return _newTask;
            }
            set
            {
                _newTask = value;
                NotifyPropertyChanged();
            }
        }

        // Tasks
        public ObservableCollection<TaskViewModel> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand CompleteAllTasksCommand { get; set; }
        public ICommand UncompleteAllTasksCommand { get; set; }

        public ICommand ToggleRenameCommand { get; set; }
        public ICommand ToggleNewTaskCommand { get; set; }

        public ICommand SaveNewTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }

        #endregion        

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TodoViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            // Initialize commands
            CompleteAllTasksCommand = new RelayCommand.RelayCommand(async p => { await CompleteAllTasks(); }, p => Tasks.Count > 0);
            UncompleteAllTasksCommand = new RelayCommand.RelayCommand(async p => { await UncompleteAllTasks(); }, p => Tasks.Count > 0);

            ToggleRenameCommand = new RelayCommand.RelayCommand(p => { RenameActive = !RenameActive; });
            ToggleNewTaskCommand = new RelayCommand.RelayCommand(p => { NewTaskActive = bool.Parse((string)p); });

            SaveNewTaskCommand = new RelayCommand.RelayCommand(p => { AddTask(); }, p => (NewTaskActive && !string.IsNullOrEmpty(NewTask.Name)));
            DeleteTaskCommand = new RelayCommand.RelayCommand(p => { DeleteTask(); });
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Calculated progress based on completed tasks.
        /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                NotifyPropertyChanged();

                // Notify change in dependent properties
                NotifyPropertyChanged("ProgressString");
                NotifyPropertyChanged("Completed");
            }
        }

        /// <summary>
        /// Calculated progress based on last completed task
        /// </summary>
        public int LastProgress
        {
            get
            {
                return _lastProgress;
            }
            set
            {
                _lastProgress = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Calculated progress based on completed tasks in <see cref="String"/> format.
        /// </summary>
        public string ProgressString
        {
            get
            {
                int p = 0;

                foreach (var task in Tasks)
                {
                    p += task.Completed ? 1 : 0;
                }

                return $"{p}/{Tasks.Count}";
            }
        }

        /// <summary>
        /// Calculated completion status based on completed tasks.
        /// </summary>
        public bool Completed
        {
            get
            {
                foreach (var task in Tasks)
                {
                    if (!task.Completed)
                        return false;
                }

                return Tasks.Count > 0 ? true : false;
            }
        }        

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates the current and last known progress.
        /// </summary>
        public void UpdateProgress()
        {
            LastProgress = Progress;
            Progress = CalculateProgress();
        }

        /// <summary>
        /// Calculates the current progress based on number of completed tasks.
        /// </summary>
        /// <returns></returns>
        public int CalculateProgress()
        {
            int p = 0;

            foreach (var task in Tasks)
            {
                p += task.Completed ? 1 : 0;
            }

            return Tasks.Count == 0 ? 0 : (p * 100) / Tasks.Count;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Completes all current tasks.
        /// </summary>
        private async Task CompleteAllTasks()
        {
            await Task.Run(() =>
            {
                int temp = Progress;
                foreach (var task in this.Tasks)
                {
                    if (!task.Completed)
                    {
                        task.Completed = true;
                    }
                }

                // For smoother animation
                LastProgress = temp;
                Progress = 100;

                _dialogService.SetMessage("All tasks completed!");
            });
        }

        /// <summary>
        /// Resets all current tasks' complete status.
        /// </summary>
        /// <returns></returns>
        private async Task UncompleteAllTasks()
        {
            await Task.Run(() =>
            {
                int temp = Progress;

                foreach (var task in this.Tasks)
                {
                    if (task.Completed)
                    {
                        task.Completed = false;
                    }
                }

                // For smoother animation
                LastProgress = temp;
                Progress = 0;
            });
        }

        /// <summary>
        /// Saves the new task to the collection.
        /// </summary>
        private void AddTask()
        {
            // Create new object
            TaskViewModel task = new TaskViewModel(_dialogService)
            {
                Name = NewTask.Name,
                Subtasks = new ObservableCollection<Subtask>()
            };

            // Add new task
            Tasks.Add(task);

            // Set as the selected task
            SelectedTask = task;

            // Clear new task
            NewTask = new TaskViewModel(_dialogService);

            // Update UI
            UpdateProgress();

            // Set message
            _dialogService.SetMessage("Task added.");
        }

        /// <summary>
        /// Deletes the specified task.
        /// </summary>
        private void DeleteTask()
        {
            if (SelectedTask == null)
                return;

            Tasks.Remove(SelectedTask);

            // Update UI
            UpdateProgress();

            // Set message
            _dialogService.SetMessage("Task deleted.");
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
            TodoTask sourceItem = dropInfo.Data as TodoTask;
            TodoTask targetItem = dropInfo.TargetItem as TodoTask;

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
            TaskViewModel sourceItem = dropInfo.Data as TaskViewModel;
            TaskViewModel targetItem = dropInfo.TargetItem as TaskViewModel;

            // Let the tasks switch places
            Tasks.Move(Tasks.IndexOf(sourceItem), Tasks.IndexOf(targetItem));
        }

        #endregion
    }
}
