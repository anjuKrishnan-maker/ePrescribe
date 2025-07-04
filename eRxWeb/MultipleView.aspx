<%--Revision History
HA/Dhiraj 68 Mozilla Firefox Browser Error with buttons
--%>

<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="True" Inherits="eRxWeb.MultipleView" Title="Script Print Preview" CodeBehind="MultipleView.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script language="javascript" type="text/javascript">
        // <!CDATA[
        // Dhiraj Bug: 68
        function Button1_onclick() {

            getIFrameDocument('iframe1').focus();
            getIFrameDocument('iframe1').print();

            document.getElementById("PrintInd").value = "Y";
            //document.iframe1.focus();
            //document.iframe1.print();
        }

        function getIFrameDocument(aID) {
            // if contentDocument exists, W3C compliant (Mozilla)
            var frame = document.getElementById(aID);
            if (frame != null || frame != undefined) {
                if (frame.contentDocument) {
                    return frame.contentWindow;
                }
                else {
                    return document.frames[aID];
                }
            }
        }

// ]]>
    </script>

    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr class="h1title">
            <td colspan="2">
                <span class="Phead indnt">Print Scripts </span>
            </td>
        </tr>
        <tr class="h2title">
            <td></td>
        </tr>
        <tr>
            <td colspan="2">
                <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h4title">
                                    <td colspan="4">
                                        <asp:Button ID="btnBack" runat="server" CssClass="btnstyle"
                                            Text="Back" OnClick="btnBack_Click" />&nbsp;
                                        <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" OnClick="btnCancel_Click"
                                            Text="Cancel" />&nbsp;                                        
                                        <asp:Button ID="btnPrint" runat="server" Text="Print" OnClientClick="Button1_onclick()" CssClass="btnstyle" />
                                        <input type="hidden" id="PrintInd" name="PrintInd"
                                            value="N" />&nbsp;&nbsp;
                                        <asp:Button ID="btnPrevious" runat="server" CssClass="btnstyle" OnClick="btnPrevious_Click"
                                            Text="Previous" />&nbsp;&nbsp;
                                        <asp:Button ID="btnNext" runat="server" CssClass="btnstyle" OnClick="btnNext_Click"
                                            Text="Next" />&nbsp;
                                           <%-- <input id="Button2" class="btnstyle" type="button" value="Print"
                                                onclick="return Button1_onclick()" />--%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="indnt">
                            <iframe name="iframe1" id="iframe1" src="PrintScript.aspx" align="top" width="800"
                                height="<%=getTableHeight() %>" frameborder="0" title="Print Script">If you can
                                see this, your browser does not support iframes! </iframe>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <!-- Reference got from http://www.eggheadcafe.com/PrintSearchContent.asp?LINKID=449 -->
                            <%--comment here--%>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <%--    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <contenttemplate>           
            <asp:Panel id="panelHelpHeader" CssClass="accordionHeader" runat="server" width="100%" ><TABLE cellSpacing=0 cellPadding=0 width="100%" border=0><TBODY><TR><TD align=left width=140><DIV id="Header" class="accordionHeaderText">Help With This Screen</DIV></TD><TD align=right width=14><asp:Image id="hlpclpsimg" runat="server"  ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;</TD></TR></TBODY></TABLE></asp:Panel> 
            <asp:Panel id="panelHelp" CssClass="accordionContent" runat="server" width="95%" ></asp:Panel> 
            <ajaxToolkit:CollapsiblePanelExtender id="cpeHelp" runat="server"  Collapsed="true" CollapsedSize="0" TargetControlID="panelHelp" ExpandControlID="panelHelpHeader" CollapseControlID="panelHelpHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="hlpclpsimg" SuppressPostBack="true"></ajaxToolkit:CollapsiblePanelExtender>                      
            <asp:Panel id="panelMessageHeader" CssClass="accordionHeader" runat="server" width="100%" ><TABLE cellSpacing=0 cellPadding=0 width="100%" border=0><TBODY><TR vAlign=baseline height=16><TD align=left width=140><asp:Label cssClass="accordionHeaderText" id=lblPrintingInfo runat=server>Printing Information</asp:Label><br /></TD><TD align=right width=14><asp:Image id="msgclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;</TD></TR></TBODY></TABLE></asp:Panel> 
                <asp:Panel id="panelMessage" CssClass="accordionContent" runat="server" width="95%" >   
                Prescriptions printed in this state do not require special security paper.  If you feel more secure in printing your prescriptions on security style paper, please visit our paper vendor, <a target=_paper href="http://www.rxpaper.com/ePrescribe/">http://www.rxpaper.com/ePrescribe/</a>.  Select the state you are practicing in and follow the link for instructions on ordering.             
                </asp:Panel> <ajaxToolkit:CollapsiblePanelExtender id="cpeMessage" runat="server"  Collapsed="false" CollapsedSize="0" TargetControlID="panelMessage" ExpandControlID="panelMessageHeader" CollapseControlID="panelMessageHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="msgclpsimg" SuppressPostBack="true"></ajaxToolkit:CollapsiblePanelExtender> 
                
        </contenttemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
