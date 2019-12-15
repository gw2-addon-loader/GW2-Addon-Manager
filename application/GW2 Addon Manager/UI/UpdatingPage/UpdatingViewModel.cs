using System.ComponentModel;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>UpdatingViewModel</c> serves as the DataContext for UpdatingView.xaml, which is the screen displayed during and after the update process.
    /// </summary>
    public class UpdatingViewModel : INotifyPropertyChanged
    {
        /* private fields for ui bindings */
        private bool _closeBtnEnabled;
        private string _msg;
        private int _progress;

        /********** UI BINDINGS **********/
        /// <summary>
        /// Binding for whether the "FINISH" button is enabled.
        /// </summary>
        public bool CloseBtnEnabled
        {
            get { return _closeBtnEnabled; }
            set { _closeBtnEnabled = value; propertyChanged($"{nameof(CloseBtnEnabled)}"); }
        }
        /// <summary>
        /// Binding for the label above the progress bar.
        /// </summary>
        public string ProgBarLabel
        {
            get { return _msg; }
            set { _msg = value; propertyChanged($"{nameof(ProgBarLabel)}"); }
        }
        /// <summary>
        /// Binding for the value shown in the progress bar.
        /// </summary>
        public int DownloadProgress
        {
            get { return _progress; }
            set { _progress = value; propertyChanged($"{nameof(DownloadProgress)}"); }
        }


        /********** Class Structure/Other Methods **********/

        /*** Notify UI of Changed Binding Items ***/
        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>x
        /// A method used in notifying that a property's value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed value.</param>
        protected void propertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /* Singleton */
        private static UpdatingViewModel onlyInstance;
        /// <summary>
        /// The constructor sets the label to a placeholder value and disables the "finish" button.
        /// </summary>
        private UpdatingViewModel()
        {
            onlyInstance = this;
            ProgBarLabel = "Updating Add-Ons";
            CloseBtnEnabled = false;
        }
        /// <summary>
        /// Accessor for the one instance of this class; if the instance has not been initialized, does that before returning it.
        /// </summary>
        public static UpdatingViewModel GetInstance
        {
            get { return (onlyInstance == null) ? new UpdatingViewModel() : onlyInstance; }
        }
    }
}
