using System;
using System.Linq;
using eRxWeb.AppCode.Reports;
using Microsoft.Reporting.WebForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.ReportTests.ReportViewerHelperTests
{
    [TestClass]
    public class SetExportFormatVisibilityTests
    {
        //Fix the reference issue and uncomment this
        [TestMethod]
        public void Should_Not_Throw_Exception_If_Viewer_Is_Null()
        {
            try
            {
                ReportViewerHelper.SetExportFormatVisibility(null, ReportViewerHelper.ExportFormat.Excel, true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void Should_Set_Value_Of_Isvisible_Of_Designated_Format()
        {
            //arrange
            var report = new ReportViewer();
            var isvisible = false;
            var format = ReportViewerHelper.ExportFormat.Excel;
            //act
            ReportViewerHelper.SetExportFormatVisibility(report, format, isvisible);


            var exportoption = report.LocalReport.ListRenderingExtensions().ToList().Find(x => x.Name == format.ToString());
            //assert
            Assert.AreEqual(isvisible, exportoption.Visible);
        }
    }
}
