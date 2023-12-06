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
using System.Windows.Shapes;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.WpfDesign;
using System.IO;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Report.Units;
using Stimulsoft.Base.Services;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace UMSV.Stimul.Viewer
{
    public partial class TemplateDesignerForm : Window
    {
        #region Properties&Fields
        StiReport _StiReport = new StiReport();

        private bool Saved = false;

        public int _Id { get; set; }

        FacsimileTemplate CurrentReport
        {
            get;
            set;
        }
        StiDataSourcesCollection CurrentStiDataSources { get; set; }

        public FacsimileTemplate CurrentReportWrapper
        {
            get
            {
                return CurrentReport;
            }
        }

        private bool dialogResult = false;
        public bool UniversalDialogResult
        {
            get
            {
                return dialogResult;
            }
        }

        #endregion

        #region Constructor
        public TemplateDesignerForm(bool isNew, int templateID)
        {
            InitializeComponent();

            StiOptions.Engine.GlobalEvents.SavingReportInDesigner -= new Stimulsoft.Report.Design.StiSavingObjectEventHandler(GlobalEvents_SavingReportInDesigner);
            StiOptions.Engine.GlobalEvents.SavingReportInDesigner += new Stimulsoft.Report.Design.StiSavingObjectEventHandler(GlobalEvents_SavingReportInDesigner);


            if (!isNew)
            {
                CurrentReport = GetFacsimileTemplate(templateID);
                CurrentReport.Template = GetFacsimileTemplateFile(templateID);
            }
            else
            {
                CurrentReport = null;
                CurrentReport = new FacsimileTemplate();
            }
            InitiateTemplateDesigner(isNew);
        }

        #endregion

        #region Initialize
        #endregion

        #region Events
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            TemplateDesigner.CheckClosing(new System.ComponentModel.CancelEventArgs(true));
        }

        private void TemplateDesigner_Loaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        #region Methods
        private FacsimileTemplate GetFacsimileTemplate(int id)
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                return context.FacsimileTemplates
                              .Single(t => t.ID == id);
            }
        }


        private byte[] GetFacsimileTemplateFile(int id)
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                try
                {
                    return context.FacsimileTemplates.Where(t => t.ID == id).Select(t => t.Template).Single().ToArray();
                }
                catch
                {
                    return null;
                }
            }
        }

        private byte[] DeCompress(byte[] ComprressReport)
        {
            MemoryStream memoryStream = new MemoryStream(ComprressReport);
            GZipStream gzip = new GZipStream(memoryStream, CompressionMode.Decompress, true);
            StreamReader reader = new StreamReader(gzip);
            string text = reader.ReadToEnd();
            text = text + ">";
            return ASCIIEncoding.UTF8.GetBytes(text);
        }

        private void InitiateTemplateDesigner(bool isNew)
        {
            switch (isNew)
            {
                case false:
                    StiOptions.Engine.HideMessages = true;
                    _StiReport = new StiReport();
                    TemplateDesigner.Report = _StiReport;
                    TemplateDesigner.ShowPanelMessages = false;
                    _StiReport.Compile();
                    if (CurrentReport != null)
                        try
                        {
                            _StiReport.Load(CurrentReport.Template.ToArray());
                        }
                        catch (Exception)
                        {

                        }
                    break;
                case true:
                    StiOptions.Engine.HideMessages = true;
                    _StiReport = new StiReport();
                    TemplateDesigner.Report = _StiReport;
                    TemplateDesigner.ShowPanelMessages = false;
                    _StiReport.Compile();
                    break;
            }
        }

        private void GlobalEvents_SavingReportInDesigner(object sender, Stimulsoft.Report.Design.StiSavingObjectEventArgs e)
        {
            if (!Saved)
            {
                Saved = true;
                e.Processed = true;
                TemplateDesignerForm form = ((TemplateDesignerForm)(((System.Windows.Controls.ContentControl)(sender)).Parent));
                ((Stimulsoft.Report.WpfDesign.StiWpfDesignerControl)sender).Report.SaveToByteArray();
                Save(((Stimulsoft.Report.WpfDesign.StiWpfDesignerControl)sender).Report.SaveToByteArray());
            }
        }

        private void Save(byte[] rep)
        {
            //int id = CurrentReport.ID;
            SaveTemplateForm saveReportForm = new SaveTemplateForm(rep, _Id);
            saveReportForm.ReportID = _Id;
            bool? saveTemplateFormResult = saveReportForm.ShowDialog();
            if (saveTemplateFormResult.HasValue && saveTemplateFormResult.Value)
            {
                CurrentReport.ID = saveReportForm.CurrentReportWrapper.ID;
                CurrentReport.Title = saveReportForm.CurrentReportWrapper.Title;
                CurrentReport.Template = saveReportForm.CurrentReportWrapper.Template;
                dialogResult = true;
            }
        }

        #endregion

    }
}
