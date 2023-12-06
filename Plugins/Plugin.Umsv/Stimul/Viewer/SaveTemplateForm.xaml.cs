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
using System.IO;
using System.IO.Compression;
using Folder;
using System.ComponentModel;

namespace UMSV.Stimul.Viewer
{
    public partial class SaveTemplateForm : Window
    {
        #region Properties

        public int? ReportID { get; set; }

        //public string TemplateName
        //{
        //    get
        //    {
        //        return CurrentReport == null ? string.Empty : CurrentReport.Title;
        //    }
        //}

        //public int TemplateID
        //{
        //    get
        //    {
        //        return CurrentReport == null ? 0 : CurrentReport.ID;
        //    }
        //}

        public FacsimileTemplate CurrentReportWrapper
        {
            get
            {
                return this.CurrentReport;
            }
        }

        static readonly DependencyProperty DependencyReport = DependencyProperty.Register("CurrentReport", typeof(FacsimileTemplate), typeof(SaveTemplateForm));

        FacsimileTemplate CurrentReport
        {
            get
            {
                return (FacsimileTemplate)GetValue(DependencyReport);

            }
            set
            {
                SetValue(DependencyReport, value);
            }

        }
        #endregion

        #region constructor
        public SaveTemplateForm(byte[] reportFile, int? reportID)
        {
            InitializeComponent();
            if (reportID == null || reportID == -1)
            {
                CurrentReport = new FacsimileTemplate();
            }
            else if (reportID != 0)
            {
                CurrentReport = GetFacsimileTemplate(reportID ?? -1);
                this.ReportID = reportID;
            }
            else
            {
                CurrentReport = new FacsimileTemplate()
                {
                    Template = reportFile,
                };
                this.ReportID = 0;
            }

            if (CurrentReport != null)
                CurrentReport.Template = reportFile;

            this.DataContext = this;
        }
        #endregion

        #region Events

        private void SaveReport(object sender, RoutedEventArgs e)
        {
            //this.ReportID = CurrentReport.ID;

            if (ReportID == 0)
                using (UmsDataContext context = new UmsDataContext())
                {
                    FacsimileTemplate fax = new FacsimileTemplate()
                    {
                        Template = CurrentReport.Template,
                        Title = CurrentReport.Title,
                        Description = CurrentReport.Description
                    };

                    context.FacsimileTemplates.InsertOnSubmit(fax);
                    context.SubmitChanges();

                    CurrentReport.ID = fax.ID;
                }
            else if (ReportID.HasValue && ReportID != -1)
                using (UmsDataContext context = new UmsDataContext())
                {
                    FacsimileTemplate fax = context.FacsimileTemplates.Single(t => t.ID == ReportID);
                    fax.Template = CurrentReport.Template;
                    fax.Title = CurrentReport.Title;
                    fax.Description = CurrentReport.Description;

                    context.SubmitChanges();

                    CurrentReport.ID = fax.ID;
                }

            this.DialogResult = true;

        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Methods

        private FacsimileTemplate GetFacsimileTemplate(int id)
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                return context.FacsimileTemplates
                              .SingleOrDefault(t => t.ID == id);
            }
        }

        private void ZipReport()
        {
            byte[] data = new byte[CurrentReport.Template.Length + 10];
            data = CurrentReport.Template.ToArray();
            StreamReader reader = new StreamReader(new MemoryStream(data));
            string text = reader.ReadToEnd();
            text = text + (char)(26);
            MemoryStream memoryStream = new MemoryStream();
            GZipStream gZip = new System.IO.Compression.GZipStream(memoryStream, CompressionMode.Compress, true);
            gZip.Write(ASCIIEncoding.UTF8.GetBytes(text), 0, (ASCIIEncoding.UTF8.GetBytes(text).Length));
            CurrentReport.Template = memoryStream.ToArray();
        }

        #endregion
    }
}
