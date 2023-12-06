using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SynchronizeService.Services
{
    partial class SynchronizeService : ServiceBase
    {
        public SynchronizeService()
        {
            InitializeComponent();
        }

        Codes.SyncTimer syncTimer = new Codes.SyncTimer();

        protected override void OnStart(string[] args)
        {
            syncTimer.Start();
        }

        protected override void OnStop()
        {
            syncTimer.Stop();
        }
    }
}
