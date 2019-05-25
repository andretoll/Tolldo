﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tolldo.Models
{
    /// <summary>
    /// The model for Task-items. Implements the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public class TodoTask : INotifyPropertyChanged
    {
        #region Private Members

        private string _name;
        private string _description;        
        private ObservableCollection<Subtask> _subtasks;

        #endregion

        #region Public Members

        public bool _completed;

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                NotifyPropertyChanged();

                // If subtasks exist, set all subtasks' completion status equal to the task's
                if (Subtasks != null)
                {
                    foreach (var subtask in Subtasks)
                    {
                        subtask.Completed = value;
                    } 
                }
            }
        }

        public ObservableCollection<Subtask> Subtasks
        {
            get
            {
                return _subtasks;
            }
            set
            {
                _subtasks = value;
                NotifyPropertyChanged();
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
    }
}
