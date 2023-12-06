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
using Folder;
using System.ComponentModel;

namespace UMSV.Forms
{
    /// <summary>
    /// Interaction logic for DashboardPanel.xaml
    /// </summary>
    public partial class DashboardPanel : UserControl
    {
        public DashboardPanel()
        {
            InitializeComponent();
        }

        void UserControl_Initialized(object sender, EventArgs e)
        {
            for (int index = 0; index <= 60 * 12; index += 10)
            {
                Line line = new Line() {
                    X1 = index + 15,
                    Y1 = 0,
                    X2 = index + 15,
                    Y2 = 330,
                    Stroke = index % 60 == 0 ? Brushes.LightGray : new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)),
                };
                CallsPanel.Children.Add(line);
            }

            for (int index = 0; index < 330; index += 60)
            {
                Line line = new Line() {
                    X1 = 0,
                    Y1 = index + 15,
                    X2 = 780,
                    Y2 = index + 15,
                    Stroke = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255)),
                };
                CallsPanel.Children.Add(line);
            }

            for (int index = 0; index <= 60 * 12; index += 60)
            {
                Label timeLabel = new Label() {
                    Margin = new Thickness(index-5, 325, 0, 0),
                    Foreground = Brushes.White,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                };
                CallsPanel.Children.Add(timeLabel);
                Lables.Add(timeLabel);
            }
        }

        List<Label> Lables = new List<Label>();

        void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (e.NewValue as DashboardPanelModel).PropertyChanged += new PropertyChangedEventHandler(DashboardPanel_PropertyChanged);
        }

        void DashboardPanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OnlineOperators":
                    Dispatcher.Invoke((Action)(() => {
                        DialogsGauge.OnApplyTemplate();
                    }));
                    break;

                case "TimeLables":
                    Dispatcher.Invoke((Action)(() => {
                        var items = (this.DataContext as DashboardPanelModel).TimeLables;
                        for (int index = 0; index < items.Count; index++)
                        {
                            Lables[index].Content = items[index];
                        }
                    }));
                    break;
            }
        }
    }

    public class NumberMultipleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var multiple = decimal.Parse(parameter.ToString());
            return (int)((int)value * multiple);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
