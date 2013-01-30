using System;
using System.Windows;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string text = String.Format("{0}:\n{1}", TaskManager.Properties.Resources.MSG_UnhandledError_Text, e.ExceptionObject);
            MessageBox.Show(text, TaskManager.Properties.Resources.MSG_UnhandledError, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
