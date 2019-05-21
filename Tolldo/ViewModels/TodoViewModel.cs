using GongSolutions.Wpf.DragDrop;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tolldo.Models;

namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Todo-object. Based on the <see cref="Todo"/> class and implements the <see cref="IDropTarget"/> interface.
    /// </summary>
    public class TodoViewModel : Todo, IDropTarget
    {
        #region Private Members

        // Current progress
        private int _progress;

        // Last known progress
        private int _lastProgress;

        #endregion

        #region Commands

        public ICommand CompleteAllTasksCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TodoViewModel()
        {
            // Initialize commands
            CompleteAllTasksCommand = new RelayCommand.RelayCommand(async p => { await CompleteAllTasks(); }, p => Tasks.Count > 0);
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Calculated progress based on completed tasks.
        /// </summary>
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                NotifyPropertyChanged();

                // Notify change in dependent properties
                NotifyPropertyChanged("ProgressString");
                NotifyPropertyChanged("Completed");
            }
        }

        /// <summary>
        /// Calculated progress based on last completed task
        /// </summary>
        public int LastProgress
        {
            get
            {
                return _lastProgress;
            }
            set
            {
                _lastProgress = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Calculated progress based on completed tasks in <see cref="String"/> format.
        /// </summary>
        public string ProgressString
        {
            get
            {
                int p = 0;

                foreach (var task in Tasks)
                {
                    p += task.Completed ? 1 : 0;
                }

                return $"{p}/{Tasks.Count}";
            }
        }

        /// <summary>
        /// Calculated completion status based on completed tasks.
        /// </summary>
        public bool Completed
        {
            get
            {
                foreach (var task in Tasks)
                {
                    if (!task.Completed)
                        return false;
                }

                return Tasks.Count > 0 ? true : false;
            }
        }

        #endregion

        #region Public Helpers

        /// <summary>
        /// Updates the current and last known progress.
        /// </summary>
        public void UpdateProgress()
        {
            LastProgress = Progress;
            Progress = CalculateProgress();
        }

        /// <summary>
        /// Calculates the current progress based on number of completed tasks.
        /// </summary>
        /// <returns></returns>
        public int CalculateProgress()
        {
            int p = 0;

            foreach (var task in Tasks)
            {
                p += task.Completed ? 1 : 0;
            }

            return Tasks.Count == 0 ? 0 : (p * 100) / Tasks.Count;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Completes all current tasks.
        /// </summary>
        private async Task CompleteAllTasks()
        {
            await Task.Run(() =>
            {
                foreach (var task in this.Tasks)
                {
                    if (!task.Completed)
                    {
                        task.Completed = true;
                    }
                }
            });
        }

        #endregion

        #region DragDrop Interface

        /// <summary>
        /// Method called when dragging an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Get source and target
            TodoTask sourceItem = dropInfo.Data as TodoTask;
            TodoTask targetItem = dropInfo.TargetItem as TodoTask;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// Method called when dropping an item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            // Get source and target
            TodoTask sourceItem = dropInfo.Data as TodoTask;
            TodoTask targetItem = dropInfo.TargetItem as TodoTask;

            // Let the tasks switch places
            Tasks.Move(Tasks.IndexOf(sourceItem), Tasks.IndexOf(targetItem));
        }

        #endregion
    }
}
