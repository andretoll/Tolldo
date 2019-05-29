namespace Tolldo.ViewModels
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
            }
        }

        #endregion
    }
}
