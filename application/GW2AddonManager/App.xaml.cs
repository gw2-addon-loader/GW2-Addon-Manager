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
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private NamedMutex _mutex;

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services => _serviceProvider;

        public App()
        {
            _mutex = new NamedMutex("GW2AddonManager", true);

            _serviceProvider = ConfigureServices();
        }

        private void ClearMutex()
        {
            if (_mutex is not null)
                _mutex.Dispose();
            _mutex = null;
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                          .AddSingleton<IFileSystem, FileSystem>()
                          .AddSingleton<IHttpClientProvider, HttpClientProvider>()
                          .AddSingleton<IConfigurationProvider, ConfigurationProvider>()
                          .AddSingleton<IAddonRepository, AddonRepository>()
                          .AddSingleton<IAddonManager, AddonManager>()
                          .AddSingleton<ISelfManager, SelfManager>()
                          .AddSingleton<ILoaderManager, LoaderManager>()
                          .AddSingleton<ICoreManager, CoreManager>()
                          .AddTransient<MainWindowViewModel>()
                          .AddTransient<OpeningViewModel>()
                          .AddTransient<UpdatingViewModel>()
                          .AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            #if !DEBUG
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(AppDispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
            #endif

            _serviceProvider.GetService<ICoreManager>().UpdateCulture(_serviceProvider.GetService<IConfigurationProvider>().UserConfig.Culture);
            Application.Current.Exit += new ExitEventHandler((_, _) => ClearMutex());

            Application.Current.MainWindow = _serviceProvider.GetService<MainWindow>();
            Application.Current.MainWindow.Show();
        }

        internal void ReopenMainWindow()
        {
            var oldWindow = Application.Current.MainWindow;
            Application.Current.MainWindow = _serviceProvider.GetService<MainWindow>();
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Top = oldWindow.Top;
            Application.Current.MainWindow.Left = oldWindow.Left;
            oldWindow.Close();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ClearMutex();
            ShowUnhandledException(e);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ClearMutex();
            ShowUnhandledException(e);
        }

        private void ShowUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            //LogError(logPath, e);
            string errmsg = "An unhandled exception occurred." + "\n" + e.Exception.Message + (e.Exception.InnerException != null ? "\n" + e.Exception.InnerException.Message : "");
            if (Popup.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                Application.Current.Shutdown();
        }

        private void ShowUnhandledException(UnhandledExceptionEventArgs e)
        {
            //LogError(logPath, e);
            Exception exc = (Exception)e.ExceptionObject;
            string errmsg = "An unhandled exception occurred." + "\n" + exc.Message + (exc.InnerException != null ? "\n" + exc.InnerException.Message : "");
            if (Popup.Show(errmsg, "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                Application.Current.Shutdown();
        }
    }
}
