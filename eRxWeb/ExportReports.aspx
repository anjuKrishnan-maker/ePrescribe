<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.ExportReports" Codebehind="ExportReports.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export Reports</title>
    <link href="Style/Style.css" rel="stylesheet" type="text/css" />
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <form id="form1" runat="server">
    <table width="100%">
        <tr>
            <td class="h2title" colspan="4">
                <asp:ScriptManager ID="masterScriptManager" runat="server" EnablePartialRendering="true"
                    ScriptMode="Release">
                </asp:ScriptManager>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" ClientEvents-OnRequestStart="requestStart">
                    <ClientEvents OnRequestStart="requestStart" />
                    <AjaxSettings>
                        <telerik:AjaxSetting AjaxControlID="grdExportToExcel">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdExportToExcel"  />
                                <telerik:AjaxUpdatedControl ControlID="lblGridResultsStatus" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                    </AjaxSettings>
                </telerik:RadAjaxManager>
                <telerik:RadGrid ID="grdExportToExcel" AllowSorting="True" AllowPaging="True" PageSize="10"
                    runat="server" GridLines="None" Width="95%">
                    <ExportSettings HideStructureColumns="true" />
                    <MasterTableView Width="100%" CommandItemDisplay="Top">
                        <PagerStyle Mode="NextPrevNumericAndAdvanced" />
                        <CommandItemSettings ShowExportToWordButton="true" ShowExportToExcelButton="true"
                            ShowExportToCsvButton="true" />
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %></body>
</html>
