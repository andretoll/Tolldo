using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Tolldo.Data;
using Tolldo.Helpers;
using Tolldo.Models;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The view model for the application. Based on the <see cref="BaseViewModel"/> and <see cref="IDropTarget"/>.
    /// </summary>
    public class MainViewModel : BaseViewModel, IDropTarget
    {
        #region Private Members

        // Theme manager
        private ThemeManager _themeManager;

        // Data repository
        private TodoRepository _repo;

        // If side menu is open or closed
        private bool _isMenuOpen = true;

        // Popup menu
        private bool _isPopupMenuOpen = false;

        // Search mode active
        private bool _searchMode;
        private string _searchString;        

        // Todos
        private ObservableCollection<Todo> _todos;
        private ObservableCollection<Todo> _todosConstant;

        // Selected todo
        private Todo _selectedTodo;

        // New todo
        private string _newTodo;

        // Drag & drop
        private bool _dragHandleActive;

        #endregion

        #region Public Properties

        // If side menu is open or closed
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

        // If popup menu is open or closed
        public bool IsPopupMenuOpen
        {
            get { return _isPopupMenuOpen; }
            set
            {
                _isPopupMenuOpen = value;
                NotifyPropertyChanged();
            }
        }

        // Search mode active
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

        // Todos
        public ObservableCollection<Todo> Todos
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
        public Todo SelectedTodo
        {
            get
            {
                return _selectedTodo;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                // Unsubscribe from PropertyChanged for each task from previous todo
                if (_selectedTodo != null)
                {
                    foreach (var task in _selectedTodo.Tasks)
                    {
                        task.PropertyChanged -= Task_PropertyChanged;
                    }

                    _selectedTodo = null;
                }                

                _selectedTodo = value;
                NotifyPropertyChanged();

                // Subscribe to PropertyChanged for each task
                foreach (var task in _selectedTodo.Tasks)
                {
                    task.PropertyChanged += Task_PropertyChanged;
                }

                // Set initial progress
                SelectedTodo.Progress = 0;
                SelectedTodo.LastProgress = 0;
                SelectedTodo.UpdateProgress();
            }
        }        

        // New todo
        public string NewTodo
        {
            get
            {
                return _newTodo;
            }
            set
            {
                _newTodo = value;
                NotifyPropertyChanged();
            }
        }

        // Drag & drop
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
        /// Event that fires when a child property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update progress
            SelectedTodo.UpdateProgress();
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

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainViewModel()
        {
            // Initialize theme manager
            _themeManager = new ThemeManager();

            // Initialize repository
            _repo = new TodoRepository();

            // Get data
            Todos = new ObservableCollection<Todo>(_repo.GetTodos());
            _todosConstant = new ObservableCollection<Todo>(Todos);

            // Initialize commands
            CollapseMenuCommand = new RelayCommand.RelayCommand(p => { IsMenuOpen = !IsMenuOpen; });
            ToggleSearchModeCommand = new RelayCommand.RelayCommand(p => { SearchMode = !SearchMode; });
            ClearSearchStringCommand = new RelayCommand.RelayCommand(p => { SearchString = ""; SearchMode = !SearchMode; SearchMode = !SearchMode; });
            ActivateDragCommand = new RelayCommand.RelayCommand(p => { DragHandleActive = true; });
            AddNewTodoCommand = new RelayCommand.RelayCommand(p => { AddTodo(NewTodo); }, p => !string.IsNullOrEmpty(NewTodo));
            InvertThemeCommand = new RelayCommand.RelayCommand(p => { _themeManager.SetTheme(!_themeManager.DarkThemeEnabled); });
            TogglePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = !IsPopupMenuOpen; });
            ClosePopupMenuCommand = new RelayCommand.RelayCommand(p => { IsPopupMenuOpen = false; });
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Filter To-Do's with keyword.
        /// </summary>
        private void FilterTodos(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return;
            }

            ObservableCollection<Todo> todosTemp = new ObservableCollection<Todo>(_todosConstant);

            foreach (var todo in todosTemp)
            {
                if (!todo.Name.ToLower().Contains(keyword.ToLower()))
                {
                    Todos.Remove(todo);
                }
            }
        }

        /// <summary>
        /// Clear filter from To-Do's.
        /// </summary>
        private void ClearFilter()
        {
            Todos = new ObservableCollection<Todo>(_todosConstant);
        }

        /// <summary>
        /// Add new todo item
        /// </summary>
        /// <param name="name"></param>
        private void AddTodo(string name)
        {
            // Create new object
            Todo todo = new Todo()
            {
                Name = name,
                Tasks = new ObservableCollection<TodoTask>()
            };

            // Add to collections
            Todos.Add(todo);
            _todosConstant.Add(todo);

            // Set as selected
            SelectedTodo = todo;

            // Clear new todo
            NewTodo = string.Empty;
        }

        #endregion

        #region DragDrop Interface      

        /// <summary>
        /// When dragging item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // If search mode is active, disable dragging
            if (SearchMode || !_dragHandleActive)
            {
                return;
            }

            Todo sourceItem = dropInfo.Data as Todo;
            Todo targetItem = dropInfo.TargetItem as Todo;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// When dropping item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            Todo sourceItem = dropInfo.Data as Todo;
            Todo targetItem = dropInfo.TargetItem as Todo;

            Todos.Move(Todos.IndexOf(sourceItem), Todos.IndexOf(targetItem));
            _todosConstant = new ObservableCollection<Todo>(Todos);

            DragHandleActive = false;
        }

        #endregion
    }
}
