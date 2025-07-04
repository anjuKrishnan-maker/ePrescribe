<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.PDF_InPage" Title="PDF Print" Codebehind="PDF_InPage.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr class="h1title">
            <td>
            </td>
        </tr>
        <tr class="h2title">
            <td>
            </td>
        </tr>
        <tr class="h4title">
            <td>
                <asp:Button ID="btnClose" CssClass="btnstyle" runat="server" Text="Close" OnClick="btnClose_Click" />
            </td>
        </tr>
        <tr>
            <td align="center" class="indnt" height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                <asp:Panel ID="pnlFrame" runat="server">
                    <iframe name="iframe1" id="iframe1" runat="server" src="pdf.aspx?PrintTaskType=2"
                        align="top" width="100%" height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>"
                        frameborder="0" title="">If you can see this, your browser does not support
                        iframes! </iframe>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
            <asp:Panel ID="panelHelpHeader" CssClass="accordionHeader" runat="server" Width="100%">
                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tbody>
                        <tr>
                            <td align="left" width="140">
                                <div id="Header" class="accordionHeaderText">
                                    Help With This Screen</div>
                            </td>
                            <td align="right" width="14">
                                <asp:Image ID="hlpclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="panelHelp" CssClass="accordionContent" runat="server" Width="95%">
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="cpeHelp" runat="server" Collapsed="true"
                CollapsedSize="0" TargetControlID="panelHelp" ExpandControlID="panelHelpHeader"
                CollapseControlID="panelHelpHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png"
                ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="hlpclpsimg" SuppressPostBack="true">
            </ajaxToolkit:CollapsiblePanelExtender>
            <asp:Panel ID="panelMessageHeader" CssClass="accordionHeader" runat="server" Width="100%">
                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tbody>
                        <tr valign="baseline" height="16">
                            <td align="left" width="140">
                                <asp:Label CssClass="accordionHeaderText" ID="lblPrintingInfo" runat="server">Printing Information</asp:Label><br />
                            </td>
                            <td align="right" width="14">
                                <asp:Image ID="msgclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="panelMessage" CssClass="accordionContent" runat="server" Width="95%">
                Prescriptions printed in this state do not require special security paper. If you
                feel more secure in printing your prescriptions on security style paper, please
                visit our paper vendor, <a target="_paper" href="http://www.rxpaper.com/ePrescribe/">
                    http://www.rxpaper.com/ePrescribe/</a>. Select the state you are practicing
                in and follow the link for instructions on ordering.
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="cpeMessage" runat="server" Collapsed="false"
                CollapsedSize="0" TargetControlID="panelMessage" ExpandControlID="panelMessageHeader"
                CollapseControlID="panelMessageHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png"
                ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="msgclpsimg" SuppressPostBack="true">
            </ajaxToolkit:CollapsiblePanelExtender>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
