<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Title="Reports" Inherits="eRxWeb.MultipleViewReport" CodeBehind="MultipleViewReport.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script language="javascript" type="text/javascript">
    function exportAction(ddlExport)
    {
        switch (ddlExport.value)
        {
            case 'pdf':
                exportToPdfAndAudit();
                break;
            case 'excel':
               ExportToExcel();
                break;
            case 'word':
                exportToWordAndAudit();
                break;
            case 'print':
                callFunction();
                ddlExport.disabled = true;
                break;
        }
    }

function callFunction()
{
    var btnExport;
    btnExport = document.getElementById('btnExport');
    if (btnExport != null)
    {
        btnExport.disabled = true;
    }

    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/AuditReportPrinted',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{extensionInfo: "' + document.getElementById('ctl00_ContentPlaceHolder1_hdnAuditExtensionInfo').value + '" }'
    });

    window.frames.iframe1.fnCheck();
    return false;
}

function exportToPdfAndAudit() {
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/AuditReportPdfExport',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{extensionInfo: "' + document.getElementById('ctl00_ContentPlaceHolder1_hdnAuditExtensionInfo').value + '" }'
    });

    window.frames.iframe1.exportToPdfAndAudit();
}

function exportToWordAndAudit() {
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/AuditReportWordExport',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{extensionInfo: "' + document.getElementById('ctl00_ContentPlaceHolder1_hdnAuditExtensionInfo').value + '" }'
    });

    window.frames.iframe1.exportToWordAndAudit();
}
function ExportToExcel() {
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/AuditReportExcelExport',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{extensionInfo: "' + document.getElementById('ctl00_ContentPlaceHolder1_hdnAuditExtensionInfo').value + '" }'
    });

    var hdnExportUrl;    
    hdnExportUrl = document.getElementById('ctl00_ContentPlaceHolder1_hdnExportToExcel');   
    if(hdnExportUrl !== undefined)   
        $('<form></form>').attr('method', 'POST').attr('action', hdnExportUrl.value).appendTo('body').submit().remove();
    //Closing overlay on submitted
    var dvLoad = document.getElementById("divLoading");
    if (dvLoad != undefined && dvLoad != null)
        dvLoad.style.display = "none";
}
        var win = null;

window.onload = function () {
    var browser = detectBrowser();

    if ((browser == 'MSIE,8.0') || (browser == 'MSIE,7.0')) {
        var objiFrame = document.getElementById("iframe1"); // To fix the issue with the IE 8 browser (Scroll bar issue #720663)
        if (objiFrame != null) {
            objiFrame.height = "600";
        }
    }
}
function detectBrowser() {
    var N = navigator.appName;
    var UA = navigator.userAgent;
    var temp;
    var browserVersion = UA.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if (browserVersion && (temp = UA.match(/version\/([\.\d]+)/i)) != null)
        browserVersion[2] = temp[1];
    browserVersion = browserVersion ? [browserVersion[1], browserVersion[2]] : [N, navigator.appVersion, '-?'];
    return browserVersion;
    };

    </script>

    <table border="0" cellpadding="0" cellspacing="0" width="100%">

        <tr>
            <td class="Phead indnt h1title">
               Print Reports
            </td>
        </tr>
        <tr class="h2title">
            <td colspan="2" style="height: 19px"></td>
        </tr>
        <tr>
            <td colspan="2">
                <table align="center" border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0"
                    width="100%">
                    <tr>
                        <td>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h4title">
                                    <td colspan="4">
                                        <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" Text="Close" OnClick="btnCancel_Click" />&nbsp;&nbsp;
                                        Export To:
                                        <select id="ddlExport">
                                            <option value="print">Print</option>
                                            <option value="excel">Excel</option>
                                            <option value="word">Word</option>
                                            <option value="pdf">PDF</option>
                                        </select>
                                        <input id="btnExport" type="button" value="GO" onclick="exportAction(ddlExport)" />
                                        <asp:HiddenField ID="hdnExportToExcel" runat="server" />
                                        <asp:HiddenField ID="hdnAuditExtensionInfo" runat="server" />
                                    </td>

                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="indnt" style="width: 100%">
                            <asp:Panel ID="pnlFrame" runat="server">
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <asp:Panel ID="panelNotesHeader" class="accordionHeader" runat="server" Width="95%" Visible="false">
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tbody>
                <tr>
                    <td align="left" width="140">
                        <div id="Div1" class="accordionHeaderText">
                            Report Notes
                        </div>
                    </td>
                    <td align="right" width="14">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;</td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="panelNotes" class="accordionContent" runat="server" Width="92%" Visible="false">
        <center>
            <br />
            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Width="95%" Height="100px" Visible="false"></asp:TextBox>
            <br />
            <asp:Button ID="btnAddNotes" runat="server" Text="Update Notes" CssClass="btnstyle" Visible="false" OnClick="btnAddNotes_Click" />
            <br />
            <br />
        </center>
    </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeNotes" runat="server" TargetControlID="panelNotes"
        Collapsed="false" CollapsedSize="0" ExpandControlID="panelNotesHeader" CollapseControlID="panelNotesHeader"
        ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png"
        ImageControlID="hlpclpsimg" SuppressPostBack="true">
    </ajaxToolkit:CollapsiblePanelExtender>
</asp:Content>
