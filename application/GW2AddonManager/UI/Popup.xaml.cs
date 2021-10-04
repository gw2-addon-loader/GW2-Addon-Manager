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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GW2AddonManager
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        public Popup(MessageBoxButton buttons)
        {
            var vm = new PopupViewModel(buttons);
            DataContext = vm;
            vm.RequestClose += (_, _) => Close();
            InitializeComponent();
        }

        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender && e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
