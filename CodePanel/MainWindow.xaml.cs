using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace CodePanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> _vaultPasswords = new List<string>(Properties.Settings.Default.vaultPasswords);
        private List<string> _androidPasswords = new List<string>(Properties.Settings.Default.androidPasswords);
        private List<string> _cameraPasswords = new List<string>(Properties.Settings.Default.cameraPasswords);

        // Checks password and removes it from source
        private bool CheckPassword(IList<string> passwordSource, string password)
        {
            if (passwordSource == null || password == null || password == string.Empty)
                return false;
            return passwordSource.Remove(password);
        }


        // Prevents closing window in any common way
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
#if !DEBUG
            e.Cancel = true;
#endif
        }

        // Prevents all clicks except LMB
        private void MainWindow_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                e.Handled = true;
        }

        private void ErrorClose_OnClick(object sender, RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Collapsed;
            BluringGrid.Effect = null;
        }

        private void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            IList<string> passwordSource = null;
            var button = e.OriginalSource as Button;
            switch (button.Tag)
            {
                case "Android":
                    passwordSource = _androidPasswords;
                    break;
                case "Camera":
                    passwordSource = _cameraPasswords;
                    break;
                case "Vault":
                    passwordSource = _vaultPasswords;
                    break;
            }

            if (CheckPassword(passwordSource, KeyPad.Password))
            {
                DisplaySuccess();
            }
            else
            {
                DisplayError();
            }
            KeyPad.ClearPassword();
        }

        private void DisplayError()
        {
            CloseErrorGridButton.IsEnabled = false;
            ErrorGrid.Visibility = Visibility.Visible;
            BluringGrid.Effect = new BlurEffect() {Radius = 12, KernelType = KernelType.Gaussian};
            
            var coolDownTask = Task.Run(() =>
            {
                for (int i = Properties.Settings.Default.wrongPasswordTimeout; i > 0; i--)
                {
                    var i1 = i;
                    Dispatcher.Invoke( ()=> CloseErrorGridButton.Content = i1.ToString(), DispatcherPriority.DataBind);
                    Thread.Sleep(1000);
                }

                Dispatcher.Invoke(() =>
                {
                    CloseErrorGridButton.Content = "OK";
                    CloseErrorGridButton.IsEnabled = true;
                });
            });
        }

        private void DisplaySuccess()
        {
            MessageBox.Show("Success");
        }
    }
}
