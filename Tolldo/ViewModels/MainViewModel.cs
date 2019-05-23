using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tolldo.Data;
using Tolldo.Helpers;
using Tolldo.Models;
using Tolldo.Services;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The main ViewModel for the application. Based on the <see cref="BaseViewModel"/> and implements the <see cref="IDropTarget"/> interface.
    /// </summary>
    public class MainViewModel : BaseViewModel, IDropTarget
    {
        #region Private Members

        // Theme manager
        private ThemeManager _themeManager;

        // Dialog service
        private readonly IDialogService _dialogService;

        // Data repository
        private TodoRepository _repo;

        // Indicates if side menu is open or closed
        private bool _isMenuOpen = true;

        // Indicates if popup menu is open or closed
        private bool _isPopupMenuOpen = false;

        // Indicates if accents menu is open or closed
        private bool _isAccentsMenuOpen = false;

        // Indicates if search mode is active
        private bool _searchMode;

        // Search string
        private string _searchString;

        // Message
        private string _message;

        // Todos
        private ObservableCollection<TodoViewModel> _todos;
        private ObservableCollection<TodoViewModel> _todosConstant;

        // Selected todo
        private TodoViewModel _selectedTodo;

        // Previously deleted todo
        private TodoViewModel _deletedTodo;

        // New todo
        private string _newTodoName;

        // Indicates if drag is active
        private bool _dragHandleActive;

        #endregion

        #region Public Properties

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
                    FilterTodos(value);
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

        // Todos
        public ObservableCollection<TodoViewModel> Todos
        {
            get
            {
                return _todos;
            }
            set
            {
                _todos = value;
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
                if (_selectedTodo != null)
                {
                    // Unsubscribe from PropertyChanged for each task from previous todo
                    foreach (var task in _selectedTodo.Tasks)
                    {
                        task.PropertyChanged -= Task_PropertyChanged;
                    }
                    // Unsubscribe from CollectionChanged for tasks
                    _selectedTodo.Tasks.CollectionChanged -= Tasks_CollectionChanged;

                    // Reset active modes
                    _selectedTodo.RenameActive = false;

                    _selectedTodo = null;                    
                }                

                _selectedTodo = value;
                NotifyPropertyChanged();

                if (value == null)
                {
                    return;
                }

                // Subscribe to PropertyChanged for each task
                foreach (var task in _selectedTodo.Tasks)
                {
                    task.PropertyChanged += Task_PropertyChanged;
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
                UnsetMessage();
                Message = sender.ToString();
            }
        }

        /// <summary>
        /// Event that fires when a child property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update progress if the completed property changed
            if (e.PropertyName == "Completed")
            {
                SelectedTodo.UpdateProgress();

                if (SelectedTodo.Progress == 100)
                {
                    _dialogService.SetMessage("All tasks completed! Well done!");
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
                (e.NewItems[0] as TodoTask).PropertyChanged += Task_PropertyChanged;
            }
        }

        #endregion

        #region Commands

        public ICommand CollapseMenuCommand { get; set; }
        public ICommand ToggleSearchModeCommand { get; set; }
        public ICommand ClearSearchStringCommand { get; set; }
        public ICommand ActivateDragCommand { get; set; }
        public ICommand AddNewTodoCommand { get; set; }
        public ICommand InvertThemeCommand { get; set; }
        public ICommand TogglePopupMenuCommand { get; set; }
        public ICommand ClosePopupMenuCommand { get; set; }
        public ICommand CloseMessageBoxCommand { get; set; }
        public ICommand ToggleAccentsMenuCommand { get; set; }
        public ICommand SetAccentCommand { get; set; }

        public ICommand DeleteTodoCommand { get; set; }
        public ICommand UndeleteTodoCommand { get; set; }

        #endregion                

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainViewModel(IDialogService dialogService)
        {
            // Initialize dialog service
            _dialogService = dialogService;
            _dialogService.MessageChanged += MessageChanged;

            // Initialize theme manager
            _themeManager = new ThemeManager();

            // Initialize data repository
            _repo = new TodoRepository();

            // Get data
            Todos = new ObservableCollection<TodoViewModel>(_repo.GetTodos(_dialogService));
            _todosConstant = new ObservableCollection<TodoViewModel>(Todos);

            // Initialize commands
            CollapseMenuCommand = new RelayCommand.RelayCommand(p => { IsMenuOpen = !IsMenuOpen; });
            ToggleSearchModeCommand = new RelayCommand.RelayCommand(p => { SearchMode = !SearchMode; });
            ClearSearchStringCommand = new RelayCommand.RelayCommand(p => { SearchString = ""; SearchMode = !SearchMode; SearchMode = !SearchMode; });
            ActivateDragCommand = new RelayCommand.RelayCommand(p => { DragHandleActive = true; });
            AddNewTodoCommand = new RelayCommand.RelayCommand(p => { AddTodo(NewTodoName); }, p => !string.IsNullOrEmpty(NewTodoName));
            InvertThemeCommand = new RelayCommand.RelayCommand(p => { _themeManager.SetTheme(!_themeManager.DarkThemeEnabled); });
            TogglePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = !IsPopupMenuOpen; });
            ClosePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = false; });
            CloseMessageBoxCommand = new RelayCommand.RelayCommand(p => { UnsetMessage(); });
            ToggleAccentsMenuCommand = new RelayCommand.RelayCommand(p => { IsAccentsMenuOpen = !IsAccentsMenuOpen; });
            SetAccentCommand = new RelayCommand.RelayCommand(p => { _themeManager.SetAccent((string)p); });

            DeleteTodoCommand = new RelayCommand.RelayCommand(p => { DeleteTodo(SelectedTodo); });
            UndeleteTodoCommand = new RelayCommand.RelayCommand(p => { UndeleteTodo(); });

            // Set welcome message
            SetMessage("Welcome back!");
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Applies a filter for the collection of Todo-items with the specified keyword.
        /// </summary>
        private void FilterTodos(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return;
            }

            // Create new temporary list
            ObservableCollection<TodoViewModel> todosTemp = new ObservableCollection<TodoViewModel>(_todosConstant);

            // Clear the filter
            ClearFilter();

            // For each Todo-item, remove it if the name does NOT contain the keyword
            foreach (var todo in todosTemp)
            {
                if (!todo.Name.ToLower().Contains(keyword.ToLower()))
                {
                    Todos.Remove(todo);
                }
            }
        }

        /// <summary>
        /// Clears the current filter
        /// </summary>
        private void ClearFilter()
        {
            Todos = new ObservableCollection<TodoViewModel>(_todosConstant);
        }

        /// <summary>
        /// Adds a new Todo-item
        /// </summary>
        /// <param name="name">Name of the new Todo-item.</param>
        private void AddTodo(string name)
        {
            // Create new object
            TodoViewModel todo = new TodoViewModel(_dialogService)
            {
                Name = name,
                Tasks = new ObservableCollection<TodoTask>()
            };

            // Add to collections
            Todos.Add(todo);
            _todosConstant.Add(todo);

            // Set as the selected Todo-item
            SelectedTodo = todo;

            // Clear new todo
            NewTodoName = string.Empty;

            // Set message
            SetMessage("List added.");
        }

        /// <summary>
        /// Deletes a Todo-item from the collection.
        /// </summary>
        /// <param name="todo">The Todo-item to be deleted.</param>
        private void DeleteTodo(TodoViewModel todo)
        {
            // Remove item
            Todos.Remove(todo);
            _todosConstant.Remove(todo);

            // Reset selected item
            SelectedTodo = null;

            // Set message
            SetMessage("List deleted.");

            // Set as deleted todo
            DeletedTodo = todo;
        }

        /// <summary>
        /// Undeletes the last known deleted Todo-item
        /// </summary>
        private void UndeleteTodo()
        {
            if (DeletedTodo == null)
            {
                return;
            }

            // Undelete todo-item
            Todos.Add(DeletedTodo);
            _todosConstant.Add(DeletedTodo);

            // Clear last deleted
            DeletedTodo = null;

            // Unset message
            UnsetMessage();
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

        /// <summary>
        /// Unsets the current message.
        /// </summary>
        private void UnsetMessage()
        {
            // Unset message
            Message = null;

            // Unset deleted items
            DeletedTodo = null;
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
            if (SearchMode || !_dragHandleActive)
            {
                return;
            }

            // Get source and target
            Todo sourceItem = dropInfo.Data as Todo;
            Todo targetItem = dropInfo.TargetItem as Todo;

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
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Get source and target
            TodoViewModel sourceItem = dropInfo.Data as TodoViewModel;
            TodoViewModel targetItem = dropInfo.TargetItem as TodoViewModel;

            // Let the Todo-items switch places
            Todos.Move(Todos.IndexOf(sourceItem), Todos.IndexOf(targetItem));
            _todosConstant = new ObservableCollection<TodoViewModel>(Todos);

            DragHandleActive = false;
        }

        #endregion
    }
}
