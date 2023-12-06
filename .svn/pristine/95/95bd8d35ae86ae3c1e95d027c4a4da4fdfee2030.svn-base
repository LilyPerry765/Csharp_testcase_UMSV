using System.Collections.Generic;
using System.Linq;
using UMSV;
using Folder.Commands;
using UMSV.ViewModels;

namespace UMSV.ModelViews
{
    public class SmsSendListViewModel : DataDrivenViewModel
    {
        public IEnumerable<Informing> Messages
        {
            get
            {
                return DB.Informings.Where(i => i.Type == (byte)Pendar.Ums.Model.Enums.InformingType.SMS);
            }
        }

        public Informing SelectedMessage
        {
            get;
            set;
        }

        private void RefreshMessages()
        {
            DB = new UmsDataContext();
            SendPropertyChanged("Messages");
        }

        #region Commands

        private DelegateCommand _AddCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new DelegateCommand(delegate
                    {
                        SmsSendView dlg = new SmsSendView()
                          {
                              DataContext = new SmsSendViewModel()
                          };

                        if (dlg.ShowDialog() == true)
                            RefreshMessages();
                    });
                }
                return _AddCommand;
            }
        }

        private DelegateCommand _EditCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                {
                    _EditCommand = new DelegateCommand(delegate
                    {
                        SmsSendView dlg = new SmsSendView()
                        {
                            DataContext = new SmsSendViewModel(SelectedMessage)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshMessages();
                    },
                    delegate
                    {
                        return SelectedMessage != null;
                    });
                }
                return _EditCommand;
            }
        }

        private DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new DelegateCommand(delegate
                    {
                        if (Folder.MessageBox.ShowQuestion("اطلاعات انتخاب شده حذف شود؟") == System.Windows.MessageBoxResult.Yes)
                        {
                            DB.Informings.DeleteOnSubmit(SelectedMessage);
                            DB.SubmitChanges();
                            RefreshMessages();
                        }
                    },
                    delegate
                    {
                        return SelectedMessage != null;
                    });
                }
                return _DeleteCommand;
            }
        }

        private DelegateCommand showNumbersCommand;
        public DelegateCommand ShowNumbersCommand
        {
            get
            {
                if (showNumbersCommand == null)
                {
                    showNumbersCommand = new DelegateCommand(delegate
                    {
                        SmsSendView dlg = new SmsSendView()
                        {
                            DataContext = new SmsSendViewModel(SelectedMessage, 1)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshMessages();
                    },
                    delegate
                    {
                        return SelectedMessage != null;
                    });
                }
                return showNumbersCommand;
            }
        }

        private DelegateCommand _ShowScheduleCommand;
        public DelegateCommand ShowScheduleCommand
        {
            get
            {
                if (_ShowScheduleCommand == null)
                {
                    _ShowScheduleCommand = new DelegateCommand(delegate
                    {
                        SmsSendView dlg = new SmsSendView()
                        {
                            DataContext = new SmsSendViewModel(SelectedMessage, 2)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshMessages();
                    },
                    delegate
                    {
                        return SelectedMessage != null;
                    });
                }
                return _ShowScheduleCommand;
            }
        }

        #endregion

    }
}
