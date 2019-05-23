using System.Windows.Controls;
using Tolldo.Services;
using Tolldo.ViewModels;

namespace Tolldo.Views.UserControls
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        public MainUserControl()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel(new DialogService());
        }
    }
}
