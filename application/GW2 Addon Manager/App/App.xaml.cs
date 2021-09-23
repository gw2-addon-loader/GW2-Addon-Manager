using GW2_Addon_Manager.App.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GW2_Addon_Manager.App
{
    /// <summary>
    /// Interaction logic for App.xaml. Currently, the functions here are dedicated solely to application-wide exception handling and error logging.
    /// </summary>
    public partial class App
    {
        private ServiceProvider serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<AddonYamlReader>();
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            services.AddSingleton<ApprovedList>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<GenericUpdaterFactory>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<OpeningViewModel>();
            services.AddSingleton<UpdatingViewModel>();
            services.AddSingleton<SelfUpdate>();
            services.AddSingleton<LoaderSetup>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            SetCulture();

            MainWindow = serviceProvider.GetService<MainWindow>();
            MainWindow.Show();
        }

        private void SetCulture()
        {
            ConfigurationManager configurationManager = new ConfigurationManager();
            CultureInfo culture = new CultureInfo(configurationManager.UserConfig.Culture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnhandledException(e);
        }

        /// <summary>
        /// Displays a message and exits when an exception is thrown.
        /// </summary>
        /// <param name="sender">The object sending the exception.</param>
        /// <param name="e">The exception information.</param>
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowUnhandledException(e);
        }

        /// <summary>
        /// Displays an error message when an unhandled exception is thrown.
        /// </summary>
        /// <param name="e">The exception information.</param>
        private void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            //LogError(logPath, e);
            string errmsg = "An unhandled exception occurred." + "\n" + e.Exception.Message + (e.Exception.InnerException != null ? "\n" + e.Exception.InnerException.Message : "");
            if (MessageBox.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                Shutdown();
            }
        }

        /// <summary>
        /// Displays an error message when an unhandled exception is thrown.
        /// </summary>
        /// <param name="e">The exception information.</param>
        private void ShowUnhandledException(UnhandledExceptionEventArgs e)
        {
            //LogError(logPath, e);
            Exception exc = (Exception) e.ExceptionObject;
            string errmsg = "An unhandled exception occurred." + "\n" + exc.Message + (exc.InnerException != null ? "\n" + exc.InnerException.Message : "");
            if (MessageBox.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
            {
                Shutdown();
            }

        }
    }
}
