﻿namespace Tolldo.ViewModels
{
    /// <summary>
    /// The ViewModel for a single Subtask-object. Based on the <see cref="BaseViewModel"/> class.
    /// </summary>
    public class SubtaskViewModel : BaseViewModel
    {
        #region Private Members

        #region Model

        private string _name;

        private bool _completed; 

        #endregion

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
                NotifyPropertyChanged(nameof(IsCompleted));
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
                base.NotifyPropertyChanged();
            }
        }

        public int TodoTaskId { get; set; }

        #endregion

        #region Helper Properties

        public bool NoAction { get; set; }

        #endregion

        #region Base Class Override

        /// <summary>
        /// Invoke PropertyChanged with no actions.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="noAction"></param>
        override protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            (this as SubtaskViewModel).NoAction = true;
            base.RaiseNotifyPropertyChangedEvent(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            (this as SubtaskViewModel).NoAction = false;
        } 

        #endregion
    }
}
