using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using Pendar.Ums.Model;
using UMSV.Schema;

namespace UMSV.ViewModels
{
    public class SettingsViewModel : DataDrivenViewModel, IEditableForm
    {
        public SettingsViewModel()
        {
            Config.Load();
        }

        public bool Save()
        {
            Config.Save();
            return true;
        }

        public Config Config
        {
            get
            {
                return Config.Default;
            }
        }

        public IEnumerable<UMSV.Graph> Graphs
        {
            get
            {
                return DB.Graphs;
            }
        }

    }
}
