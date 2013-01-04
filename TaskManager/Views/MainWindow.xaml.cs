using System.Windows;
using TaskManager.ViewModels;
using TaskManager.Models;

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

        private void TasksTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            taskManagerViewModel.SelectedTaskModel = (TasksModel) e.NewValue;
        }
    }
}
