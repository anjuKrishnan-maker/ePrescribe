<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ePANcpdpStatusInfo.ascx.cs" Inherits="eRxWeb.Controls_ePANcpdpStatusInfo" %>
<%@ Register Assembly="RadAjax.Net2" Namespace="Telerik.WebControls" TagPrefix="rad" %>
<%@ Register Assembly="RadGrid.Net2" Namespace="Telerik.WebControls" TagPrefix="rad" %>
<asp:Panel ID="panelEPAStatus" Style="display: none" runat="server" class="overlaymainwide">
    <div id="ePAStatusHolder" runat="server">
        <table width="100%" cellpadding="10px">
            <tr>
                <td>
                    <div id="statusTitle" runat="server">
                        <h2>
                            <asp:Literal ID="litMedication" runat="server"></asp:Literal>
                        </h2>
                        <h3>
                            <b>Patient Name: </b>
                            <asp:Literal ID="litPatient" runat="server"></asp:Literal>
                        </h3>
                        <p>
                            <asp:Literal ID="litContactMessage" runat="server"></asp:Literal>
                        </p>
                    </div>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="questionReview" runat="server" style="border: solid 1px black">
                        <table cellpadding="2px" class="ePAReviewTable">
                            <tr>
                                <td>
                                    <asp:Literal ID="litGeneralStatusNote" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Notes: </b><asp:Literal ID="litNotes" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <rad:RadGrid ID="gridEPAStatus" runat="server" AllowSorting="False" AutoGenerateColumns="False"
                                        GridLines="None" AllowMultiRowSelection="false" ShowHeader="False" OnItemCommand="gridEPAStatus_ItemCommand"
                                        OnItemCreated="gridEPAStatus_ItemCreated" Skin="Default2006" SkinsPath="telerik/Grid/Skins" EnableViewState="true" 
                                        Visible="false" BorderStyle="None">
                                        <MasterTableView DataKeyNames="DocumentName,DocumentBody,DocumentType"> 
                                            <Columns>
                                                <rad:GridTemplateColumn>
                                                    <ItemTemplate>
                                                        <asp:Image ID="imageAttachmentType" runat="server"  Width="16px" Height="16px"/>
                                                    </ItemTemplate>
                                                    <ItemStyle BorderStyle = "None" />
                                                </rad:GridTemplateColumn>
                                                <rad:GridButtonColumn Text="DocumentName" DataTextField="DocumentName" CommandName="ViewAttachment">
                                                </rad:GridButtonColumn>
                                            </Columns>
                                        </MasterTableView>
                                    </rad:RadGrid>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td valign="bottom" width="50px" style="text-align: right; vertical-align: bottom;">
                    <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btnstyle" OnClick="btnClose_Click"
                        CausesValidation="false" /><br />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:Button ID="hiddenEPAStatus" runat="server" Style="display: none;" />
<asp:Button ID="hiddenEPAStatusCancel" runat="server" Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpeEpaNcpdpStatus" runat="server" BehaviorID="mpeEpaNcpdpStatus"
    DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenEPAStatus"
    PopupControlID="panelEPAStatus" CancelControlID="hiddenEPAStatusCancel">
</ajaxToolkit:ModalPopupExtender>

