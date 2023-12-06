using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using UMSV;
using Folder.Commands;
using Folder;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;


namespace UMSV.ViewModels
{
    public class PhoneCallsListViewModel : DataDrivenViewModel
    {

        public IEnumerable<Informing> PhoneCalls
        {
            get
            {
                return DB.Informings.Where(i => i.Type == (byte)InformingType.Graph);
            }
        }

        public Informing SelectedPhoneCall
        {
            get;
            set;
        }

        private void RefreshPhoneCalls()
        {
            DB = new UmsDataContext();
            SendPropertyChanged("PhoneCalls");
        }

        #region Commands

        private DelegateCommand showGraphCommand;
        public DelegateCommand ShowGraphCommand
        {
            get
            {
                if (showGraphCommand == null)
                {
                    showGraphCommand = new DelegateCommand(delegate
                    {
                        SelectGraphViewModel selectedGraphViewModel = new SelectGraphViewModel(SelectedPhoneCall.ID);
                        if (SelectedPhoneCall.Graph != null)
                            selectedGraphViewModel.SelectedGraphIDForInforming = SelectedPhoneCall.Graph;

                        SelectGraphView dlg = new SelectGraphView(selectedGraphViewModel);

                        if (dlg.ShowDialog() == true)
                            RefreshPhoneCalls();
                    },
                    delegate
                    {
                        return SelectedPhoneCall != null;
                    });
                }
                return showGraphCommand;
            }
        }

        private DelegateCommand setVoiceCommand;
        public DelegateCommand SetVoiceCommand
        {
            get
            {
                if (setVoiceCommand == null)
                {
                    setVoiceCommand = new DelegateCommand(delegate
                        {
                            OpenFileDialog dlg = new OpenFileDialog()
                            {
                                Multiselect = false,
                                Filter = "Wav files(*.wav)|*.wav"
                            };

                            if (dlg.ShowDialog() == true)
                            {
                                FileInfo f = new FileInfo(dlg.FileName);

                                Informing informing = DB.Informings.Where(t => t.ID == SelectedPhoneCall.ID).SingleOrDefault();
                                informing.VoiceName = f.Name;
                                informing.VoiceData = System.IO.File.ReadAllBytes(f.FullName);
                            }

                            DB.SubmitChanges();

                            RefreshPhoneCalls();
                        },
                        delegate
                        {
                            return SelectedPhoneCall != null;
                        });
                }

                return setVoiceCommand;
            }
        }

        private DelegateCommand _AddCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new DelegateCommand(delegate
                    {
                        PhoneCallView dlg = new PhoneCallView()
                        {
                            DataContext = new PhoneCallViewModel()
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshPhoneCalls();
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
                        PhoneCallView dlg = new PhoneCallView()
                        {
                            DataContext = new PhoneCallViewModel(SelectedPhoneCall)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshPhoneCalls();
                    },
                    delegate
                    {
                        return SelectedPhoneCall != null;
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
                            DB.Informings.DeleteOnSubmit(SelectedPhoneCall);
                            DB.SubmitChanges();
                            RefreshPhoneCalls();
                        }
                    },
                    delegate
                    {
                        return SelectedPhoneCall != null;
                    });
                }
                return _DeleteCommand;
            }
        }

        private DelegateCommand _StartAllCommand;
        public DelegateCommand StartAllCommand
        {
            get
            {
                if (_StartAllCommand == null)
                {
                    _StartAllCommand = new DelegateCommand(delegate
                    {
                        if (Folder.MessageBox.ShowQuestion("آیا از راه اندازی همه ارسال ها اطمینان دارید؟") == System.Windows.MessageBoxResult.Yes)
                            VoIPServiceClient_Plugin_UMSV.Default.StartScheduledOutcall();
                    });
                }
                return _StartAllCommand;
            }
        }

        private DelegateCommand _StopAllCommand;
        public DelegateCommand StopAllCommand
        {
            get
            {
                if (_StopAllCommand == null)
                {
                    _StopAllCommand = new DelegateCommand(delegate
                    {
                        if (Folder.MessageBox.ShowQuestion("آیا از توقف همه ارسال ها اطمینان دارید؟") == System.Windows.MessageBoxResult.Yes)
                            VoIPServiceClient_Plugin_UMSV.Default.StopScheduledOutcall();
                    });
                }
                return _StopAllCommand;
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
                        PhoneCallView dlg = new PhoneCallView()
                        {
                            DataContext = new PhoneCallViewModel(SelectedPhoneCall)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshPhoneCalls();
                    },
                    delegate
                    {
                        return SelectedPhoneCall != null;
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
                        PhoneCallView dlg = new PhoneCallView()
                        {
                            DataContext = new PhoneCallViewModel(SelectedPhoneCall, 1)
                        };

                        if (dlg.ShowDialog() == true)
                            RefreshPhoneCalls();
                    },
                    delegate
                    {
                        return SelectedPhoneCall != null;
                    });
                }
                return _ShowScheduleCommand;
            }
        }

        #endregion


    }
}
