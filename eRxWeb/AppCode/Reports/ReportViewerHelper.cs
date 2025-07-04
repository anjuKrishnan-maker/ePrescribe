using System.Reflection;
using Microsoft.Reporting.WebForms;

namespace eRxWeb.AppCode.Reports
{
    public class ReportViewerHelper
    {
        public enum ExportFormat
        {
            Excel,
            PDF
        }

        public static void SetExportFormatVisibility(ReportViewer viewer, ExportFormat format, bool isVisible)
        {
            if (viewer != null)
            {
                foreach (var exportOption in viewer.LocalReport.ListRenderingExtensions())
                {
                    if (exportOption.Name == format.ToString())
                    {
                        var info = exportOption.GetType().GetField("m_isVisible", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (info != null)
                        {
                            info.SetValue(exportOption, isVisible);
                            break;
                        }
                    }
                }
            }
        }
    }
   
}
