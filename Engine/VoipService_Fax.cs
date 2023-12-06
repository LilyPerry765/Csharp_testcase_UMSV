using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using Stimulsoft.Report.Export;
using System.IO;
using Stimulsoft.Report;
using System.ComponentModel;
using System.Collections;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        void TrackFaxNode(SipDialog dialog)
        {
            try
            {
                Schema.FaxNode node = dialog.CurrentNode.AsFaxNode;
                string templateName = node.TemplateName;
                int templateId = node.TemplateID;

                using (UmsDataContext context = new UmsDataContext())
                {
                    FacsimileTemplate template = context.FacsimileTemplates
                                                        .FirstOrDefault(t => t.ID == templateId);

                    if (template != null)
                    {
                        Stimulsoft.Report.StiReport stiReport = new Stimulsoft.Report.StiReport();

                        byte[] TemplateByteArray = template.Template.ToArray();
                        stiReport.Load(TemplateByteArray);

                        ArrayList argNames = new ArrayList();
                        List<string> args = node.Arg.Select(a => a.Name).ToList();

                        if (args != null)
                        {
                            foreach (string arg in args)
                            {
                                Logger.WriteInfo("({0}) -> Variable received: {1} = {2}", dialog.CallerID, arg, dialog.Keys.Trim('#').Trim('*').Trim());
                                stiReport.Dictionary.Variables[arg].Value = dialog.Keys.Trim('#').Trim('*').Trim();
                            }
                        }

                        if (node.ReadFromFile)
                        {
                            Stimulsoft.Report.Components.StiImage stiImage = (Stimulsoft.Report.Components.StiImage)stiReport.GetComponents().OfType<Stimulsoft.Report.Components.StiImage>().FirstOrDefault();
                            string fileNameByCallerId = (dialog.CallerID.Length > 8 ? dialog.CallerID.Substring(dialog.CallerID.Length - 8, 8) : dialog.CallerID) + ".tif";
                            stiImage.Image = System.Drawing.Image.FromFile(Path.Combine(UMSV.Schema.Config.Default.FaxRepository, fileNameByCallerId));
                        }

                        stiReport.CompileStandaloneReport("", Stimulsoft.Report.StiStandaloneReportType.ShowWithWpf);
                        stiReport.Render(true);

                        string fileName = Guid.NewGuid() + "_" + dialog.CallerID + ".tif";
                        string fullFileName = GetPath(fileName);

                        Stimulsoft.Base.StiFileUtils.ProcessReadOnly(fullFileName);
                        System.IO.FileStream stream = new System.IO.FileStream(fullFileName, System.IO.FileMode.Create);

                        new FaxTiffExportManager().Export(stiReport, stream, fullFileName);

                        Logger.WriteInfo("Fax export done successfully. Waiting for DIS to continue and complete signaling. Dialog Id: {0}", dialog.DialogID);

                        dialog.Extension = fullFileName;
                        Logger.WriteImportant(dialog.Extension);
                    }
                    else
                        Logger.WriteWarning("Template not found for the current fax node. dialog Id: {0}", dialog.DialogID);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        string GetPath(string fileName)
        {
            string[] paths = new string[]{
                UMSV.Schema.Config.Default.FaxDirectory,
                DateTime.Now.ToString("yyyy-MM-dd"),
                fileName
            };

            var path = Path.Combine(paths);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            return path;
        }
    }

    public class FaxTiffExportManager : StiTiffExportService
    {
        public void Export(StiReport stiReport, FileStream stream, string fileName)
        {
            Stimulsoft.Report.Components.StiPage page = stiReport.RenderedPages[0];

            StiResizeReportHelper.ResizeReport(stiReport, page.Orientation, System.Drawing.Printing.PaperKind.Letter,
                page.Margins, 21.95, 30.2, StiResizeReportOptions.ProcessAllPages);


            StiBitmapExportSettings settings = new StiBitmapExportSettings();
            settings.ImageZoom = 1;
            settings.ImageResolution = 200;
            settings.CutEdges = false;
            settings.ImageFormat = StiImageFormat.Grayscale;
            settings.PageRange = StiPagesRange.All;
            settings.MultipleFiles = false;
            settings.TiffCompressionScheme = StiTiffCompressionScheme.CCITT4;
            settings.DitheringType = StiMonochromeDitheringType.FloydSteinberg;

            //Only works with 2012 version of Stimul which is customized in Pendar...
            //base.StartExportInBackground(stiReport, stream, settings, false, false, fileName, Stimulsoft.Base.StiGuiMode.Wpf);
        }
    }


}
