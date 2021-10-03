using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace GW2AddonManager
{
    public class UpdatingViewModel : ObservableObject
    {
        private bool _closeBtnEnabled;
        private bool _backBtnEnabled;
        private string _progBarLabel;
        private int _downloadProgress;

        public bool CloseBtnEnabled { get => _closeBtnEnabled; set => SetProperty(ref _closeBtnEnabled, value); }
        public bool BackBtnEnabled { get => _backBtnEnabled; set => SetProperty(ref _backBtnEnabled, value); }
        public string ProgBarLabel { get => _progBarLabel; set => SetProperty(ref _progBarLabel, value); }
        public int DownloadProgress { get => _downloadProgress; set => SetProperty(ref _downloadProgress, value); }

        public UpdatingViewModel()
        {
        }
    }
}
