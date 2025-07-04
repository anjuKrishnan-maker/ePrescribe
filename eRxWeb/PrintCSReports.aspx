<%@ Page Language="C#" AutoEventWireup="true" Title="Veradigm ePrescribe CS Reports Print Preview" 
    Inherits="Allscripts.Web.UI.PrintCSReports" Codebehind="PrintCSReports.aspx.cs" %>

<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Veradigm ePrescribe CS Reports Print Preview</title>
</head>
    <script language="javascript" type="text/javascript">
        function printPdf(Url) {
            location.href = Url;
        }        
      </script>

<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="masterScriptManager" runat="server" EnablePartialRendering="true" ScriptMode="Debug" EnablePageMethods="true" EnableScriptGlobalization="true">
		</asp:ScriptManager>
        <div class="divReportWrapper">            

             <rsweb:ReportViewer ID="rptvwrCSReportPrint" runat="server" Width="100%" Height="1000px" AsyncRendering="False" Font-Names="Verdana" ShowZoomControl="false"
                                                Font-Size="8pt" ShowPrintButton="false" ShowExportControls="false">
                                                </rsweb:ReportViewer>
                      
        </div>
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
