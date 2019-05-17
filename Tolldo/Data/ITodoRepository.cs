using System.Collections.Generic;
using Tolldo.Models;

namespace Tolldo.Data
{
    /// <summary>
    /// Interface for Todo.
    /// </summary>
    public interface ITodoRepository
    {
        List<Todo> GetTodos();
    }
}
