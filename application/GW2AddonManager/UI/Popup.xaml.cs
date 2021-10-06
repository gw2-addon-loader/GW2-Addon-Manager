using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GW2AddonManager
{
    // FIXME: Now using custom popup window, just need sound and optional delay before buttons unfreeze.
    public partial class Popup : Window
    {
        Storyboard _hide, _show;
        private DispatcherTimer _timer;
        int _delay;
        string _initOk, _initYes;

        public MessageBoxResult Result => (DataContext as PopupViewModel).Result;

        private (double, double) ComputeLeftTop()
        {
            var refw = Application.Current.MainWindow;

            double left = refw.Left + refw.Width / 2 - Width / 2;
            double top = refw.Top + refw.Height / 6;

            return (left, top);
        }

        public Popup(string content, string title = "Message Box", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, int delay = 0)
        {
            var vm = new PopupViewModel(content, title, buttons, image, delay);
            DataContext = vm;
            Opacity = 0;

            InitializeComponent();

            _show = FindResource("ShowWindow") as Storyboard;
            _hide = FindResource("HideWindow") as Storyboard;
            _hide.Completed += (_, _) => Close();

            vm.RequestClose += (_, _) => _hide.Begin(this);
            Loaded += (_, _) => {
                (Left, Top) = ComputeLeftTop();
                _show.Begin(this);
            };

            if (vm.Delay > 0)
            {
                _initOk = Ok.Content as string;
                _initYes = Yes.Content as string;
                Ok.IsEnabled = Yes.IsEnabled = false;
                _delay = vm.Delay;
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
                _timer.Tick += (_, _) => {
                    _delay -= 100;
                    if (_delay <= 0)
                    {
                        _delay = 0;
                        _timer.Stop();
                        Ok.IsEnabled = Yes.IsEnabled = true;
                        Ok.Content = _initOk;
                        Yes.Content = _initYes;
                    }
                    else
                    {
                        Ok.Content = $"{_initOk} ({(_delay + 999) / 1000})";
                        Yes.Content = $"{_initYes} ({(_delay + 999) / 1000})";
                    }

                };
                _timer.Start();
            }
        }

        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender && e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public static MessageBoxResult Show(string content, string title = "Message Box", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None, int delay = 0)
        {
            var p = new Popup(content, title, buttons, image, delay);
            _ = p.ShowDialog();

            return p.Result;
        }
    }
}
