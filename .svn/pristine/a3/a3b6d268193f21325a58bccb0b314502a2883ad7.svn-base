using System.Windows.Controls;

namespace UMSV.BasicNodes
{

    public partial class GetKeyNodeResultView : UserControl
    {
        public GetKeyNodeResultView()
        {
            InitializeComponent();
        }

        private void ComboBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!(comboBox.ItemsSource as string).Contains(e.Text))
                e.Handled = true;
        }

    }
}
