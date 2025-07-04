<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.DeluxeAccountManagement"
    Title="Deluxe Account Management" CodeBehind="DeluxeAccountManagement.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/DeluxeEPCSLogoffControl.ascx" TagName="DeluxeEPCSLogoffControl"
    TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .ButtonStyle {
            Height: 30px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnBasicOrDeluxe" runat="server" Value="Deluxe" />
    <script type="text/javascript">
        
        $( document ).ready(function() {
            if($("input[id*='hdnBasicOrDeluxe']").val() == 'Basic'){
                $('.basicOrDeluxe').html('Basic');
            }
        });

        
        function showContactCS()
        {   
            var divOrderOTP = document.getElementById('divOrderOTP');  
            var divContactCS = document.getElementById('divContactCS');            
            divOrderOTP.style.display = 'none';
            divContactCS.style.display = 'block';
        }
        function showOrderOTP()
        {   
            var divOrderOTP = document.getElementById('divOrderOTP');  
            var divContactCS = document.getElementById('divContactCS');            
            divOrderOTP.style.display = 'block';
            divContactCS.style.display = 'none';
        }
        
               

    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function throwCancelConfirm() {
                var hiddenCancel = document.getElementById("<%=hiddenCancel.ClientID %>");
                if (hiddenCancel != null) {
                    hiddenCancel.click();
                }
            }


        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManager2" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btContinue">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlPurchaseReceipt" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAccount" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdViewHistory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewHistory" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="pnlPurchaseReceipt" />
                    <telerik:AjaxUpdatedControl ControlID="pnlAccount" />
                    <telerik:AjaxUpdatedControl ControlID="rcptOrderStatus" />
                    <telerik:AjaxUpdatedControl ControlID="rcptOrderInfo" />
                    <telerik:AjaxUpdatedControl ControlID="btPrint" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderDate" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderNumber" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderDescription" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderAmount" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderCCNo" />
                    <telerik:AjaxUpdatedControl ControlID="lblOrderCCExpDate" />
                    <telerik:AjaxUpdatedControl ControlID="btnLeavePage" />
                    <telerik:AjaxUpdatedControl ControlID="btnStayOnPage" />
                    <telerik:AjaxUpdatedControl ControlID="backButton" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>

    <table cellpadding="5" cellspacing="0" width="100%" border="0">
        <tr class="h1title">
            <td></td>
        </tr>
        <tr class="h2title">
            <td>
                <asp:Button ID="backButton" runat="server" Text="Back" CssClass="btnstyle" OnClick="backButton_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <span style="font-size: 28px; font-weight: lighter; color: #808080;">Your Veradigm
                    ePrescribe™ Account </span>
            </td>
        </tr>
        <tr>
            <td align="left">
                <div style="margin-left: 150px">
                    <asp:Button ID="btnPaidProviders" runat="server" Text="Show Enrolled Providers" Height="30" CssClass="btnstyle" />
                </div>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlAccount" runat="server" Visible="true" Width="100%" Style="display: inline">
        <table cellpadding="2" cellspacing="2" width="100%">
            <tr>
                <td colspan="3" align="center" valign="top">
                    <asp:Label ID="onHoldMessage" runat="server" Visible="false">Your ePrescribe <span class="basicOrDeluxe">Deluxe</span> license is on hold. Please see below for details. To update your account, click on the link provided below.</asp:Label>
                    <asp:Label ID="cancelledMessage" runat="server" Visible="false">Your ePrescribe <span class="basicOrDeluxe">Deluxe</span> license is pending cancellation. The subscription will be disabled at the end of the current billing cycle:</asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <span id="onlHold" runat="server" style="color: Red; font-size: larger; font-weight: bold; display: none">- ON HOLD
                    </span>
                    <span id="cancelled" runat="server" style="color: Red; font-size: larger; font-weight: bold; display: none">- CANCELLED
                    </span>
                </td>
            </tr>
        </table>
        <div style="width: 90%; margin-left: 150px">
            <div id="divDeluxeAccount" style="width: 80%; height: 125px;">
                <br />

                <table cellpadding="2" cellspacing="2" width="100%" border="0">
                    <tr>
                        <td style="width: 10%;">
                            <span class="primaryForeColor" style="font-size: larger; font-weight: bold;">SUBSCRIPTION</span>

                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="width: 10%;">
                            <span style="margin-left: 30px">Product</span>
                        </td>
                        <td>
                            <asp:Label ID="lblProductName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%;">
                            <span ID="spanStartDate" runat="server" style="margin-left: 30px">Started                                
                            </span>
                        </td>
                        <td>
                            <asp:Label ID="lblStartDate" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%;">
                            <span ID="spanNextBilling" runat="server" style="margin-left: 30px">Next billing                                
                            </span>
                        </td>
                        <td>
                            <asp:Label ID="lblEndDate" runat="server"></asp:Label>

                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divEdit" runat="server">
                                <span class="primaryForeColor" style="font-size: larger; font-weight: bold;">Click</span>
                                <a id="lnkEdit" runat="server" class="primaryForeColor" style="cursor: pointer; font-size: larger; font-weight: bold;">here</a>
                                <span runat="server" id="spnUpdateOptions" class="primaryForeColor" style="font-size: larger; font-weight: bold;">to update your subscription options</span>

                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td colspan="2">
                            <div id="divDeluxeCancel" runat="server">
                                <span class="primaryForeColor" style="font-size: larger; font-weight: bold;">Click</span>
                                <a id="A1" runat="server" class="primaryForeColor" style="cursor: pointer; font-size: larger; text-decoration: underline; font-weight: bold;"
                                    onclick="throwCancelConfirm()">here</a>
                                <span class="primaryForeColor" style="font-size: larger; font-weight: bold;">to cancel your subscription</span>

                            </div>
                        </td>

                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    
                    <tr id="trBillingInfo" runat="server">
                        <td>
                            <span class="primaryForeColor" style="font-size: larger; font-weight: bold; padding-left: 0%;">BILLING</span>

                        </td>
                        <td>
                            <a id="updateLink" runat="server" href="DeluxeBillingPage.aspx?Mode=Update" class="primaryForeColor" style="cursor: pointer; margin-left: 20px">edit</a>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr id="trAddressInfo" runat="server">
                        <td>
                            <span style="margin-left: 30px">Address                               
                            </span>
                        </td>
                        <td>
                            <asp:Literal ID="litBilling" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr id="trCreditInfo" runat="server">
                        <td>
                            <span style="margin-left: 30px">Credit card                               
                            </span>
                        </td>
                        <td>
                            <asp:Literal ID="litCreditCard" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>

            </div>
        </div>
        <br />
        <br />
        <br />
        <br />
        <div style="height: 125px; width: 50%; margin-left: 65px">
            <table cellpadding="2" cellspacing="2" width="100%" border="0">
            </table>
        </div>
        <table cellpadding="2" cellspacing="2" width="100%" border="0">
            <tr>
                <td colspan="3" align="left">
                    <table cellpadding="0" cellspacing="0" width="90%" style="margin-left: 150px">
                        <tr id="trAccountHistory" runat="server">
                            <td class="primaryForeColor" style="width: 20%; text-align: left; font-size: larger; font-weight: bold;">ACCOUNT HISTORY
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
                                    <asp:Image ID="Image4" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif"
                                        Style="border: 0; vertical-align: middle; text-align: center" />
                                </telerik:RadAjaxLoadingPanel>
                                <telerik:RadGrid ID="grdViewHistory" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    GridLines="None" DataSourceID="GetAccountHistory" AllowAutomaticUpdates="True"
                                    OnItemDataBound="grdViewHistory_ItemDataBound" AllowPaging="True" AllowSorting="True"
                                    Style="width: 80%" AutoGenerateColumns="False">
                                    <MasterTableView GridLines="None" Style="width: 100%" NoMasterRecordsText="No account history."
                                        CommandItemDisplay="Top" PageSize="10" DataKeyNames="TransStatus, OrderNumber, OrderDate, Description, CardNo, Amount, Email">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Description">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                            <telerik:GridBoundColumn HeaderText="Date" DataField="OrderDate" DataFormatString="{0:MM/dd/yyyy}"
                                                UniqueName="OrderDate">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderText="Order Number" UniqueName="OrderStatus">
                                                <HeaderStyle HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblOrderStatus" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                            <telerik:GridTemplateColumn UniqueName="Action">
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <center>
                                                        <asp:Button ID="btnTransAction" runat="server" OnClick="btnTransAction_Click" CssClass="btnstyle"
                                                            Width="100px" />
                                                    </center>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                                </telerik:RadGrid>
                                <asp:ObjectDataSource ID="GetAccountHistory" runat="server" SelectMethod="GetDeluxeTransHistory"
                                    TypeName="Allscripts.Impact.DeluxePurchaseManager" OldValuesParameterFormatString="original_{0}">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlPurchaseReceipt" runat="server" Visible="true" Width="100%" Style="display: none">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">
                    <strong><a id="rcptOrderStatus" runat="server"></a></strong>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">
                    <strong><a id="rcptOrderInfo" runat="server"></a></strong>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center" valign="top">
                    <br />
                    <asp:Button ID="btPrint" runat="server" Text="Print Receipt" OnClientClick="javascript:window.print();"
                        CssClass="btnstyle" />
                    <asp:Button ID="btnStayOnPage" runat="server" Text="Continue" OnClick="btnStayOnPage_Click"
                        CssClass="btnstyle" />
                    <asp:Button ID="btnContinueLogOff" runat="server" Text="Logout" CssClass="btnstyle" />
                    <br />
                </td>
            </tr>
        </table>
        <table width="100%" cellpadding="4px">
            <tr>
                <td style="width: 10%"></td>
                <td colspan="2" align="left" style="font-weight: bold">
                    <asp:Label Style="font-weight: bold;" ID="lblLogout" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td align="left">
                    <hr style="width: 100%; background-color: #ffa500" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" align="left" style="font-weight: bold">Date :
                    <asp:Label ID="lblOrderDate" runat="server" Text="" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" align="left" style="font-weight: bold">
                    <asp:Label ID="lblOrderNumber" runat="server" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">
                    <div style="float: left; font-weight: bold;">
                        Description :&nbsp;
                    </div>
                    <div style="float: left">
                        <asp:Label ID="lblOrderDescription" runat="server" Text="12" Font-Bold="true">
                        </asp:Label>
                    </div>
                </td>
            </tr>            
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Charge :
                    <asp:Label ID="lblOrderAmount" runat="server" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Card : xxxx xxxx xxxx
                    <asp:Label ID="lblOrderCCNo" runat="server" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel Style="display: none" ID="panelProviders" runat="server">
        <div id="div6" runat="server" class="overlaymainwide">
            <div class="overlayTitle">
                <asp:Label CssClass="overlayTitleText" Style="font-size: medium" ID="lblProviders" runat="server" Text="Veradigm ePrescribe™ Deluxe - Enrolled Providers"></asp:Label>
            </div>
            <div id="div8" runat="server" class="overlaysub1">
                <table class="PopUpTable">
                    <tr>
                        <td align="center">
                            <table border="0">
                                <tr>
                                    <td runat="server" id="spnEnrolledProviderFeature" align="left" class="PopUpTd">Pricing for Veradigm ePrescribe™ is based on the number of enrolled providers.
                                        <br />
                                        Sharing user IDs is a violation of the terms of service and can result in immediate deactivation.
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="PopUpTd">
                                        <asp:Panel ID="panelProvidersScroll" runat="server" ScrollBars="Vertical" Width="90%"
                                            Height="200px">
                                            <center>
                                                <asp:GridView ID="deluxeProviderListGrid" runat="server" GridLines="Both" DataKeyNames="Name,AdminCode"
                                                    DataSourceID="GetDeluxePaidProviderList" AllowPaging="false" AutoGenerateColumns="false"
                                                    PageSize="100">
                                                    <Columns>
                                                        <asp:BoundField DataField="Name" HeaderText="Name" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="AdminCode" HeaderText="Admin" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:ObjectDataSource ID="GetDeluxePaidProviderList" runat="server" SelectMethod="GetDeluxePaidUsers"
                                                    TypeName="Allscripts.Impact.DeluxePurchaseManager" OldValuesParameterFormatString="original_{0}">
                                                    <SelectParameters>
                                                        <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                            </center>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnProviderListClose" runat="server" Text="Close" CssClass="btnstyle btnStyleAction" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeProviders" runat="server" BehaviorID="mpeProviders"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="btnPaidProviders"
        PopupControlID="panelProviders" CancelControlID="btnProviderListClose">
    </ajaxToolkit:ModalPopupExtender>

    <asp:Panel Style="display: none" ID="panelCancel" runat="server">
        <div id="div9" runat="server" class="overlaymain">
            <div id="div10" runat="server" class="overlaysub1">
                <table width="100%">
                    <tr>
                        <td align="center">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/eprescribedeluxe.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <br />
                            Are you sure you would like to cancel your ePrescribe™ <span class="basicOrDeluxe">Deluxe</span> subscription?
                        </td>
                    </tr>
                </table>
                <br />
                <center>
                    <asp:Button ID="btnCancelSubscriptionConfirm" runat="server" Text="Yes" CssClass="btnstyle"
                        OnClick="btnCancelSubscriptionConfirm_Click" />
                    <asp:Button ID="btnCancelSubscriptionCancel" runat="server" Text="No" CssClass="btnstyle" />
                </center>
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenCancel" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeCancel" runat="server" BehaviorID="mpeCancel"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenCancel"
        PopupControlID="panelCancel" CancelControlID="btnCancelSubscriptionCancel">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Panel ID="pnlFeedbackCancel" runat="server" Width="100%" Style="display: none">
        <table width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td align="center">
                    <table cellpadding="5" cellspacing="5" width="80%" style="display: inline">
                        <tr>
                            <td align="left">Thank you for using Veradigm ePrescribe™ <span class="basicOrDeluxe">Deluxe</span>. Your subscription will be canceled
                                on <a id="cancelDate" runat="server"></a>, after which you will no longer have access
                                to the enhanced features of ePrescribe™ <span class="basicOrDeluxe">Deluxe</span>.<br />
                                <br />
                                Please take a moment to reflect on your ePrescribe™ <span class="basicOrDeluxe">Deluxe</span> experience so that we
                                may better serve you in the future. We greatly appreciate your feedback.
                            </td>
                        </tr>
                        <tr style="height: 20px">
                            <td align="left"></td>
                        </tr>
                        <tr>
                            <td align="left" style="text-align: left; font-weight: bold;" class="primaryForeColor">Why are you cancelling your subscription?
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:RadioButtonList ID="cancelReason" runat="server" RepeatDirection="Vertical">
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">Your feedback is always appreciated: please feel free to add more details to your
                                answer above or provide general suggestions on how to improve our overall solution.
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <telerik:RadTextBox ID="txtComments" runat="server" TextMode="MultiLine" Width="400px"
                                    Height="150px" MaxLength="500">
                                </telerik:RadTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top">
                                <br />
                                <asp:Button ID="btnCancelFeedbackContinue" runat="server" Text="Continue" OnClick="btnCancelFeedbackContinue_Click"
                                    CssClass="btnstyle" CausesValidation="false" />
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlCancelled" runat="server" Width="90%" Style="display: none" HorizontalAlign="Center">
        <br />
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">Thank you for your feedback.
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <br />
                    <%--<asp:Button ID="btnReturn" runat="server" Text="Return to Program" CssClass="btnstyle" OnClick="btContinue_Click" />--%>
                    <asp:Button ID="btnCancelContinueLogOff" runat="server" Text="Continue" CssClass="btnstyle" />
                    <br />
                </td>
            </tr>
        </table>
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlCancelLogOff" Width="500px" runat="server" Style="text-align: center; display: none;">
        <table style="width: 100%; background-color: White;" cellpadding="10px">
            <tr>
                <td>
                    <span style="font-size: 14pt; text-align: center;">These changes will take effect once
                        you log out and log back in.</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnCancelLogOff" runat="server" Text="OK" CssClass="btnstyle" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeCancelLogOff" runat="server" BehaviorID="mpeLogOff"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="btnCancelContinueLogOff"
        PopupControlID="pnlCancelLogOff" Drag="false" PopupDragHandleControlID="pnlCancelLogOff" />

    <asp:Button ID="Button3" runat="server" Style="display: none;" />

    <ePrescribe:DeluxeEPCSLogoffControl ID="DeluxeLogOffMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
