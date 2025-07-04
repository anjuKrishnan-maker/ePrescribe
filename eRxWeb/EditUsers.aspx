<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.EditUsers" Title="Edit Users" Codebehind="EditUsers.aspx.cs" %>
<%@ Import Namespace="eRxWeb.AppCode" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr class="h1title">
            <td>
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucError" runat="server" Visible="false" />
            </td>
        </tr>
        <tr class="h2title">
            <td class="Phead indnt" style="color: white">
                Edit Users
            </td>
        </tr>
        <tr>
            <td colspan="8">
                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td colspan="6">
                                        <asp:Button UseSubmitBehavior="false" ID="btnBack" runat="server" Text="Back" OnClick="btnBack_OnClick" CssClass="btnstyle" CausesValidation="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr>                                   
                                     <td>
                                        <table border="0" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td >
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblLastName" runat="server" Text="Last Name: " CssClass="Phead" ForeColor="Black"></asp:Label>
                                                                <asp:TextBox ID="txtLastName" runat="server" ></asp:TextBox>

                                                                <asp:Label ID="lblFirstName" runat="server" Text="First Name: " CssClass="Phead" ForeColor="Black"></asp:Label>
                                                                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>  
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnUserSearch" runat="server" OnClick="btnUserSearch_Click" Text="Search" ToolTip="Search patient" />  
                                                            </td>
                                                        </tr>
                                                    </table> 
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="rblSearch" runat="server" RepeatDirection="Horizontal"  
														 AutoPostBack="true" OnSelectedIndexChanged="rblSearch_SelectedIndexChanged">
														<asp:ListItem Text="Active" Value="Active" Selected="true"></asp:ListItem>
														<asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
														<asp:ListItem Text="All" Value="All"></asp:ListItem>
													</asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>   
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <telerik:RadGrid ID="gridUsers" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="false"
                                Skin="Allscripts" AllowSorting="true" AllowPaging="true" AllowCustomPaging="true"
                                PageSize="20" OnItemCommand="gridUsers_ItemCommand" OnItemDataBound="gridUsers_ItemDataBound"
                                OnNeedDataSource="gridUsers_NeedDataSource" OnPageIndexChanged="gridUsers_PageIndexChanged">
                                <MasterTableView DataKeyNames="UserID, UserName, ShieldStatus, ShieldProfileName, ShieldProfileUserID, Email,FirstName,LastName"
                                    AllowNaturalSort="true">
                                    <NoRecordsTemplate>
                                        <div>
                                            <p>
                                                No users found.
                                            </p>
                                        </div>
                                    </NoRecordsTemplate>
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridButtonColumn ButtonType="LinkButton" Text="Edit" UniqueName="Edit" CommandName="Edit">
                                        </telerik:GridButtonColumn>
                                        <telerik:GridBoundColumn DataField="LoginID" HeaderText="Login ID" SortExpression="LoginID">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="FirstName" HeaderText="First Name" SortExpression="FirstName">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="StatusDescription" HeaderText="Status" SortExpression="StatusDescription"
                                            UniqueName="Status">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn ButtonType="LinkButton" UniqueName="ActionLink">
                                        </telerik:GridButtonColumn>
                                    </Columns>
                                </MasterTableView>
                                <PagerStyle Mode="NextPrevAndNumeric" PagerTextFormat="{4}" />
                            </telerik:RadGrid>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="panelResetPassword" runat="server" class="overlaymain" Style="display: none;">
        <div class="overlayTitle">
            Password Reset
        </div>
        <div style="width: 100%; text-align: center" class="overlayContent">
            <p>
                Please supply the user with the system generated password below:
            </p>
            <br />
            <p style="font-weight: bold">
                <asp:Literal ID="litNewPassword" runat="server"></asp:Literal>
            </p>
            <br />
        </div>
        <div class="overlayFooter">
            <asp:Button ID="btnOK" runat="server" Text="OK" CssClass="btnstyle btnStyleAction" Width="100px" />
        </div>
    </asp:Panel>
    <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeResetPassword" runat="server" PopupControlID="panelResetPassword"
        TargetControlID="btnHiddenTrigger" BackgroundCssClass="modalBackground" OkControlID="btnOK">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel ID="panelNewActivationCode" runat="server" class="overlaymain" Style="display: none;">
        <div class="overlayTitle">
            New Activation Code Requested
        </div>
        <div class="overlayContent">
        <p>
            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label></p>
        <p>
            Submit activation code to the user.
        </p>
        <br />
        <p style="color: Red;">
            Important: Only the selected user can complete this process. You cannot complete
            this process for them!
        </p>
        <br />
        <p>
            Instructions to provide to the user for activating their account:
        </p>
        <ol>
            <li>Navigate to
                <asp:Label ID="lblShieldPortalLink" runat="server"></asp:Label>
            </li>
            <li>
                <asp:Label ID="litActivationCode" runat="server"></asp:Label></li>
            <li>Follow the onscreen instructions to complete the activation process.</li>
        </ol>
        <p>
            Please choose how you want to provide the Activation Code to the user.
        </p>
        <br />
        <div style="float: left">
            <asp:CheckBox ID="chkEmailAC" runat="server" Text="Send to user's email" Checked="true" />&nbsp&nbsp
        </div>
        <div>
            <asp:TextBox ID="txtEmailAC" runat="server" Width="250px"></asp:TextBox>
        </div>
        <p>
            <asp:CheckBox ID="chkPrintAC" runat="server" Text="Print" Checked="true" />
        </p>
        <br />
            </div>
        <div class="overlayFooter">
            <asp:Button ID="btnSendACEmail" runat="server" Text="Process" OnClick="btnSendACEmail_Click"
                CssClass="btnstyle btnStyleAction" Width="100px" />
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeNewActivationCode" runat="server" PopupControlID="panelNewActivationCode"
        TargetControlID="btnHiddenTrigger" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel ID="panelException" runat="server" class="overlaymain" Style="display: none;">
        <p style="color: Red; width: 100%; text-align: center;">
            <asp:Literal ID="litExceptionText" runat="server"></asp:Literal>
        </p>
        <br />
        <div style="float: right">
            <asp:Button ID="btnClose" runat="server" CssClass="btnstyle" Text="Close" OnClientClick="return false;" />
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeException" runat="server" BackgroundCssClass="modalBackground"
        OkControlID="btnClose" PopupControlID="panelException" TargetControlID="btnHiddenTrigger">
    </ajaxToolkit:ModalPopupExtender>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   <%-- <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
            <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex="0" runat="server" ContentCssClass="accordionContent"
                HeaderCssClass="accordionHeader">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="paneHelp" runat="server">
                        <Header>
                            Help With This Screen</Header>
                        <Content>
                            <asp:Panel ID="HelpPanel" runat="server">
                            </asp:Panel>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>
            </ajaxToolkit:Accordion>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
