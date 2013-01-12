using System.Windows;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskManagerViewModel taskManagerViewModel;
        public MainWindow()
        {
            InitializeComponent();
            taskManagerViewModel = new TaskManagerViewModel();
            DataContext = taskManagerViewModel;
        }
    }
}
