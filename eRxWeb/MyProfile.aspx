<%@ Page Language="C#" Title="My Profile" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="Allscripts.Web.UI.MyProfile" Codebehind="MyProfile.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/MyeRxHeader.ascx" TagName="MyeRxHeader" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
            <script type="text/javascript">
                function openChangePasswordModal() {
                    window.parent.showChangePasswordModal();
            }            
            </script>
    <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td>
                    <table border="0" width="100%" cellspacing="0" cellpadding="0">
                        <tr class="h1title">
                            <td style="padding-left: 25px">
                                <ePrescribe:MyeRxHeader ID="MyeRxHeader" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <eprescribe:message ID="ucMessage" runat="server" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="pnlUserMain" runat="server">
                                    <asp:Panel ID="userInfoPanelHeader" runat="server" BackColor="LightGray" Width="100%"
                                        BorderColor="black" BorderWidth="1px">
                                        <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:Panel ID="userInfoPanelDetail" runat="server" Width="100%">
                                        <table border="0" width="100%" cellpadding="3" cellspacing="0">
                                            <tr>
                                                <td style="padding-left: 15px">
                                                    <table>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                First Name:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblFirstName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                Middle Initial:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblMI" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                Last Name:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblLastName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                User Name:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                Email:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblEmail" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="text-align: right; font-weight: bold">
                                                                Roles:
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblRoles" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <asp:Panel ID="pnlChangePassword" runat="server" Visible="true">
                                                        <img src="images/splitbar_expand_h.gif" alt="" />
                                                        <asp:LinkButton ID="lnkEdit" runat="server" OnClick="EditProfileButton_Click">Edit Profile</asp:LinkButton>
                                                        <br />
                                                        <asp:Panel ID="pnlChangePasswordLink" runat="server" Visible="true">
                                                            <img src="images/splitbar_expand_h.gif" alt="" />
                                                           <asp:HyperLink ID="lnkChangePassword" runat="server" NavigateUrl="javascript:openChangePasswordModal();">Change Password</asp:HyperLink>
                                                         </asp:Panel>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   <%-- <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
        <div id="divHideTools_Help" runat="server">
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
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>--%>

</asp:Content>
