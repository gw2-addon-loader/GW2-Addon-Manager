using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml. Currently, the functions here are dedicated solely to application-wide exception handling and error logging.
    /// </summary>
    public partial class App : Application
    {
        static string logPath = "log.txt";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
        }

        void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            #if DEBUG

            e.Handled = false;

            #else

            ShowUnhandledException(e);

            #endif
        }

        /// <summary>
        /// Displays a message and exits when an exception is thrown.
        /// </summary>
        /// <param name="sender">The object sending the exception.</param>
        /// <param name="e">The exception information.</param>
        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            #if DEBUG

            e.Handled = false;

            #else

            ShowUnhandledException(e);

            #endif
        }

        /// <summary>
        /// Displays an error message when an unhandled exception is thrown.
        /// </summary>
        /// <param name="e">The exception information.</param>
        void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogError(logPath, e);
            string errmsg = "An unhandled exception occurred." + "\n" + e.Exception.Message + (e.Exception.InnerException != null ? "\n" + e.Exception.InnerException.Message : "");
            if (MessageBox.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }

        }

        /// <summary>
        /// Displays an error message when an unhandled exception is thrown.
        /// </summary>
        /// <param name="e">The exception information.</param>
        void ShowUnhandledException(UnhandledExceptionEventArgs e)
        {
            LogError(logPath, e);
            Exception exc = (Exception) e.ExceptionObject;
            string errmsg = "An unhandled exception occurred." + "\n" + exc.Message + (exc.InnerException != null ? "\n" + exc.InnerException.Message : "");
            if (MessageBox.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }

        }

        /// <summary>
        /// Writes information about unhandled exceptions to a log file.
        /// </summary>
        /// <param name="logfile">The path to the log file to be written to.</param>
        /// <param name="e">The exception information.</param>
        void LogError(string logfile, DispatcherUnhandledExceptionEventArgs e)
        {
            string header = "[Log Entry]\n";
            string exceptionTree = "";

            Exception ex = e.Exception;
            exceptionTree += ex.Message + "\n";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                exceptionTree += ex.Message + "\n";
            }

            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString() + "\n";

            string fullLogMsg = header + date + exceptionTree;
            File.AppendAllText(logfile, fullLogMsg);
        }

        /// <summary>
        /// Writes information about unhandled exceptions to a log file.
        /// </summary>
        /// <param name="logfile">The path to the log file to be written to.</param>
        /// <param name="e">The exception information.</param>
        void LogError(string logfile, UnhandledExceptionEventArgs e)
        {
            string header = "[Log Entry]\n";
            string exceptionTree = "";

            Exception ex = (Exception)e.ExceptionObject;
            exceptionTree += ex.Message + "\n";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                exceptionTree += ex.Message + "\n";
            }

            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString() + "\n";

            string fullLogMsg = header + date + exceptionTree;
            File.AppendAllText(logfile, fullLogMsg);
        }
    }
}
