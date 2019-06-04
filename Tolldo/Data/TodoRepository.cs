using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        #region Protected Members

        protected IMapper _mapper;
        protected IDialogService _dialogService;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor. Accepts dependency injection of type <see cref="IDialogService"/>.
        /// </summary>
        /// <param name="dialogService">Accepts dependency injection of type <see cref="IDialogService"/>.</param>
        public TodoRepository(IDialogService dialogService)
        {
            // Initialize services and ensure data store
            _dialogService = dialogService;
            EnsureDataBase();

            // Initialize automapper with profile. Inject the dialog service
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfiles(dialogService));
            }));
        }        

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Ensures that the database for the context exists.
        /// </summary>
        public void EnsureDataBase()
        {
            using (var context = new TolldoDbContext())
            {
                context.Database.EnsureCreated();
            }            
        }

        /// <summary>
        /// Returns the todo-items from the data context.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TodoViewModel> GetTodos()
        {
            // Get todos, tasks and subtasks
            using (var context = new TolldoDbContext())
            {
                // Get items from database
                var todos = context.Todos
                .Include(a => a.Tasks)                
                .ThenInclude(b => b.Subtasks)
                .ToList();

                // Map items
                var todoViewModels = _mapper.Map<List<TodoViewModel>>(todos);

                // Order tasks by order property
                foreach (var todo in todoViewModels)
                {
                    var tasks = todo.Tasks.OrderBy(a => a.Order).ToList();
                    todo.Tasks = new System.Collections.ObjectModel.ObservableCollection<TaskViewModel>(tasks);
                }

                // Return items
                return todoViewModels;
            }      
        }

        /// <summary>
        /// Adds a todo-item to the data context.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <returns>Primary key for inserted item.</returns>
        public async Task<int> AddTodo(TodoViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Create item to add
                var itemToAdd = _mapper.Map<Todo>(item);

                // Add item to database
                context.Add(itemToAdd);
                await context.SaveChangesAsync();

                return itemToAdd.Id; 
            }
        }

        /// <summary>
        /// Deletes a todo-item from the data context.
        /// </summary>
        /// <param name="item">Item to delete.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> DeleteTodo(TodoViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to remove
                var itemToRemove = await context.Todos.Where(i => i.Id == item.Id).FirstOrDefaultAsync();

                if (itemToRemove == null)
                    return false;

                // Remove item from database
                context.Remove(itemToRemove);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }

        /// <summary>
        /// Updates a todo-item in the data context.
        /// </summary>
        /// <param name="item">Item to update.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> UpdateTodo(TodoViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to update
                var itemToUpdate = await context.Todos.Where(i => i.Id == item.Id).FirstOrDefaultAsync();
                _mapper.Map(item, itemToUpdate);

                if (itemToUpdate == null)
                    return false;

                // Update item in database   
                itemToUpdate.Tasks = null;
                context.Update(itemToUpdate);

                // Ignore tasks
                context.Entry(itemToUpdate).Collection(x => x.Tasks).IsModified = false;

                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }

        /// <summary>
        /// Adds a task-item to the data context.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="id">Foreign key to the item.</param>
        /// <returns>Primary key for inserted item.</returns>
        public async Task<int> AddTask(TaskViewModel item, int id)
        {
            using (var context = new TolldoDbContext())
            {
                // Create item to add
                var itemToAdd = _mapper.Map<TodoTask>(item);

                // Add foreign key
                itemToAdd.TodoId = id;

                // Add item to database
                context.Add(itemToAdd);
                await context.SaveChangesAsync();

                return itemToAdd.Id;
            }                
        }

        /// <summary>
        /// Deletes a task-item from the data context.
        /// </summary>
        /// <param name="item">Item to delete.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> DeleteTask(TaskViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to remove
                var itemToRemove = await context.Tasks.Where(i => i.Id == item.Id).FirstOrDefaultAsync();

                if (itemToRemove == null)
                    return false;

                // Remove item from database
                context.Remove(itemToRemove);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }

        /// <summary>
        /// Updates a todo-item in the data context.
        /// </summary>
        /// <param name="item">Item to update.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> UpdateTask(TaskViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to update
                var itemToUpdate = await context.Tasks.Where(i => i.Id == item.Id).FirstOrDefaultAsync();
                _mapper.Map(item, itemToUpdate);

                if (itemToUpdate == null)
                    return false;

                // Update item in database
                itemToUpdate.Subtasks = null;
                context.Update(itemToUpdate);

                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }

        /// <summary>
        /// Adds a subtask-item to the data context.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="id">Foreign key to the item.</param>
        /// <returns>Primary key for inserted item.</returns>
        public async Task<int> AddSubtask(SubtaskViewModel item, int id)
        {
            using (var context = new TolldoDbContext())
            {
                // Create item to add
                var itemToAdd = _mapper.Map<Subtask>(item);

                // Add foreign key
                itemToAdd.TodoTaskId = id;

                // Add item to database
                context.Add(itemToAdd);
                await context.SaveChangesAsync();

                return itemToAdd.Id;
            }                
        }

        /// <summary>
        /// Deletes a subtask-item from the data context.
        /// </summary>
        /// <param name="item">Item to delete.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> DeleteSubtask(SubtaskViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to remove
                var itemToRemove = await context.Subtasks.Where(i => i.Id == item.Id).FirstOrDefaultAsync();

                if (itemToRemove == null)
                    return false;

                // Remove item from database
                context.Remove(itemToRemove);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }     

        /// <summary>
        /// Updates a subtask-item in the data context.
        /// </summary>
        /// <param name="item">Item to update.</param>
        /// <returns>Success value.</returns>
        public async Task<bool> UpdateSubtask(SubtaskViewModel item)
        {
            using (var context = new TolldoDbContext())
            {
                // Get item to update
                var itemToUpdate = await context.Subtasks.Where(i => i.Id == item.Id).FirstOrDefaultAsync();
                _mapper.Map(item, itemToUpdate);

                if (itemToUpdate == null)
                    return false;

                // Update item in database
                context.Update(itemToUpdate);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }                
        }

        #endregion
    }
}
