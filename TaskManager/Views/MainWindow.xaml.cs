using System;
using System.Windows;
using TaskManager.ViewModels;

namespace TaskManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string text = String.Format("{0}:\n{1}", Properties.Resources.MSG_UnhandledError_Text, e.ExceptionObject);
            MessageBox.Show(text, Properties.Resources.MSG_UnhandledError, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
