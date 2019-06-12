using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tolldo.Data;
using Tolldo.Helpers;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Todo-object. Based on the <see cref="BaseViewModel"/> class and implements the <see cref="IDropTarget"/> interface.
    /// </summary>
    public class TodoViewModel : BaseViewModel, IDropTarget
    {
        #region Private Members

        #region Services

        // Dialog service
        private readonly IDialogService _dialogService;
        // Data repository
        private ITodoRepository _repo;

        #endregion

        #region Model

        private string _name;

        private string _imageUrl;

        private int _order;

        private bool _favorite;

        private ObservableCollection<TaskViewModel> _tasks;

        private int _progress;

        private int _lastProgress;

        #endregion

        #region User Interface

        // Indicates if renaming mode is active
        private bool _renameActive;
        private string _renameValue;

        // Indicates if new task is being created
        private bool _newTaskActive;

        // Indicates if drag is active
        private bool _dragHandleActive;

        // Indicates if banner image popup menu is open
        private bool _isImageMenuOpen;

        #endregion

        #region Objects

        // Selected task
        private TaskViewModel _selectedTask;

        // New task
        private TaskViewModel _newTask;

        #endregion

        #endregion

        #region Public Properties

        #region Model

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

        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
                NotifyPropertyChanged();
            }
        }

        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
                NotifyPropertyChanged();
            }
        }

        public bool Favorite
        {
            get
            {
                return _favorite;
            }
            set
            {
                _favorite = value;
                NotifyPropertyChanged();
            }
        }

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

        #region User Interface

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

                // If true, save name
                if (value)
                    _renameValue = this.Name;
                else
                    _renameValue = null;
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

        // Indicates if drag is active
        public bool DragHandleActive
        {
            get
            {
                return _dragHandleActive;
            }
            set
            {
                _dragHandleActive = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if banner image popup menu is open
        public bool IsImageMenuOpen
        {
            get
            {
                return _isImageMenuOpen;
            }
            set
            {
                _isImageMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // List of banner images
        public List<string> BannerImageCollection
        {
            get
            {
                return SettingsManager.GetBannerImageList();
            }
        }

        #endregion

        #region Objects

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

        #endregion

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
                    p += task.IsCompleted ? 1 : 0;
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

        #region Commands

        public ICommand ResetTasksCommand { get; set; }
        public ICommand ToggleRenameCommand { get; set; }
        public ICommand ToggleNewTaskCommand { get; set; }
        public ICommand SaveNewTaskCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand CopyTaskCommand { get; set; }
        public ICommand ActivateDragCommand { get; set; }
        public ICommand CloseImageMenuCommand { get; set; }
        public ICommand ChangeBannerImageCommand { get; set; }
        public ICommand MakeFavoriteCommand { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event that fires when a property in <see cref="TodoViewModel"/> changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TodoViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Evaluate calling property
            switch (e.PropertyName)
            {
                // Update name if value changed
                case nameof(RenameActive):

                    if (!RenameActive & _renameValue != null & this.Name != _renameValue)
                    {
                        var success = await _repo.UpdateTodo(this);
                        if (success)
                            SetMessage("Todo updated.");
                        else SetMessage("Something went wrong. Try again.");
                    }
                    break;

                default:
                    break;
            }
        } 

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TodoViewModel(IDialogService dialogService)
        {
            // Initialize services
            _dialogService = dialogService;
            _repo = new TodoRepository(_dialogService);

            // Subscribe to Todo Property Changed
            this.PropertyChanged += TodoViewModel_PropertyChanged;

            // Initialize commands
            ResetTasksCommand = new RelayCommand.RelayCommand(async p => { await ResetTasks(); }, p => Tasks.Count > 0);
            ToggleRenameCommand = new RelayCommand.RelayCommand(p => { RenameActive = !RenameActive; });
            ToggleNewTaskCommand = new RelayCommand.RelayCommand(p => { NewTaskActive = bool.Parse((string)p); });
            SaveNewTaskCommand = new RelayCommand.RelayCommand(async p => { await AddTask(); SetMessage("Task added."); }, p => (NewTaskActive && !string.IsNullOrEmpty(NewTask.Name)));
            DeleteTaskCommand = new RelayCommand.RelayCommand(async p => { await DeleteTask(SelectedTask); });
            CopyTaskCommand = new RelayCommand.RelayCommand(async p => { await CopyTask(SelectedTask); });
            ActivateDragCommand = new RelayCommand.RelayCommand(p => { DragHandleActive = true; });
            CloseImageMenuCommand = new RelayCommand.RelayCommand(p => { IsImageMenuOpen = false; });
            ChangeBannerImageCommand = new RelayCommand.RelayCommand(async p => { await ChangeBannerImage((string)p); });
            MakeFavoriteCommand = new RelayCommand.RelayCommand(p => { Favorite = !Favorite; });
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
        /// Resets all current tasks' complete status.
        /// </summary>
        /// <returns></returns>
        private async Task ResetTasks()
        {
            await Task.Run(() =>
            {
                int temp = Progress;

                foreach (var task in this.Tasks)
                {
                    if (task.IsCompleted)
                    {
                        task.IsCompleted = false;
                    }
                }
            });
        }

        /// <summary>
        /// Saves the new task to the collection.
        /// </summary>
        private async Task AddTask()
        {
            // Create new object
            TaskViewModel task = new TaskViewModel(_dialogService)
            {
                Name = NewTask.Name,
                Description = "",
                Order = Tasks.Count + 1,
                Subtasks = new ObservableCollection<SubtaskViewModel>()
            };

            // Add item to database
            await _repo.AddTask(task, this.Id).ContinueWith(async p =>
            {
                task.Id = await p;
            });

            // Add item to collection
            Tasks.Add(task);

            // Set as the selected task
            SelectedTask = task;

            // Clear new task
            NewTask = new TaskViewModel(_dialogService);

            // Update UI
            UpdateProgress();
        }

        /// <summary>
        /// Deletes the specified task.
        /// </summary>
        /// <param name="task">The task to delete.</param>
        /// <returns></returns>
        private async Task DeleteTask(TaskViewModel task)
        {
            if (task == null)
                return;            

            // Delete item from database
            var success = await _repo.DeleteTask(task);

            // If failed, set message and return
            if (!success)
            {
                SetMessage("Deletion failed. Try again later.");
                return;
            }

            // Reset selected item
            SelectedTask = null;

            // Remove item from view
            Tasks.Remove(task);

            // Set message
            SetMessage("Task deleted.");

            // Update UI
            UpdateProgress();

            // Update task order
            await UpdateTaskOrder();
        }

        /// <summary>
        /// Copies the specified task.
        /// </summary>
        /// <param name="task">The task to copy.</param>
        /// <returns></returns>
        private async Task CopyTask(TaskViewModel task)
        {
            if (task == null)
                return;

            // Create new object
            TaskViewModel newTask = new TaskViewModel(_dialogService)
            {
                Name = task.Name,
                Description = task.Description,
                Completed = task.Completed,
                Order = task.Order + 1,
                Subtasks = new ObservableCollection<SubtaskViewModel>()
            };

            // Add item to database
            await _repo.AddTask(newTask, this.Id).ContinueWith(async p =>
            {
                newTask.Id = await p;
            });

            // Add item to collection
            Tasks.Insert(Tasks.IndexOf(task) + 1, newTask);

            // Update UI
            UpdateProgress();

            // Update task order
            await UpdateTaskOrder();
        }

        /// <summary>
        /// Changes the current banner image for this todo.
        /// </summary>
        /// <param name="url">The url to the image</param>
        /// <returns></returns>
        private async Task ChangeBannerImage(string url)
        {
            if (url == this.ImageUrl)
            {
                this.ImageUrl = null;
                return;
            }

            // Close menu after update
            this.IsImageMenuOpen = false;

            this.ImageUrl = url;

            var success = await _repo.UpdateTodo(this);

            if (!success)
            {
                SetMessage("Image could not be set. Try again later.");
                return;
            }

            SetMessage("Image updated.");

            if (string.IsNullOrEmpty(this.ImageUrl))
                this.ImageUrl = null;
        }

        /// <summary>
        /// Updates the current task order.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateTaskOrder()
        {
            // Update UI
            int order = 1;
            foreach (var task in this.Tasks)
            {
                task.Order = order;
                order++;
            }

            // Change order property and update database
            await Task.Run(async () =>
            {
                foreach (var task in this.Tasks)
                {
                    await _repo.UpdateTask(task);
                }
            });
        }

        /// <summary>
        /// Sets a message to be displayed.
        /// </summary>
        /// <param name="msg">The message to be displayed.</param>
        private void SetMessage(string msg)
        {
            // Set message
            _dialogService.SetMessage(msg);
        }

        #endregion

        #region DragDrop Interface

        /// <summary>
        /// Method called when dragging an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (!DragHandleActive)
            {
                return;
            }

            // Get source and target
            TaskViewModel sourceItem = dropInfo.Data as TaskViewModel;
            TaskViewModel targetItem = dropInfo.TargetItem as TaskViewModel;

            sourceItem.RenameActive = false;

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
        async void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Get source and target
            TaskViewModel sourceItem = dropInfo.Data as TaskViewModel;
            TaskViewModel targetItem = dropInfo.TargetItem as TaskViewModel;

            // Let the tasks switch places
            Tasks.Move(Tasks.IndexOf(sourceItem), Tasks.IndexOf(targetItem));

            // Update task order
            await UpdateTaskOrder();    

            DragHandleActive = false;
        }

        #endregion
    }
}
