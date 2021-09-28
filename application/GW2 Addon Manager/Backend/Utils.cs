using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GW2_Addon_Manager
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public delegate void UpdateMessageChangedEventHandler(object sender, string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pct"></param>
    public delegate void UpdateProgressChangedEventHandler(object sender, int pct);

    public static class Constants
    {
        public const string AddonFolder = "resources\\addons";
    }

    public static class Utils
    {
        public static TValue? GetNull<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class
        {
            return dict.TryGetValue(key, out var val) ? val : null;
        }

        public static WebClient OpenWebClient()
        {
            var client = new WebClient();
            var name = typeof(Utils).Assembly.GetName();
            client.Headers.Add("User-Agent", $"{name.FullName} {name.Version.Major}.{name.Version.Minor}.{name.Version.Build}");

            return client;
        }

        public static string GetRelativePath(this IPath inst, string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path);
        }
    }

    public class DelayedMessageBox : Xceed.Wpf.Toolkit.MessageBox
    {
        private Timer _timer;
        private int _count = 4;
        private object _origOk, _origYes, _origNo;

        private void Tick()
        {
            _count--;

            var no = GetTemplateChild("PART_NoButton") as Button;
            no.IsEnabled = _count == 0;
            var yes = GetTemplateChild("PART_YesButton") as Button;
            yes.IsEnabled = _count == 0;
            var ok = GetTemplateChild("PART_OkButton") as Button;
            ok.IsEnabled = _count == 0;

            if (_count == 0) {
                OkButtonContent = _origOk;
                YesButtonContent = _origYes;
                NoButtonContent = _origNo;
                _timer.Dispose();
            } else {
                if (_origOk is string)
                    OkButtonContent = $"{_origOk} ({_count})";
                if (_origYes is string)
                    YesButtonContent = $"{_origYes} ({_count})";
                if (_origNo is string)
                    NoButtonContent = $"{_origNo} ({_count})";
            }
        }

        public DelayedMessageBox()
        {
            Loaded += DelayedMessageBox_Loaded;
        }

        private void DelayedMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            _origOk = OkButtonContent;
            _origYes = YesButtonContent;
            _origNo = NoButtonContent;

            _timer = new Timer((object state) =>
            {
                Tick();
            }, this, 0, 1000);
        }

        private static MessageBoxResult ShowCore(int delaySeconds, Window owner, IntPtr ownerHandle, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, Style messageBoxStyle)
        {
            if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) {
                throw new InvalidOperationException("Static methods for MessageBoxes are not available in XBAP. Use the instance ShowMessageBox methods instead.");
            }

            if ((owner != null) && (ownerHandle != IntPtr.Zero)) {
                throw new NotSupportedException("The owner of a MessageBox can't be both a Window and a WindowHandle.");
            }

            var msgBox = new DelayedMessageBox();
            msgBox._count = delaySeconds + 1;
            msgBox.InitializeMessageBox(owner, ownerHandle, messageText, caption, button, icon, defaultResult);

            // Setting the style to null will inhibit any implicit styles      
            if (messageBoxStyle != null) {
                msgBox.Style = messageBoxStyle;
            }

            msgBox.ShowDialog();
            return msgBox.MessageBoxResult;
        }

        public static new MessageBoxResult Show(int delaySeconds, string messageText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowCore(delaySeconds, null, IntPtr.Zero, messageText, caption, button, icon, MessageBoxResult.None, null);
        }
    }
}
