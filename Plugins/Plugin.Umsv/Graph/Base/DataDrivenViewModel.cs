using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pendar.Ums.Model;
using System.Windows.Input;
using Folder.Commands;
using UMSV;

namespace UMSV.ViewModels
{
    public class DataDrivenViewModel : ViewModelBase
    {
        #region DB

        private UmsDataContext db;
        protected UmsDataContext DB
        {
            get
            {
                if (db == null)
                {
                    db = new UmsDataContext();
                }
                return db;
            }

            set
            {
                db = value;
            }
        }

        #endregion

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new DelegateCommand(delegate
                    {
                        OnSave();
                    });
                }
                return saveCommand;
            }
        }

        protected virtual bool OnSave()
        {
            if (Validate())
            {
                DB.SubmitChanges();
                return true;
            }
            return false;
        }

        protected virtual bool Validate()
        {
            return true;
        }
    }
}
