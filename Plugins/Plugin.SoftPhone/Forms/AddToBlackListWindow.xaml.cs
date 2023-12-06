using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Folder;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for UserLoginWindow.xaml
    /// </summary>
    public partial class AddToBlackListWindow : Window
    {
        public AddToBlackListWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                var phone = new SpecialPhone()
                {
                    UserID = User.Current.ID,
                    Number = NumberTextbox.Text,
                    Comment = CommentTextbox.Text,
                    RegisterTime = new FolderDataContext().GetDate().Value,
                    Type = (int)SpecialPhoneType.TemporaryBlackList,
                };
                dc.SpecialPhones.InsertOnSubmit(phone);
                dc.SubmitChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
