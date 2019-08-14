using System.ComponentModel;

namespace GW2_Addon_Updater
{
    public class UpdatingView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void propertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        string message;
        int progress;
        bool _closeButtonEnabled;

        public bool closeButtonEnabled
        {
            get {return _closeButtonEnabled;}
            set {
                _closeButtonEnabled = value;
                propertyChanged("closeButtonEnabled");
            }
        }

        public string label
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                propertyChanged("label");
            }
        }


        public int showProgress
        {
            get
            {
                return progress;
            }

            set
            {
                progress = value;
                propertyChanged("showProgress");
            }
        }        
    }
}
