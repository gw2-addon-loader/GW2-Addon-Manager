using System.ComponentModel;

namespace GW2AddonManager
{
    /// <summary>
    /// <c>UpdatingViewModel</c> serves as the DataContext for UpdatingView.xaml, which is the screen displayed during and after the update process.
    /// </summary>
    public class UpdatingViewModel : INotifyPropertyChanged
    {
        /********** UI BINDINGS **********/
        /// <summary>
        /// Binding for whether the "FINISH" button is enabled.
        /// </summary>
        public bool CloseBtnEnabled { get; set; }
        /// <summary>
        /// Binding for whether the "BACK" button is enabled.
        /// </summary>
        public bool BackBtnEnabled { get; set; }
        /// <summary>
        /// Binding for the label above the progress bar.
        /// </summary>
        public string ProgBarLabel { get; set; }
        /// <summary>
        /// Binding for the value shown in the progress bar.
        /// </summary>
        public int DownloadProgress { get; set; }

        /********** Class Structure/Other Methods **********/

        /*** Notify UI of Changed Binding Items ***/
        /// <summary>
        /// An event used to indicate that a property's value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The constructor sets the label to a placeholder value and disables the "finish" button.
        /// </summary>
        public UpdatingViewModel()
        {
            ProgBarLabel = "Updating Addons";
            CloseBtnEnabled = false;
            BackBtnEnabled = false;
        }
    }
}
