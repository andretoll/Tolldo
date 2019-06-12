using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Tolldo.Data;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Task-object. Based on the <see cref="BaseViewModel"/> class.
    /// </summary>
    public class TaskViewModel : BaseViewModel
    {
        #region Private Members

        #region Services

        // Dialog service
        private IDialogService _dialogService;
        // Data repository
        private ITodoRepository _repo;

        #endregion

        #region Model

        private string _name;

        private string _description;

        private bool _completed;

        private bool _important;

        private int _order;

        private ObservableCollection<SubtaskViewModel> _subtasks;

        #endregion

        #region User Interface

        // Indicates if renaming task mode is active
        private bool _renameActive;
        private string _renameValue;

        // Indicates if expanded mode is active
        private bool _expandedActive;

        // New subtask name
        private string _newSubtaskName;

        // Indicates if a subtask has been added
        private bool _newSubtaskAdded;

        // Indicates if app is busy
        private bool _isBusy;

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

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

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

        public bool IsCompleted
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;

                // Evaluate the completion status of subtasks
                Task.Run(async () =>
                {
                    await CompleteSubtasks();
                });

                NotifyPropertyChanged();
            }
        }
               
        public bool Important
        {
            get
            {
                return _important;
            }
            set
            {
                _important = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsImportant
        {
            get
            {
                return _important;
            }
            set
            {
                _important = value;
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

        public ObservableCollection<SubtaskViewModel> Subtasks
        {
            get
            {
                return _subtasks;
            }
            set
            {
                _subtasks = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region User Interface

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

                // If true, save name
                if (value)
                    _renameValue = this.Name;
                else
                    _renameValue = null;
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

                // If false, save expanded properties
                if (!value)
                {
                    _repo.UpdateTask(this);
                }
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

        // Indicates if a subtask has been added
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

        // Indicates if app is busy
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand ToggleRenameCommand { get; set; }
        public ICommand ToggleExpandedCommand { get; set; }
        public ICommand CheckTaskCommand { get; set; }
        public ICommand UncheckTaskCommand { get; set; }
        public ICommand AddSubtaskCommand { get; set; }
        public ICommand DeleteSubtaskCommand { get; set; }
        public ICommand MarkAsImportantCommand { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event that fires when a subtask's property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Subtask_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Determine task completion if the completed property of any subtask changed
            if (e.PropertyName == "Completed")
            {
                // Indicates if all subtasks are completed
                bool subtasksComplete = true;

                foreach (var subtask in Subtasks)
                {
                    // If subtask is uncomplted
                    if (!subtask.Completed)
                    {
                        // If any subtask is incompleted, set false
                        subtasksComplete = false;
                        break;
                    }
                }

                // If global completion has changed, set completed status indirectly to avoid infinite loop
                if (_completed != subtasksComplete)
                {
                    _completed = subtasksComplete;
                    NotifyPropertyChanged(nameof(IsCompleted));
                }

                // Update subtask in database
                await UpdateSubtask(sender as SubtaskViewModel);
            }
            else if (e.PropertyName == "Name")
            {
                // Update subtask in database
                var success = await _repo.UpdateSubtask(sender as SubtaskViewModel);
                if (!success)
                    SetMessage("Something went wrong. Try again.");
            }
        }

        /// <summary>
        /// Event that fires when a property in <see cref="TaskViewModel"/> changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TaskViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Evaluate calling property
            switch (e.PropertyName)
            {
                // Name
                case nameof(RenameActive):

                    // If rename has been made and value has changed, update item
                    if (!RenameActive & _renameValue != null & this.Name != _renameValue)
                    {
                        var success = await _repo.UpdateTask(this);
                        if (success)
                            SetMessage("Task updated.");
                        else SetMessage("Something went wrong. Try again.");
                    }
                    break;

                // Completed
                case nameof(IsCompleted):

                    // Update item in database
                    await UpdateTask(false);
                    break;

                // Important
                case nameof(IsImportant):

                    // Update item in database
                    await UpdateTask(false);
                    break;

                default:
                    break;
            }
        }    

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor. Accepts a dependency property of the type <see cref="IDialogService"/>.
        /// </summary>
        public TaskViewModel(IDialogService dialogService)
        {
            // Initialize services
            _dialogService = dialogService;
            _repo = new TodoRepository(_dialogService);

            this.PropertyChanged += TaskViewModel_PropertyChanged;

            // Commands
            ToggleRenameCommand = new RelayCommand.RelayCommand(p => { RenameActive = !RenameActive; });
            ToggleExpandedCommand = new RelayCommand.RelayCommand(p => { ExpandedActive = !ExpandedActive; });
            CheckTaskCommand = new RelayCommand.RelayCommand(p => { this.IsCompleted = true; });
            UncheckTaskCommand = new RelayCommand.RelayCommand(p => { this.IsCompleted = false; });
            AddSubtaskCommand = new RelayCommand.RelayCommand(async p => { await AddSubtask(); SetMessage("Subtask added."); }, p => NewSubtaskName.Length > 0);
            DeleteSubtaskCommand = new RelayCommand.RelayCommand(async p => { await DeleteSubtask(p as SubtaskViewModel); });
            MarkAsImportantCommand = new RelayCommand.RelayCommand(p => { this.IsImportant = !this.IsImportant; });
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Adds a new subtask to the collection.
        /// </summary>
        private async Task AddSubtask()
        {
            // New Subtask
            SubtaskViewModel subtask = new SubtaskViewModel()
            {
                Name = this.NewSubtaskName
            };

            // Subscribe to property changed event for new subtask
            subtask.PropertyChanged += Subtask_PropertyChanged;

            // Add subtask to database
            await _repo.AddSubtask(subtask, this.Id).ContinueWith(async p =>
            {
                subtask.Id = await p;
                subtask.TodoTaskId = this.Id;
            });

            // Add item to view
            Subtasks.Add(subtask);

            subtask.Completed = false;

            // Reset new subtask
            NewSubtaskName = null;
            NewSubtaskAdded = true;
            NewSubtaskAdded = false;
        }

        /// <summary>
        /// Completes or incompleted all subtasks based on its parent task.
        /// </summary>
        /// <returns></returns>
        private async Task CompleteSubtasks()
        {
            if (Subtasks != null)
            {
                await Task.Run(() =>
                {
                    bool b = Completed;
                    foreach (var subtask in Subtasks)
                    {
                        subtask.Completed = b;
                    }
                });
            }       
        }

        /// <summary>
        /// Deletes the specified subtask from the collection.
        /// </summary>
        /// <param name="subtask">The subtask to delete.</param>
        private async Task DeleteSubtask(SubtaskViewModel subtask)
        {
            if (subtask == null)
                return;

            // Unsubscribe from property changed event
            subtask.PropertyChanged -= Subtask_PropertyChanged;

            // Delete item from database
            var success = await _repo.DeleteSubtask(subtask);

            // If failed, set message and return
            if (!success)
            {
                SetMessage("Deletion failed. Try again later.");
                return;
            }

            // Remove item from view
            Subtasks.Remove(subtask);

            // Set message
            SetMessage("Subtask deleted.");
        }

        /// <summary>
        /// Updates the current task in database.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task UpdateTask(bool msg)
        {
            IsBusy = true;

            var success = await _repo.UpdateTask(this);

            if (!success)
                SetMessage("Something went wrong. Try again.");
            else if (msg)
                SetMessage("Task updated.");

            IsBusy = false;
        }

        /// <summary>
        /// Updates the specified subtask in database.
        /// </summary>
        /// <param name="subtask"></param>
        /// <returns></returns>
        private async Task UpdateSubtask(SubtaskViewModel subtask)
        {
            if (subtask == null)
                return;

            IsBusy = true;

            var success = await _repo.UpdateSubtask(subtask);

            if (!success)
                SetMessage("Something went wrong. Try again.");

            IsBusy = false;
        }

        /// <summary>
        /// Sets a message to be displayed.
        /// </summary>
        /// <param name="msg">The message to be displayed.</param>
        private void SetMessage(string msg = null)
        {
            // Set message
            _dialogService.SetMessage(msg);
        }

        #endregion
    }
}
