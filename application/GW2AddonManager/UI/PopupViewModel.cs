using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GW2AddonManager
{
    public class PopupViewModel : DependentObservableObject
    {
        private DispatcherTimer _timer;

        public event EventHandler RequestClose;

        public MessageBoxResult Result { get; private set; }
        public MessageBoxButton Buttons { get; init; }
        public MessageBoxImage Image { get; init; }

        public string Title { get; init; }
        public string Content { get; init; }
        public int Delay { get; init; }

        public Visibility YesVisible => Buttons == MessageBoxButton.YesNoCancel || Buttons == MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NoVisible => Buttons == MessageBoxButton.YesNoCancel || Buttons == MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility OkVisible => Buttons == MessageBoxButton.OK || Buttons == MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CancelVisible => Buttons == MessageBoxButton.OKCancel || Buttons == MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ErrorVisible => Image == MessageBoxImage.Error ? Visibility.Visible : Visibility.Collapsed;
        public Visibility WarningVisible => Image == MessageBoxImage.Warning ? Visibility.Visible : Visibility.Collapsed;
        public Visibility InfoVisible => Image == MessageBoxImage.Information ? Visibility.Visible : Visibility.Collapsed;
        public Visibility QuestionVisible => Image == MessageBoxImage.Question ? Visibility.Visible : Visibility.Collapsed;

        public ICommand ButtonClick => new RelayCommand<MessageBoxResult>(btn =>
        {
            Result = btn;
            RequestClose?.Invoke(this, null);
        });

        public PopupViewModel(string content, string title, MessageBoxButton buttons, MessageBoxImage image, int delay)
        {
            Content = content;
            Title = title;
            Buttons = buttons;
            Image = image;
            Delay = delay;
        }
    }
}
