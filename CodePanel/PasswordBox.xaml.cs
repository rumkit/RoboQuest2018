using System.Windows;
using System.Windows.Controls;

namespace CodePanel
{
    /// <summary>
    /// Interaction logic for PasswordBox.xaml
    /// </summary>
    public partial class PasswordBox : UserControl
    {
        public PasswordBox()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(PasswordBox), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
    }
}
