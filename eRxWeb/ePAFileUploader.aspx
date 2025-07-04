<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.ePAFileUploader" Codebehind="ePAFileUploader.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <script type="text/javascript">
        function throwPostBack() {
            var fileUploader = document.getElementById("<%=fileUploader.ClientID %>");
            var btAttach = document.getElementById("<%=btAttach.ClientID %>");
            
            if (fileUploader != null) {
                if (fileUploader.value) {

                    if (checkValidFile(fileUploader.value)) {
                        var btnProcessFile = document.getElementById("<%=btnProcessFile.ClientID %>");
                        if (btnProcessFile != null) {
                            var btAttach = document.getElementById("<%=btAttach.ClientID %>");
                            if (btAttach != null) {
                                btAttach.value = "Processing...";
                                btAttach.disabled = "true";
                                btAttach.style.cursor = "wait";
                            }
                            
                            btnProcessFile.click();
                        }
                    }
                    else {
                        alert("Invalid file type. Valid file types are PDF, JPG, or TIFF.");
                    }
                }
                else {
                }
            }
        }

        function checkValidFile(fileName) {
            var ret = false;
            var _validFileExtensions = [".jpg", ".jpeg", ".tiff", ".tif", ".pdf"];

            for (var j = 0; j < _validFileExtensions.length; j++) {
                var sCurExtension = _validFileExtensions[j];
                if (fileName.substr(fileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

    </script>
    <form id="fileUploaderForm" runat="server" enctype="multipart/form-data" target="_self">
    <div width="200px;">
        <asp:ScriptManager ID="uploaderScriptManager" runat="server" EnablePartialRendering="true"
            ScriptMode="Release">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="updatePanel1" runat="server">
            <ContentTemplate>
                <span style="font-size: small; color: Gray; font-style: italic">Add Attachments&nbsp;&nbsp;(.pdf,
                    .jpeg, .tiff)</span><br />
                <asp:FileUpload ID="fileUploader" runat="server" /><br />
                <div style="padding: 5px;">
                   <input id="btAttach" runat="server" type="button" value="Attach" onclick="throwPostBack()" class="ePAbtnStyle" />
                   <span id="maxAttachmentSize" runat="server" style="font-size: small; color: Gray; font-style: italic">12 MB Max</span><br />
                </div>
                <asp:Button runat="server" ID="btnProcessFile" Style="display: none;" OnClick="btnProcessFile_Click"
                    CausesValidation="false" />
                <asp:Repeater runat="server" ID="fileList" OnItemDataBound="file_DataBound" DataSourceID="attachmentsDS">
                    <ItemTemplate>
                        <div class="ePAFileListing" style="padding: 5px;">
                            <asp:ImageButton ID="Remove" runat="server" ImageUrl="~/Images/error.png" Style="display: inline"
                                OnClick="removeFile" CausesValidation="false" />&nbsp&nbsp<div id="fileNameDiv" runat="server"
                                    style="display: inline">
                                    <%# ObjectExtension.ToEvalEncode(Eval("FileName")) %>&nbsp(<%# ObjectExtension.ToEvalEncode(Eval("FileSizeDisplay")) %>)</div>
                            <br />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <span id="sizeWarning" runat="server" class="ePAWarning" style="display: none;">File
                    cannot be added because it would put total attachment size over 12MB.</span>
                <asp:ObjectDataSource ID="attachmentsDS" runat="server" SelectMethod="GetAttachments"
                    TypeName="Allscripts.Impact.ePAAttachment" DataObjectTypeName="ePAAttachment">
                    <SelectParameters>
                        <asp:SessionParameter Name="baseQuestionSet" SessionField="ePA_Current_QuestionSet"
                            Type="Object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnProcessFile" />
            </Triggers>
        </asp:UpdatePanel>
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
