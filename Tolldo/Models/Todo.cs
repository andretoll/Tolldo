using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tolldo.Models
{
    /// <summary>
    /// The view model for todo items. Based on <see cref="INotifyPropertyChanged"/> and <see cref="IDropTarget"/>.
    /// </summary>
    public class Todo : INotifyPropertyChanged, IDropTarget
    {
        #region Private Members

        private int _progress;
        private int _lastProgress;
        private ObservableCollection<TodoTask> _tasks;

        #endregion

        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ObservableCollection<TodoTask> Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
                NotifyPropertyChanged();
            }
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
        /// Calculated progress based on completed tasks.
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

        #region Events

        /// <summary>
        /// The event that is fired when any child property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Update current and last progress.
        /// </summary>
        public void UpdateProgress()
        {
            LastProgress = Progress;
            Progress = CalculateProgress();
        }

        /// <summary>
        /// Calculate progress.
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

        #region DragDrop Interface

        /// <summary>
        /// When dragging item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {

            TodoTask sourceItem = dropInfo.Data as TodoTask;
            TodoTask targetItem = dropInfo.TargetItem as TodoTask;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        /// <summary>
        /// When dropping item.
        /// </summary>
        /// <param name="dropInfo"></param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            TodoTask sourceItem = dropInfo.Data as TodoTask;
            TodoTask targetItem = dropInfo.TargetItem as TodoTask;

            Tasks.Move(Tasks.IndexOf(sourceItem), Tasks.IndexOf(targetItem));
        }

        #endregion
    }
}
