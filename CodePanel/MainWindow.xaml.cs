using System;
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

        Thread _supressPowerSavingThread;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _supressPowerSavingThread = new Thread(SupressPowerSavingRoutine) { IsBackground = true };
            _supressPowerSavingThread.Start();
        }

        // Prevents PC from going to powersaving
        private void SupressPowerSavingRoutine()
        {
            using (var powerManager = new PowerManager(ExecutionState.SystemRequired | ExecutionState.DisplayRequired, isContinuous: true))
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }
        }


        private List<string> _vaultPasswords = new List<string>(Properties.Settings.Default.vaultPasswords);
        private List<string> _androidPasswords = new List<string>(Properties.Settings.Default.androidPasswords);
        private List<string> _cameraPasswords = new List<string>(Properties.Settings.Default.cameraPasswords);

        // 1 Tick is 0.1s
        public int TicksToWait => Properties.Settings.Default.wrongPasswordTimeout * 10;

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

        private void SuccessClose_OnClick(object sender, RoutedEventArgs e)
        {
            SuccessGrid.Visibility = Visibility.Collapsed;
            BluringGrid.Effect = null;
        }

        // Choose action from button's tag, checks password and displays result
        private void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            IList<string> passwordSource = null;
            var button = e.OriginalSource as Button;
            string successMessage = string.Empty;
            switch (button.Tag)
            {
                case "Android":
                    passwordSource = _androidPasswords;
                    successMessage = Properties.Settings.Default.androidSuccessMessage;
                    break;
                case "Camera":
                    passwordSource = _cameraPasswords;
                    successMessage = Properties.Settings.Default.cameraSuccessMessage;
                    break;
                case "Vault":
                    successMessage = Properties.Settings.Default.vaultSuccessMessage;
                    passwordSource = _vaultPasswords;
                    break;
            }

            if (CheckPassword(passwordSource, KeyPad.Password))
            {
                DisplayProgress(()=>DisplaySuccess(successMessage));
            }
            else
            {
                DisplayProgress(DisplayError);
            }
            KeyPad.ClearPassword();
        }

        // Displays fake progress until progressbar is full, then calls continueWith callback
        private void DisplayProgress(Action continueWith)
        {
            BluringGrid.Effect = new BlurEffect() { Radius = 12, KernelType = KernelType.Gaussian };
            Task.Run(() =>
            {
                Dispatcher.Invoke(() => ProgressGrid.Visibility = Visibility.Visible);
                for (int i = 0; i < TicksToWait; i++)
                {
                    int i1 = i;
                    Dispatcher.Invoke(() => ConnectionProgressBar.Value = i1, DispatcherPriority.DataBind);
                    Thread.Sleep(100);

                }

                Dispatcher.Invoke(() => ConnectionProgressBar.Value = 0);
                Dispatcher.Invoke(() => ProgressGrid.Visibility = Visibility.Hidden);
                continueWith();
            });
        }

        private void DisplayError()
        {
            Dispatcher.Invoke(() =>
            {
                ErrorGrid.Visibility = Visibility.Visible;
            });
        }

        private void DisplaySuccess(string successMessage)
        {
            var messages = successMessage.Split(new[] {'/'}, StringSplitOptions.None);
            Dispatcher.Invoke(() =>
            {
                SuccessMessage1.Text = messages[0];
                SuccessMessage2.Text = messages[1];
                SuccessGrid.Visibility = Visibility.Visible;
            });
        }
    }
}
