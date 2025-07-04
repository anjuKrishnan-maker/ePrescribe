<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ePAInitQAFileUploader.aspx.cs"
    Inherits="eRxWeb.ePAInitQAFileUploader" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
</head>
<body>
    <script type="text/javascript">
        function throwPostBack() {
            var fileUploader = document.getElementById("<%=fileUploader.ClientID %>");
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
                        alert("Invalid file type. Valid file types are PDF, JPG, PNG or TIFF.");
                    }
                }
                else {
                }
            }
        }

        function checkValidFile(fileName) {
            var ret = false;
            var _validFileExtensions = [".jpg", ".jpeg", ".tiff", ".tif", ".png", ".pdf"];

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
    <div class="ePAFileListing">
        <asp:ScriptManager ID="uploaderScriptManager" runat="server" EnablePartialRendering="true"
            ScriptMode="Release">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="updatePanel1" runat="server">
            <ContentTemplate>
                <span style="font-size: small; color: Gray; font-style: italic">Add Attachments&nbsp;&nbsp;(.pdf,
                    .jpeg, .tiff, .png)</span><br />
                    <table class="ePAReviewTable">
                        <tr>
                            <td>
                                 <asp:FileUpload ID="fileUploader" runat="server" />
                            </td>
                            <td align="right" width="150px">
                               <input id="btAttach" runat="server" type="button" value="Attach" onclick="throwPostBack()" class="ePAbtnStyle" />
                              <span id="maxAttachmentSize" runat="server" style="font-size: small; color: Gray; font-style: italic">12 MB Max</span><br />
                            <asp:Button runat="server" ID="btnProcessFile" Style="display: none;" OnClick="btnProcessFile_Click"
                                CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                <asp:Repeater runat="server" ID="fileList" OnItemDataBound="file_DataBound" DataSourceID="attachmentsDS" >
                    <ItemTemplate>
                        <div class="ePAAttachment">
                            <table>
                                <tr>
                                    <td>
                                        <asp:ImageButton ID="Remove" runat="server" ImageUrl="~/Images/error.png" Style="display: inline"
                                            OnClick="removeFile" CausesValidation="false" />&nbsp
                                    </td>
                                    <td>
                                        <%# ObjectExtension.ToEvalEncode(Eval("FileName")) %>&nbsp(<%# ObjectExtension.ToEvalEncode(Eval("FileSizeDisplay")) %>)
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <span id="sizeWarning" runat="server" class="ePAWarning" style="display: none;">File
                    cannot be added because it would put total attachment size over 12MB.</span>
                <asp:ObjectDataSource ID="attachmentsDS" runat="server" SelectMethod="GetAttachments"
                    TypeName="Allscripts.Impact.EPAInitAttachment" DataObjectTypeName="EPAInitAttachment">
                    <SelectParameters>
                        <asp:SessionParameter Name="baseQuestionSet" SessionField="ePACurrentInitiationQuestionSet"
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
</body>
</html>
