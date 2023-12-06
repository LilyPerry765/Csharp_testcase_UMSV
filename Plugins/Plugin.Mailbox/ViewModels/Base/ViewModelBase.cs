using System.ComponentModel;
using System.Windows.Input;

namespace Plugin.Mailbox.ViewModels
{
    /// <summary>
    /// Provides common functionality for ViewModel classes
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SendPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SendPropertyChanged()
        {
            SendPropertyChanged(null);
        }

        private bool? dialogResult;
        public bool? DialogResult
        {
            get
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value;
                SendPropertyChanged("DialogResult");
            }
        }

        private Cursor cursor;
        public Cursor Cursor
        {
            get
            {
                if (cursor == null)
                {
                    cursor = Cursors.Arrow;
                }
                return cursor;
            }
            set
            {
                cursor = value;
                SendPropertyChanged("Cursor");
            }
        }

    }
}
