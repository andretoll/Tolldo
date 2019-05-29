using AutoMapper;
using Tolldo.Models;
using Tolldo.Services;
using Tolldo.ViewModels;

namespace Tolldo.Data
{
    /// <summary>
    /// Profiles for <see cref="AutoMapper"/>.
    /// </summary>
    public class AutoMapperProfiles : Profile
    {
        #region Constructor

        /// <summary>
        /// Default constructor. Accepts dependency injection of type <see cref="IDialogService"/>.
        /// </summary>
        /// <param name="dialogService">Accepts dependency injection of type <see cref="IDialogService"/>.</param>
        public AutoMapperProfiles(IDialogService dialogService)
        {
            CreateMap<Todo, TodoViewModel>()
                .ConstructUsing(t => new TodoViewModel(dialogService));

            CreateMap<TodoViewModel, Todo>();

            CreateMap<TodoTask, TaskViewModel>()
                .ConstructUsing(t => new TaskViewModel(dialogService));

            CreateMap<TaskViewModel, TodoTask>();

            CreateMap<Subtask, SubtaskViewModel>();

            CreateMap<SubtaskViewModel, Subtask>();
        }

        #endregion
    }
}
