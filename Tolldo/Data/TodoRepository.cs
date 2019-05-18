using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tolldo.Models;

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
        private List<Todo> _todos;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TodoRepository()
        {
            _todos = new List<Todo>();
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="Todo"/> items.
        /// </summary>
        /// <returns></returns>
        public List<Todo> GetTodos()
        {
            // TEMPORARY DATA

            if (_todos == null)
                _todos = new List<Todo>();

            _todos.Add(new Todo()
            {
                Id = 1,
                Name = "Thesis",
                Description = "We must complete the thesis!",

                Tasks = new ObservableCollection<TodoTask>()
                {
                    new TodoTask()
                    {
                        Id = 1,
                        Name = "Write introduction",
                        Completed = true
                    },

                    new TodoTask()
                    {
                        Id = 2,
                        Name = "Write Theoretical background",
                        Completed = true
                    },

                    new TodoTask()
                    {
                        Id = 3,
                        Name = "Write Methodology",
                        Completed = true
                    },

                    new TodoTask()
                    {
                        Id = 4,
                        Name = "Cry",
                        Completed = true
                    }
                }
            });

            _todos.Add(new Todo()
            {
                Id = 2,
                Name = "App To-Do",
                Description = "Product backlog.",

                Tasks = new ObservableCollection<TodoTask>()
                {
                    new TodoTask()
                    {
                        Id = 1,
                        Name = "Dynamic colors",
                        Completed = true
                    },

                    new TodoTask()
                    {
                        Id = 2,
                        Name = "Sorting functionality"
                    },

                    new TodoTask()
                    {
                        Id = 3,
                        Name = "Optimization"
                    }
                }
            });

            _todos.Add(new Todo()
            {
                Id = 3,
                Name = "Before moving",
                Description = "The things I have to do before moving.",

                Tasks = new ObservableCollection<TodoTask>()
            });

            // END: TEMORARY DATA

            return _todos;
        }
    }
}
