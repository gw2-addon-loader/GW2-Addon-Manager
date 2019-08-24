using System.ComponentModel;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>UpdatingViewModel</c> serves as the DataContext for UpdatingView.xaml, which is the screen displayed during and after the update process.
    /// </summary>
    public class UpdatingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void propertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The constructor sets the label to a placeholder value and disables the "finish" button.
        /// </summary>
        public UpdatingViewModel()
        {
            label = "Updating Add-Ons";
            closeButtonEnabled = false;
        }

        
        bool _closeBtnEnabled;
        public bool closeButtonEnabled
        {
            get {return _closeBtnEnabled;}
            set { _closeBtnEnabled = value; propertyChanged("closeButtonEnabled"); }
        }

        string _msg;
        public string label
        {
            get { return _msg; }
            set { _msg = value; propertyChanged("label"); }
        }

        int _progress;
        public int showProgress
        {
            get { return _progress; }
            set { _progress = value; propertyChanged("showProgress"); }
        }        
    }
}
