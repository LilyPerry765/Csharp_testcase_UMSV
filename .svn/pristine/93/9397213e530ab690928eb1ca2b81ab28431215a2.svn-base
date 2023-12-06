using System.Windows;
using System.Windows.Controls;

namespace Pendar.Ums.CompositeNodes.UserControls
{
    /// <summary>
    /// Interaction logic for PasswordBox.xaml
    /// </summary>
    public partial class PasswordBox : UserControl
    {
        private bool canChange = true;

        public PasswordBox()
        {
            InitializeComponent();
        }

        private void internalPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (canChange)
            {
                canChange = false;
                Password = internalPasswordBox.Password;
                canChange = true;
            }
        }

        public string Password
        {
            get
            {
                return (string)GetValue(PasswordProperty);
            }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBox), new FrameworkPropertyMetadata(OnPasswordChanged)
            {
                BindsTwoWayByDefault = true
            });

        protected static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox me = sender as PasswordBox;
            if (me.canChange)
            {
                me.canChange = false;
                me.internalPasswordBox.Password = e.NewValue as string;
                me.canChange = true;
            }
        }

    }
}
