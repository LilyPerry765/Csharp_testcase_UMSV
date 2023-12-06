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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pendar.Ums.CompositeNodes.UserControls
{
    /// <summary>
    /// Interaction logic for TemplateManager.xaml
    /// </summary>
    public partial class TemplateManager : UserControl
    {

        public string TemplateName
        {
            get { return (string)GetValue(TemplateNameProperty); }
            set { SetValue(TemplateNameProperty, value); }
        }

        public static readonly DependencyProperty TemplateNameProperty =
            DependencyProperty.Register("TemplateName", typeof(string), typeof(TemplateManager), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTemplateNameChanged))
            {
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                BindsTwoWayByDefault = true
            });


        protected static void OnTemplateNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateManager templateManager = sender as TemplateManager;
            if (e.Property == TemplateNameProperty)
            {
                templateManager.TemplateNameLabel.Content = (string)e.NewValue;
            }
        }


        public int TemplateID
        {
            get { return (int)GetValue(TemplateIDProperty); }
            set { SetValue(TemplateIDProperty, value); }
        }

        public static readonly DependencyProperty TemplateIDProperty =
            DependencyProperty.Register("TemplateID", typeof(int), typeof(TemplateManager), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTemplateIDChanged))
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        protected static void OnTemplateIDChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //TemplateManager templateManager = sender as TemplateManager;
            //if (e.Property == TemplateNameProperty)
            //{
            //    templateManager.TemplateNameLabel.Content = (string)e.NewValue;
            //}
        }

        public TemplateManager()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UMSV.Stimul.Viewer.TemplateDesignerForm templateDesigner = new UMSV.Stimul.Viewer.TemplateDesignerForm(true, 0);
            templateDesigner._Id = 0;

            bool? templateDesignerResult = templateDesigner.ShowDialog();

            if (templateDesigner.UniversalDialogResult)
            {
                TemplateName = templateDesigner.CurrentReportWrapper.Title;
                TemplateID = templateDesigner.CurrentReportWrapper.ID;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            UMSV.Stimul.Viewer.TemplateDesignerForm templateDesigner = new UMSV.Stimul.Viewer.TemplateDesignerForm(false, TemplateID);
            templateDesigner._Id = TemplateID;

            bool? templateDesignerResult = templateDesigner.ShowDialog();

            if (templateDesigner.UniversalDialogResult)
            {
                TemplateName = templateDesigner.CurrentReportWrapper.Title;
                TemplateID = templateDesigner.CurrentReportWrapper.ID;
            }

        }

    }
}
