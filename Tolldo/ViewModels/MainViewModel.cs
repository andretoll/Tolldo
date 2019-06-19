using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Tolldo.Data;
using Tolldo.Helpers;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The main ViewModel for the application. Based on the <see cref="BaseViewModel"/> and implements the <see cref="IDropTarget"/> interface.
    /// </summary>
    public class MainViewModel : BaseViewModel, IDropTarget
    {
        #region Private Members

        #region Services and Managers

        // Theme manager
        private ThemeManager _themeManager;

        // Dialog service
        private readonly IDialogService _dialogService;

        // Data repository
        private ITodoRepository _repo;

        // Media player
        private MediaPlayer _mediaPlayer;
        private readonly string _checkSound = SettingsManager.GetCheckSound();

        #endregion

        #region User Interface

        // Indicates if side menu is open or closed
        private bool _isMenuOpen = true;
        // Indicates if settings menu is open or closed
        private bool _isSettingsMenuOpen = false;
        // Indicates if settings have been touched and thus requires saving
        private bool _settingsTouched = false;
        // Indicates if popup menu is open or closed
        private bool _isPopupMenuOpen = false;
        // Indicates if accents menu is open or closed
        private bool _isAccentsMenuOpen = false;
        // Indicates if search mode is active
        private bool _searchMode = false;
        // Search string
        private string _searchString;
        // Message
        private string _message;
        // Indicates if drag is active
        private bool _dragHandleActive;

        #endregion

        #region Objects

        // Filtered todos
        private ListCollectionView _filteredTodos;
        // Selected todo
        private TodoViewModel _selectedTodo;
        // Previously deleted todo
        private TodoViewModel _deletedTodo;
        // New todo name
        private string _newTodoName;

        #endregion

        #endregion

        #region Public Properties

        #region User Interface

        // Indicates if side menu is open or closed
        public bool IsMenuOpen
        {
            get
            {
                return _isMenuOpen;
            }
            set
            {
                _isMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if settings menu is open or closed
        public bool IsSettingsMenuOpen
        {
            get
            {
                return _isSettingsMenuOpen;
            }
            set
            {
                _isSettingsMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if settings have been touched and thus requires saving
        public bool SettingsTouched
        {
            get
            {
                return _settingsTouched;
            }
            set
            {
                _settingsTouched = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if popup menu is open or closed
        public bool IsPopupMenuOpen
        {
            get { return _isPopupMenuOpen; }
            set
            {
                _isPopupMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if accents menu is open or closed
        public bool IsAccentsMenuOpen
        {
            get { return _isAccentsMenuOpen; }
            set
            {
                _isAccentsMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // Indicates if dark theme is enabled
        public bool DarkThemeEnabled
        {
            get { return _themeManager.DarkThemeEnabled; }
            set
            {
                _themeManager.SetTheme(!_themeManager.DarkThemeEnabled);
                SettingsManager.WriteToSetting(SettingsManager.Setting.DarkTheme.ToString(), value);
                SettingsManager.SaveAllSettings();
                NotifyPropertyChanged();
            }
        }

        // Indicates if search mode is active
        public bool SearchMode
        {
            get
            {
                return _searchMode;
            }
            set
            {
                _searchMode = value;
                NotifyPropertyChanged();

                if (value == false)
                    SearchString = "";
            }
        }

        // Search string
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                NotifyPropertyChanged();

                if (value.Length > 0)
                {
                    FilterTodos(value);
                }
                else
                {
                    ClearFilter();
                }
            }
        }

        // The message currently being displayed
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyPropertyChanged();
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

        #endregion

        #region Objects

        // Todos
        public ObservableCollection<TodoViewModel> Todos { get; set; }

        // Filtered todos
        public ListCollectionView FilteredTodos
        {
            get
            {
                return _filteredTodos;
            }
            set
            {
                _filteredTodos = value;
                NotifyPropertyChanged();
            }
        }

        // Selected todo
        public TodoViewModel SelectedTodo
        {
            get
            {
                return _selectedTodo;
            }
            set
            {
                // If previous selected todo is not null, unsubscribe from events and clear properties
                if (_selectedTodo != null)
                {
                    // Unsubscribe from PropertyChanged for each task from previous todo
                    foreach (var task in _selectedTodo.Tasks)
                    {
                        task.PropertyChanged -= Task_PropertyChanged;

                        // If subtasks exist
                        if (task.Subtasks != null)
                        {
                            // Unsubscribe from PropertyChanged for each subtask of each task
                            foreach (var subtask in task.Subtasks)
                            {
                                subtask.PropertyChanged -= task.SubtaskViewModel_PropertyChanged;
                            }
                        }
                    }

                    // Unsubscribe from CollectionChanged for tasks
                    _selectedTodo.Tasks.CollectionChanged -= Tasks_CollectionChanged;

                    // Reset active modes
                    _selectedTodo.RenameActive = false;

                    // Set selected todo to null
                    _selectedTodo = null;
                }

                _selectedTodo = value;
                NotifyPropertyChanged();

                // If no todo is selected, return
                if (value == null)
                {
                    return;
                }

                // Save last selected todo
                SettingsManager.WriteToSetting(SettingsManager.Setting.LastTodo.ToString(), _selectedTodo.Id);
                SettingsManager.SaveAllSettings();

                // Subscribe to PropertyChanged for each task
                foreach (var task in _selectedTodo.Tasks)
                {
                    // Subscribe to property changed for each task
                    task.PropertyChanged += Task_PropertyChanged;

                    // If subtasks exist
                    if (task.Subtasks != null)
                    {
                        // Subscribe to property changed for each subtask of each task
                        foreach (var subtask in task.Subtasks)
                        {
                            subtask.PropertyChanged += task.SubtaskViewModel_PropertyChanged;
                        }
                    }
                }

                // Subscribe to CollectionChagned for tasks
                _selectedTodo.Tasks.CollectionChanged += Tasks_CollectionChanged;

                // Set initial progress
                SelectedTodo.Progress = 0;
                SelectedTodo.LastProgress = 0;
                SelectedTodo.UpdateProgress();
            }
        }

        // Previously deleted todo
        public TodoViewModel DeletedTodo
        {
            get
            {
                return _deletedTodo;
            }
            set
            {
                _deletedTodo = value;
                NotifyPropertyChanged();
            }
        }

        // New todo string
        public string NewTodoName
        {
            get
            {
                return _newTodoName;
            }
            set
            {
                _newTodoName = value;
                NotifyPropertyChanged();
            }
        }    

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Event that fires when message changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageChanged(object sender, EventArgs e)
        {
            // Notify property changed
            if (sender != null)
            {
                DeletedTodo = null;
                Message = null;
                Message = sender.ToString();
            }
            else
            {
                Message = null;
            }
        }

        /// <summary>
        /// Event that fires when a task's property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update progress if the completed property changed
            if (e.PropertyName == "IsCompleted")
            {
                // Play sound if setting is enabled
                if ((sender as TaskViewModel).IsCompleted && Properties.Settings.Default.CheckSound)
                {
                    Uri sound = new Uri(_checkSound);

                    try
                    {
                        _mediaPlayer.Open(sound);
                        _mediaPlayer.Play();
                    }
                    catch (Exception)
                    {
                    }
                }

                SelectedTodo.UpdateProgress();

                // Show message if all tasks are completed
                if (SelectedTodo.Progress == 100)
                {
                    _dialogService.SetMessage("All tasks completed! Well done!");
                }

                // If setting is enabled, place completed task at the top of tasks
                if ((bool)SettingsManager.LoadSetting(SettingsManager.Setting.CompletedTasksOnTop.ToString()) & (sender as TaskViewModel).IsCompleted)
                {
                    try
                    {
                        SelectedTodo.Tasks.Move(SelectedTodo.Tasks.IndexOf((sender as TaskViewModel)), 0);
                        _ = SelectedTodo.UpdateTaskOrder();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
            // If a task was expanded, collapse others
            if (e.PropertyName == "ExpandedActive" && (sender as TaskViewModel).ExpandedActive)
            {
                foreach (var task in SelectedTodo.Tasks)
                {
                    if (task.Id == (sender as TaskViewModel).Id)
                        continue;

                    if (task.ExpandedActive)
                        task.ExpandedActive = false;
                }
            }
        }

        /// <summary>
        /// Event that fires when the Tasks collection changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If an item was added, subscribe to its property changes
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                (e.NewItems[0] as TaskViewModel).PropertyChanged += Task_PropertyChanged;
            }
        }

        /// <summary>
        /// Event that fires when a property's value changes in settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SettingsTouched = true;
        }

        /// <summary>
        /// Event that fires before settings are saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Saving(object sender, CancelEventArgs e)
        {
            SettingsTouched = false;
        }

        #endregion

        #region Commands

        public ICommand NavigateHomeCommand { get; set; }
        public ICommand CollapseMenuCommand { get; set; }
        public ICommand ToggleSearchModeCommand { get; set; }
        public ICommand ClearSearchStringCommand { get; set; }
        public ICommand ActivateDragCommand { get; set; }
        public ICommand AddNewTodoCommand { get; set; }
        public ICommand AddRandomTodoCommand { get; set; }        
        public ICommand TogglePopupMenuCommand { get; set; }
        public ICommand ClosePopupMenuCommand { get; set; }
        public ICommand CloseMessageBoxCommand { get; set; }
        public ICommand InvertThemeCommand { get; set; }
        public ICommand ToggleAccentsMenuCommand { get; set; }
        public ICommand SetAccentCommand { get; set; }
        public ICommand DeleteTodoCommand { get; set; }
        public ICommand UndeleteTodoCommand { get; set; }
        public ICommand SaveSettingsCommand { get; set; }
        public ICommand ReloadSettingsCommand { get; set; }
        public ICommand ChangeAppImageCommand { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that accepts the <see cref="IDialogService"/> interface.
        /// </summary>
        public MainViewModel(IDialogService dialogService)
        {
            // Initialize dialog service
            _dialogService = dialogService;
            _dialogService.MessageChanged += MessageChanged;

            // Initialize theme manager
            _themeManager = new ThemeManager();            

            // Initialize first data repository
            _repo = new TodoRepository(_dialogService);

            // Initialize media player
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Volume = 0.5;
            _mediaPlayer.IsMuted = false;

            // Get data and create filtered view
            Todos = new ObservableCollection<TodoViewModel>(_repo.GetTodos());
            foreach (var todo in Todos)
            {
                todo.UpdateProgress();
            }
            _filteredTodos = new ListCollectionView(Todos);

            // Check last opened list
            int lastOpened = (int)SettingsManager.LoadSetting(SettingsManager.Setting.LastTodo.ToString());
            SelectedTodo = Todos.Where(t => t.Id == lastOpened).FirstOrDefault();

            // Subscribe to settings events
            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
            Properties.Settings.Default.SettingsSaving += Settings_Saving;

            // Initialize commands
            NavigateHomeCommand = new RelayCommand.RelayCommand(p => { SelectedTodo = null; });
            CollapseMenuCommand = new RelayCommand.RelayCommand(p => { IsMenuOpen = !IsMenuOpen; });
            ToggleSearchModeCommand = new RelayCommand.RelayCommand(p => { SearchMode = !SearchMode; });
            ClearSearchStringCommand = new RelayCommand.RelayCommand(p => 
            {
                SearchString = ""; SearchMode = !SearchMode;
                SearchMode = !SearchMode;
            });
            ActivateDragCommand = new RelayCommand.RelayCommand(p => { DragHandleActive = true; });
            TogglePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = !IsPopupMenuOpen; });
            ClosePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = false; });
            CloseMessageBoxCommand = new RelayCommand.RelayCommand(p => { SetMessage(); });
            ToggleAccentsMenuCommand = new RelayCommand.RelayCommand(p => { IsAccentsMenuOpen = !IsAccentsMenuOpen; });
            InvertThemeCommand = new RelayCommand.RelayCommand(p => 
            {
                DarkThemeEnabled = !DarkThemeEnabled;
                SetMessage(DarkThemeEnabled ? "Dark theme enabled." : "Light theme enabled.");
            });
            SetAccentCommand = new RelayCommand.RelayCommand(p => {
                _themeManager.SetAccent((string)p);
                SettingsManager.WriteToSetting(SettingsManager.Setting.Accent.ToString(), p);
                SettingsManager.SaveAllSettings(); SetMessage("Accent applied.");
            });
            AddNewTodoCommand = new RelayCommand.RelayCommand(async p => 
            {
                await AddTodo(NewTodoName);
                SetMessage("List added.");
            }, p => !string.IsNullOrEmpty(NewTodoName));
            AddRandomTodoCommand = new RelayCommand.RelayCommand(async p => { await AddRandomTodo(); SetMessage("To-do added."); });
            DeleteTodoCommand = new RelayCommand.RelayCommand(async p => { await DeleteTodo(SelectedTodo); });
            UndeleteTodoCommand = new RelayCommand.RelayCommand(async p => { await UndeleteTodo(); });
            SaveSettingsCommand = new RelayCommand.RelayCommand(p => { SaveSettings(); }, p => SettingsTouched);
            ReloadSettingsCommand = new RelayCommand.RelayCommand(p => { ReloadSettings(); });
            ChangeAppImageCommand = new RelayCommand.RelayCommand(p => 
            {
                SettingsManager.WriteToSetting(SettingsManager.Setting.AppImage.ToString(), (string)p);
            });

            // Set welcome message
            SetMessage(SettingsManager.LoadSetting(SettingsManager.Setting.WelcomeMessage.ToString()).ToString());                        
        }        

        #endregion

        #region Private Helpers

        /// <summary>
        /// Applies a filter for the collection of Todo-items with the specified keyword.
        /// </summary>
        private void FilterTodos(string keyword)
        {           
            // If keyword is null, empty or whitespace, clear filter and return
            if (string.IsNullOrWhiteSpace(keyword))
            {
                FilteredTodos.Filter = null;
                return;
            }

            // Apply filter and keep selected item
            FilteredTodos.Filter = new Predicate<object>(o => ((TodoViewModel)o).Name.ToLower().Contains(keyword.ToLower()));
        }

        /// <summary>
        /// Clears the current filter
        /// </summary>
        private void ClearFilter()
        {
            FilteredTodos.Filter = null;
        }

        /// <summary>
        /// Adds a new Todo-item
        /// </summary>
        /// <param name="name">Name of the new Todo-item.</param>
        private async Task AddTodo(string name)
        {
            // Create new todo with name, image url and empty list of tasks
            TodoViewModel todo = new TodoViewModel(_dialogService)
            {
                Name = name,
                Order = Todos.Count + 1,
                Tasks = new ObservableCollection<TaskViewModel>()
            };

            // Add item to database
            await _repo.AddTodo(todo).ContinueWith(p =>
            {
                // Get Id of added item
                todo.Id = p.Result;                
            });

            // Add item to collection
            Todos.Add(todo);

            // Set as the selected Todo-item
            SelectedTodo = todo;

            // Clear new todo
            NewTodoName = string.Empty;            
        }

        /// <summary>
        /// Adds a random Todo-item
        /// </summary>
        private async Task AddRandomTodo()
        {
            // Add todo with random name
            await AddTodo(RandomNameGenerator.GetRandomName());

            // Enter rename mode
            SelectedTodo.RenameActive = true;
        }

        /// <summary>
        /// Deletes a Todo-item from the collection.
        /// </summary>
        /// <param name="todo">The Todo-item to be deleted.</param>
        private async Task DeleteTodo(TodoViewModel todo)
        {
            if (todo == null)
                return;            

            // Delete item from database
            var success = await _repo.DeleteTodo(todo);

            // If failed, set message and return
            if (!success)
            {
                SetMessage("Deletion failed. Try again later.");
                return;                
            }

            // Reset selected item
            SelectedTodo = null;

            // Remove item from view
            Todos.Remove(todo);

            // Set message
            SetMessage("List deleted.");

            // Set as deleted todo
            DeletedTodo = todo;

            // Update todo order
            await UpdateTodoOrder();
        }

        /// <summary>
        /// Undeletes the last known deleted Todo-item
        /// </summary>
        private async Task UndeleteTodo()
        {
            if (DeletedTodo == null)
            {
                return;
            }           

            // Re-add item to database
            await _repo.AddTodo(DeletedTodo).ContinueWith(p =>
            {
                // Get Id of added item
                DeletedTodo.Id = p.Result;
            });

            // Re-add item to collection
            Todos.Add(DeletedTodo);

            // Select
            SelectedTodo = DeletedTodo;

            // Clear last deleted
            DeletedTodo = null;

            // Unset message
            SetMessage(null);

            // Update todo order
            await UpdateTodoOrder();
        }

        /// <summary>
        /// Updates the current todo order.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateTodoOrder()
        {
            // Update UI
            int order = 1;
            foreach (var todo in this.Todos)
            {
                todo.Order = order;
                order++;
            }

            // Change order property and update database
            await Task.Run(async () =>
            {
                foreach (var todo in this.Todos)
                {
                    await _repo.UpdateTodo(todo);
                }
            });
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

        /// <summary>
        /// Saves the current settings and closes the menu.
        /// </summary>
        private void SaveSettings()
        {
            SettingsManager.SaveAllSettings();
            IsSettingsMenuOpen = false;

            SetMessage("Settings saved!");
        }

        /// <summary>
        /// Reloads the previous settings without making any changes.
        /// </summary>
        private void ReloadSettings()
        {
            SettingsManager.ReloadSettings();
            IsSettingsMenuOpen = false;
            SettingsTouched = false;
        }

        #endregion

        #region DragDrop Interface      

        /// <summary>
        /// Method called when dragging an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // If search mode is active, disable dragging
            if (SearchMode || !DragHandleActive)
            {
                return;
            }

            // Get source and target
            TodoViewModel sourceItem = dropInfo.Data as TodoViewModel;
            TodoViewModel targetItem = dropInfo.TargetItem as TodoViewModel;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Method called when dropping an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        async void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Get source and target
            TodoViewModel sourceItem = dropInfo.Data as TodoViewModel;
            TodoViewModel targetItem = dropInfo.TargetItem as TodoViewModel;

            // Let the Todo-items switch places
            Todos.Move(Todos.IndexOf(sourceItem), Todos.IndexOf(targetItem));

            // Update todo order
            await UpdateTodoOrder();

            DragHandleActive = false;
        }

        #endregion
    }
}
