<%@ Page Language="C#" Title="CS Reports" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="Allscripts.Web.UI.EPCSReport" Codebehind="EPCSReport.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <style>
 body, div, span, p, td
{
	font-weight: normal;
    color: Black;
	margin: 0px;
	vertical-align:middle;
}

</style>

    <script language="javascript" type="text/javascript">
        function CheckPrint() {            
            var newUrlString = window.location.href.replace("EPCSReport.aspx", "PrintCSReports.aspx");
            window.open(newUrlString, '_blank', 'height=700,width=750,left=100,top=100,titlebar=no,resizable=yes,scrollbars=yes,toolbar=no,status=no');            
            return false;
        }
        </script>         
    <div>
        <table width="100%"  border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td>
                    <table border="0" width="100%" cellspacing="0" cellpadding="0" bordercolor="#b5c4c4">
                        <tr class="h1title">
                            <td class="auto-stylewh">
                            </td>
                        </tr>
                        <tr class="h2title">
                            <td class="auto-stylew">
                                <asp:Button ID="btnBack" runat="server" CssClass="btnstyle" OnClick="btnBack_Click"
                                    Text="Back" Visible="true" />
                                    <asp:Button id="btnPrint" runat="server" Text="Print" CssClass="btnstyle" />
                            </td>
                        </tr>
                        <tr>
                            <td class="auto-stylew">
                                <table cellspacing="0" cellpadding="0" height="100%" width="100%" border="0" style="margin: 0px;
                                    overflow: auto">
                                    <tr>
                                        <td>
                                            <div class="divReportWrapper" style="position: absolute;">                                              
                                                <rsweb:ReportViewer ID="rptvwrCSReports" runat="server" Height="400px" Width="100%" AsyncRendering="False" Font-Names="Verdana" ShowZoomControl="false"
                                                Font-Size="8pt" ShowPrintButton="false" ShowExportControls="true" PageCountMode="Actual">
                                                </rsweb:ReportViewer>
                                                 <asp:ObjectDataSource ID="epcsAdhocReport" runat="server" SelectMethod="GetAdhocReport" TypeName="Allscripts.Impact.EpcsReport">
                                                  <SelectParameters>
                                                <asp:SessionParameter Name="prescriberId" SessionField="UserID" Type="object"></asp:SessionParameter>
                                                <asp:SessionParameter Name="startDate" SessionField="EPCSStartDate" Type="object"></asp:SessionParameter>
                                                <asp:SessionParameter Name="endDate" SessionField="EPCSEndDate" Type="object"></asp:SessionParameter>   
                                                <asp:SessionParameter Name="timezone" SessionField="TimeZone" Type="object"></asp:SessionParameter>                                                 
                                                <asp:Parameter Name="ShieldUserName" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                                                <asp:Parameter Name="ePrescribeUserName" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                                                  </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:ObjectDataSource ID="AdhocCurrentAsOf" runat="server" SelectMethod="GetCurrentAsOfLocal" TypeName="Allscripts.Impact.EpcsReport">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="timezone" SessionField="TimeZone" Type="object"></asp:SessionParameter>  
                                                </SelectParameters>
                                                </asp:ObjectDataSource>
                                                 <asp:ObjectDataSource ID="EpcsMonthlyReportDs" runat="server" SelectMethod="GetMonthlyReport" TypeName="Allscripts.Impact.EpcsReport">
                                                  <SelectParameters>
                                                <asp:QueryStringParameter Name="reportExecutionId" QueryStringField="ID" />                                                  
                                                <asp:SessionParameter Name="timezone" SessionField="TimeZone" Type="object"></asp:SessionParameter>  
                                                  </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:ObjectDataSource ID="EpcsReporExecutionTimeTable" runat="server" SelectMethod="GetMonthlyReportGenerationTime" TypeName="Allscripts.Impact.EpcsReport">
                                                <SelectParameters>
                                                    <asp:QueryStringParameter Name="reportExecutionId" QueryStringField="ID" />                                                  
                                                <asp:SessionParameter Name="timezone" SessionField="TimeZone" Type="object"></asp:SessionParameter>  
                                                </SelectParameters>
                                                </asp:ObjectDataSource>
                                                 <asp:ObjectDataSource ID="EpcsDailyReportDs" runat="server" SelectMethod="GetEpcsDailyActivityReport" TypeName="Allscripts.Impact.EpcsReport">
                                                  <SelectParameters>
                                                <asp:SessionParameter Name="UserId" SessionField="UserID" Type="object"></asp:SessionParameter>                                                 
                                                <asp:SessionParameter Name="createDate" SessionField="EPCSStartDate" Type="object"></asp:SessionParameter>
                                                <asp:SessionParameter Name="timezone" SessionField="TimeZone" Type="object"></asp:SessionParameter>  
                                                <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object"></asp:SessionParameter>  
                                                  </SelectParameters>
                                                </asp:ObjectDataSource>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                        <td class="auto-stylewh1"></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>                                                
    </div>
    
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   
</asp:Content>
<asp:Content ID="Content3" runat="server" contentplaceholderid="HeaderPlaceHolder">
    <style type="text/css">
        .auto-stylewh {
            height: 20px;
            width: 1125px;
        }
        .auto-stylew {
            width: 1125px;
        }
        .auto-stylewh1 {
            height: 100px;
            width: 1125px;
        }
    </style>
</asp:Content>

