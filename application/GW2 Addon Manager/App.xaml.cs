using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GW2AddonManager
{
    /// <summary>
    /// Interaction logic for App.xaml. Currently, the functions here are dedicated solely to application-wide exception handling and error logging.
    /// </summary>
    public partial class App
    {
        private ServiceProvider serviceProvider;
        private NamedMutex _mutex;

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            _mutex = new NamedMutex("GW2AddonManager", true);

            ServiceCollection services = new();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
            services.AddSingleton<IAddonRepository, AddonRepository>();
            services.AddSingleton<IAddonManager, AddonManager>();
            services.AddSingleton<SelfManager>();
            services.AddSingleton<LoaderManager>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<OpeningViewModel>();
            services.AddSingleton<UpdatingViewModel>();

            services.AddSingleton<MainWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            SetCulture();
            Application.Current.Exit += new ExitEventHandler((_, _) => _mutex.Dispose());

            Application.Current.MainWindow = serviceProvider.GetService<MainWindow>();
            Application.Current.MainWindow.Show();
        }

        private void SetCulture()
        {
            ConfigurationProvider configurationManager = new ConfigurationProvider();
            CultureInfo culture = new CultureInfo(configurationManager.UserConfig.Culture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _mutex.Dispose();
            ShowUnhandledException(e);
        }

        /// <summary>
        /// Displays a message and exits when an exception is thrown.
        /// </summary>
        /// <param name="sender">The object sending the exception.</param>
        /// <param name="e">The exception information.</param>
        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _mutex.Dispose();
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
                Application.Current.Shutdown();
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
                Application.Current.Shutdown();
            }

        }
    }
}
