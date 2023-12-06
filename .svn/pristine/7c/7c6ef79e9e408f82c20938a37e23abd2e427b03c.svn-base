using System.Linq;
using UMSV;
using System.Threading;

namespace UMSV.ViewModels
{
    public class SmsSendViewModel : InformingViewModel
    {

        #region Constructors

        public SmsSendViewModel()
            : this(null)
        {
        }

        public SmsSendViewModel(Informing sms, int defaultTab = 0)
        {
            if (sms == null)
            {
                IsNew = true;
                this.Informing = new Informing()
                {
                    Enabled = true,
                    Type = (byte)Pendar.Ums.Model.Enums.InformingType.SMS
                };
                DB.Informings.InsertOnSubmit(this.Informing);
            }
            else
                this.Informing = DB.Informings.Single(i => i == sms);
            SelectedTabIndex = defaultTab;
        }

        #endregion

        protected override bool OnSave()
        {
            RemoveInvalidRecords();
            if (Validate())
            {
                Informing.CallTime = Informing.Schedule.ToXElement();
                CancelImport = true;
                new Thread(delegate()
                {
                    DB.SubmitChanges();
                    // TODO: complete this
                    // EngineInteropClient.RestartAllInformingsForAllEngines();
                }).Start();
                IsNew = false;
                return true;
            }
            return false;
        }

        protected override bool Validate()
        {
            if (Informing.Subject == null)
            {
                Folder.MessageBox.ShowError("لطفا عنوان را وارد نماييد.");
                return false;
            }
            return true;
        }



    }
}
