using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GW2_Addon_Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            #if DEBUG

            e.Handled = false;

            #else

            ShowUnhandledException(e);

            #endif
        }

        void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            string errmsg = string.Format("An unhandled exception occurred.",
                e.Exception.Message + (e.Exception.InnerException != null ? "\n" + e.Exception.InnerException : null));
            if (MessageBox.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }

        }
    }
}
