using System;
using System.IO;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using ExpertPdf.HtmlToPdf;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.ePrescribeSvc;
using ExpertPdf;
namespace eRxWeb
{
public partial class PrintActivationCode : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // transform xls into html
        string xslPath = Server.MapPath("Scripts/ActivationCode.xslt");
        string emailBody = string.Empty;
        PdfConverter pdfconverter = new PdfConverter { LicenseKey = eRxWeb.AppCode.ConfigKeys.PdfLicenseKey };

        StringWriter writer = new StringWriter();
        XsltArgumentList xslArg = new XsltArgumentList();
        XslCompiledTransform transformContent = new XslCompiledTransform();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("<Root><Name></Name><ActivationCodeURL></ActivationCodeURL><ActivationCode></ActivationCode></Root>");

        xslArg.AddParam("name", "", Session["ActivationCodeName"].ToString());
		xslArg.AddParam("shieldActivationCodeURL", "", string.Concat(ConfigurationManager.AppSettings["appurl"], "/register/activateuser"));
        xslArg.AddParam("activationCode", "", Session["ActivationCode"].ToString());
        xslArg.AddParam("shieldHelpURL", "", ConfigKeys.ShieldHelpURL);

        transformContent.Load(xslPath);
        transformContent.Transform(xmlDoc, xslArg, writer);

            var bytes = pdfconverter.GetPdfBytesFromHtmlString(writer.ToString());
            Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-length", bytes.Length.ToString());
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }
}
}