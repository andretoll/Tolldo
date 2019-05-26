using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tolldo.Models;
using Tolldo.Services;
using Tolldo.ViewModels;

namespace Tolldo.Data
{
    /// <summary>
    /// Data provider that implements the <see cref="ITodoRepository"/> interface.
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        #region Private Members

        /// <summary>
        /// A list of Todo-items.
        /// </summary>
        private List<TodoViewModel> _todos;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TodoRepository()
        {
            _todos = new List<TodoViewModel>();
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="TodoViewModel"/> items.
        /// </summary>
        /// <param name="dialogService">The dialog service to broadcast messages.</param>
        /// <returns></returns>
        public List<TodoViewModel> GetTodos(IDialogService dialogService)
        {
            // TEMPORARY DATA
            if (_todos == null)
                _todos = new List<TodoViewModel>();

            _todos.Add(new TodoViewModel(dialogService)
            {
                Id = 1,
                Name = "Thesis",
                Description = "We must complete the thesis!",

                Tasks = new ObservableCollection<TaskViewModel>()
                {
                    new TaskViewModel(dialogService)
                    {
                        Id = 1,
                        Name = "Write introduction",
                        Description = "Obviously we have to start with this part...",
                        Completed = true,
                        Subtasks = new ObservableCollection<Subtask>()
                        {
                            new Subtask()
                            {
                                Id = 1,
                                Name = "Think about the first words",
                                Completed = true
                            },
                            new Subtask()
                            {
                                Id = 2,
                                Name = "Write down the references",
                                Completed = true
                            }
                        }
                    },

                    new TaskViewModel(dialogService)
                    {
                        Id = 2,
                        Name = "Write Theoretical background",
                        Completed = true,
                        Subtasks = new ObservableCollection<Subtask>()
                        {
                            new Subtask()
                            {
                                Id = 1,
                                Name = "Think about the first words",
                                Completed = true
                            },
                            new Subtask()
                            {
                                Id = 2,
                                Name = "Write down the references",
                                Completed = true
                            }
                        }
                    },

                    new TaskViewModel(dialogService)
                    {
                        Id = 3,
                        Name = "Write Methodology",
                        Completed = true,
                        Subtasks = new ObservableCollection<Subtask>()
                        {
                            new Subtask()
                            {
                                Id = 1,
                                Name = "Think about the first words",
                                Completed = true
                            },
                            new Subtask()
                            {
                                Id = 2,
                                Name = "Write down the references",
                                Completed = true
                            }
                        }
                    },

                    new TaskViewModel(dialogService)
                    {
                        Id = 4,
                        Name = "Cry",
                        Completed = true,
                        Subtasks = new ObservableCollection<Subtask>()
                        {
                            new Subtask()
                            {
                                Id = 1,
                                Name = "Think about the first words",
                                Completed = true
                            },
                            new Subtask()
                            {
                                Id = 2,
                                Name = "Write down the references",
                                Completed = true
                            }
                        }
                    }
                }
            }) ;

            // END: TEMORARY DATA

            return _todos;
        }
    } 

    #endregion
}
