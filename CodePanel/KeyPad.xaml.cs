using System;
using System.Windows;
using System.Windows.Controls;

namespace CodePanel
{
    /// <summary>
    /// Interaction logic for KeyPad.xaml
    /// </summary>
    public partial class KeyPad : UserControl
    {
        public KeyPad()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        #region dependency props

        public static readonly DependencyProperty MaxPasswordLengthProperty = DependencyProperty.Register(
            "MaxPasswordLength", typeof(int), typeof(KeyPad), new PropertyMetadata(4));

        public int MaxPasswordLength
        {
            get { return (int) GetValue(MaxPasswordLengthProperty); }
            set { SetValue(MaxPasswordLengthProperty, value); }
        }

        public static readonly DependencyPropertyKey PasswordPropertyKey = DependencyProperty.RegisterReadOnly(
            "Password", typeof(string), typeof(KeyPad), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PasswordProperty
            = PasswordPropertyKey.DependencyProperty;

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            private set { SetValue(PasswordPropertyKey, value); }
        }

        public static readonly DependencyProperty OKButtonVisibleProperty = DependencyProperty.Register(
            "OKButtonVisible", typeof(bool), typeof(KeyPad), new PropertyMetadata(true));

        public bool OKButtonVisible
        {
            get { return (bool) GetValue(OKButtonVisibleProperty); }
            set { SetValue(OKButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty DeleteButtonVisibleProperty = DependencyProperty.Register(
            "DeleteButtonVisible", typeof(bool), typeof(KeyPad), new PropertyMetadata(true));

        public bool DeleteButtonVisible
        {
            get { return (bool) GetValue(DeleteButtonVisibleProperty); }
            set { SetValue(DeleteButtonVisibleProperty, value); }
        }

        #endregion

       
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            ProcessInput(button.Content.ToString());
        }

        // handles all buttons' input
        private void ProcessInput(string input)
        {
            switch (input)
            {
                case "C":
                    RemoveSymbol();
                    break;
                case "OK":
                    MessageBox.Show(Password);
                    break;
                default:
                    AddSymbol(input);
                    break;
            }
        }

        // removes 1 symbol from end
        private void RemoveSymbol()
        {
            if (Password.Length > 0)
                Password = Password.Remove(Password.Length - 1);
        }

        // adds one symbol to end
        private void AddSymbol(string input)
        {
            if (Password.Length < MaxPasswordLength)
                Password = Password.Insert(Password.Length, input);
        }

        public void ClearPassword()
        {
            Password = String.Empty;
        }
        
    }
}
