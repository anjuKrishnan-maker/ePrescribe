<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.NurseFreeFormDrug" Title="Free Form Drug" Codebehind="NurseFreeFormDrug.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="chkBoxCSMed">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="chkBoxCSMed" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSchedule" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rblFreeTextMedType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlControlledSubstance" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td colspan="2" class="h1title">
                <span runat="server" id="heading" class="Phead indnt">Free Form Drug</span>
            </td>
        </tr>
        <tr>
            <td style="background-color: #868585">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="h2title indnt">
                <table border="0" cellspacing="0" cellpadding="0" width="100%">
                    <tr>
                        <td>
                            <ePrescribe:Message ID="Message1" runat="server" MessageText="Enter the name of the drug (Max characters allowed is 105)"
                                Icon="INFORMATION" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td colspan="3">
                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnChangeMed" runat="server" CssClass="btnstyle" Text="Change Med"
                                                        CausesValidation="false" ToolTip="Click to go to the 'Choose Medication' page where you can choose a medication."
                                                        OnClick="btnChangeMed_Click" />&nbsp;&nbsp;
                                                    <asp:Button ID="btnSelectSig" runat="server" CssClass="btnstyle" Text="Select Sig"
                                                        OnClick="btnSelectSig_Click" ToolTip="Click to select a SIG." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                                            <tr class="Titbarcolor4">
                                                <td>
                                                    &nbsp;
                                                </td>
                                            </tr>
                                            <tr bgcolor="#FFFFFF">
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                            <td>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <span style="color: Red">*</span> Please choose:
                                                                        </td>
                                                                        <td>
                                                                            <asp:RadioButtonList ID="rblFreeTextMedType" runat="server" AutoPostBack="true" RepeatDirection="Horizontal"
                                                                                OnSelectedIndexChanged="rblFreeTextMedType_SelectedIndexChanged">
                                                                                <asp:ListItem Text="Compound Medication" Value="Compound"></asp:ListItem>
                                                                                <asp:ListItem Text="Supply Item" Value="Supply"></asp:ListItem>
                                                                            </asp:RadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <span style="color: Red">*</span> Medication:
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtFreeForm" runat="server" Height="100px" TextMode="MultiLine"
                                                                                Width="350px" MaxLength="105"></asp:TextBox>
                                                                            <div>
                                                                                (Maximum 105 Characters / <span id="charsRemaining" runat="server">105</span> characters
                                                                                remaining)
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:RequiredFieldValidator ID="rfvFreeTextMedType" runat="server" ControlToValidate="rblFreeTextMedType"
                                                                                Display="None" ErrorMessage="Compound or supply selection is required. A free form medication must be designated as either compound or supply.  Single medications or non-supply items must be selected from the all meds list.">
                                                                            </asp:RequiredFieldValidator>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFreeForm"
                                                                                Display="None" ErrorMessage="Please enter a drug name.">
                                                                            </asp:RequiredFieldValidator>
                                                                            <asp:RegularExpressionValidator ID="revFreeFormMed" runat="server" ControlToValidate="txtFreeForm"
                                                                                ErrorMessage="Invalid medication name. Please enter a valid medication." ValidationExpression="^([0-9a-zA-Z@.\s-,()&%/]{1,105})$"  Display="None">
                                                                            </asp:RegularExpressionValidator>
                                                                            <asp:ValidationSummary ID="validationSummary" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                            <td>
                                                                <asp:Panel ID="pnlControlledSubstance" runat="server">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkBoxCSMed" runat="server" Text="Controlled Substance Medication"
                                                                                    Visible="false" ToolTip="Check here to indicate it is a Controlled Substance"
                                                                                    OnCheckedChanged="chkBoxCSMed_CheckedChanged" AutoPostBack="true" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Panel ID="pnlSchedule" runat="server" Visible="false">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lblSchedule" runat="server" Text="Schedule"></asp:Label>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:RadioButtonList ID="rblSchedule" runat="server" RepeatDirection="Horizontal">
                                                                                                    <asp:ListItem Text="II" Value="2"></asp:ListItem>
                                                                                                    <asp:ListItem Text="III" Value="3"></asp:ListItem>
                                                                                                    <asp:ListItem Text="IV" Value="4"></asp:ListItem>
                                                                                                    <asp:ListItem Text="V" Value="5"></asp:ListItem>
                                                                                                </asp:RadioButtonList>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="rblSchedule"
                                                                                                    ErrorMessage="Please select a schedule." Display="None" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </asp:Panel>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <%--<asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
