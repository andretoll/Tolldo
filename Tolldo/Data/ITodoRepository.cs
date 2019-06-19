using System.Collections.Generic;
using System.Threading.Tasks;
using Tolldo.ViewModels;

namespace Tolldo.Data
{
    /// <summary>
    /// Data provider interface for Tolldo.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Ensure that the database already exists.
        /// </summary>
        void EnsureDataBase();       

        /// <summary>
        /// Get todo-items from database.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TodoViewModel> GetTodos();

        /// <summary>
        /// Add todo-item to database.
        /// </summary>
        /// <param name="todo">Todo-item to add</param>
        /// <returns>Id of inserted item.</returns>
        Task<int> AddTodo(TodoViewModel todo);

        /// <summary>
        /// Delete todo-item from database.
        /// </summary>
        /// <param name="todo">Todo-item to delete.</param>
        /// <returns></returns>
        Task<bool> DeleteTodo(TodoViewModel todo);

        /// <summary>
        /// Update todo-item in database.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        Task<bool> UpdateTodo(TodoViewModel todo);

        /// <summary>
        /// Add task-item to database.
        /// </summary>
        /// <param name="task">Task to add.</param>
        /// <returns>Id of inserted item.</returns>
        Task<int> AddTask(TaskViewModel task, int id);

        /// <summary>
        /// Delete task-item from database.
        /// </summary>
        /// <param name="task">Task to delete.</param>
        /// <returns></returns>
        Task<bool> DeleteTask(TaskViewModel task);

        /// <summary>
        /// Update task-item in database.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<bool> UpdateTask(TaskViewModel task);

        /// <summary>
        /// Add subtask-item to database.
        /// </summary>
        /// <param name="subtask">Subtask to add.</param>
        /// <param name="id"></param>
        /// <returns>Id of inserted item</returns>
        Task<int> AddSubtask(SubtaskViewModel subtask, int id);

        /// <summary>
        /// Delete subtask-item from database.
        /// </summary>
        /// <param name="subtask"></param>
        /// <returns></returns>
        Task<bool> DeleteSubtask(SubtaskViewModel subtask);

        /// <summary>
        /// Update subtask-item in database.
        /// </summary>
        /// <param name="subtask"></param>
        /// <returns></returns>
        Task<bool> UpdateSubtask(SubtaskViewModel subtask);

        /// <summary>
        /// Update a list of subtask-items in database.
        /// </summary>
        /// <param name="subtasks"></param>
        /// <returns></returns>
        Task<bool> UpdateSubtasks(IEnumerable<SubtaskViewModel> subtasks);
    }
}
