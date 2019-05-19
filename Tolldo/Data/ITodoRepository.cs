using System.Collections.Generic;
using Tolldo.ViewModels;

namespace Tolldo.Data
{
    /// <summary>
    /// Data provider interface for Tolldo.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Get a list of all Todo-items.
        /// </summary>
        /// <returns></returns>
        List<TodoViewModel> GetTodos();
    }
}
