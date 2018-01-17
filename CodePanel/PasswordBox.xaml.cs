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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
