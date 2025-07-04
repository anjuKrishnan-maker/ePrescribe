<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.DeluxeBillingPage" Title="Purchase Deluxe" CodeBehind="DeluxeBillingPage.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/BillingPageFooter.ascx" TagName="BillingPageFooter" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/DeluxeEPCSLogoffControl.ascx" TagName="DeluxeEPCSLogoffControl" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/DeluxeEPCSCommonPanels.ascx" TagName="DeluxeEPCSCommonPanels" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .DeluxeMarginTop {
            margin-top: -5px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdnSelectedPricingStructure" runat="server" />
    <script type="text/javascript">
    

        
        function ValidateTandC(sender, args)
        {
            var chkTermcond = document.getElementById("<%=chkTermcond.ClientID %>");
            if (chkTermcond != null)
            {
                if (chkTermcond.checked)
                {
                    args.IsValid = true;
                    return;
                }
            }
            
            args.IsValid = false;
        }        
                
        function disableProcessOrder()
        {
            var processOrder = document.getElementById("<%=btProcessOrder.ClientID %>");
            if (processOrder != null )
            {          
                processOrder.disabled = true;                               
            }      

        }
    </script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function throwProviderList()
            {
                var hiddenProviders = document.getElementById("<%=DeluxeEPCSCommonPanels1.FindControl("hiddenProviders").ClientID %>");
                if($("input[id*='hdnSelectedPricingStructure']").val() == "CompulsoryBasic"){
                    $("#spnFeatureInPopUpTd").html("Basic");
                }
                if (hiddenProviders != null)
                {
                    hiddenProviders.click();
                }
            }
            
            function showFAQ()
            {            
                var hrefFAQ = document.getElementById("hrefFAQ");             
                if (hrefFAQ != null)
                {
                    hrefFAQ.click();
                }
            }    
                 
                                 
            
            function throwTC()
            {
                var hiddenTC = document.getElementById("<%= DeluxeEPCSCommonPanels1.FindControl("hiddenTC").ClientID %>");
               if (hiddenTC != null)
               {
                   hiddenTC.click();
               }
           }               
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManager2" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdViewCart">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewCart" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>

    <table cellpadding="0" cellspacing="0" width="100%" border="0">
        <tr class="h1title">
            <td></td>
        </tr>
        <tr class="h2title">
            <td></td>
        </tr>
        <tr>
            <td>
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td align="center" valign="top">
                <span id="spanHeader" runat="server" style="font-size: 30px; font-weight: lighter; color: #7B737B; margin-top: 10px; font-family: Verdana;"></span>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlDeluxePurchase" runat="server" Visible="true" Width="100%" Style="display: inline">
        <table cellpadding="0" cellspacing="0" width="100%" border="0">
            <tr>
                <td colspan="2">&nbsp;<asp:Label ID="lblWarningMsg" Font-Bold="true" Text="ERROR: There was an issue with your transaction. Please review the red boxes below for accuracy and resubmit your order" ForeColor="red" runat="server" Visible="false"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="color: black; width: 100%; font-weight: bold; padding-left: 5px; border-bottom: solid gray 1px; text-align: left; margin-bottom: 5px;">
                        Shopping Cart
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
                        <asp:Image ID="Image4" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif" Style="border: 0; vertical-align: middle; text-align: center" />
                    </telerik:RadAjaxLoadingPanel>
                    <telerik:RadGrid ID="grdViewCart" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                        GridLines="None" AllowAutomaticUpdates="True" OnItemDataBound="grdViewCart_ItemDataBound"
                        AllowPaging="True" AllowSorting="True" Style="width: 90%"
                        AutoGenerateColumns="False">
                        <MasterTableView GridLines="None" Style="width: 100%" NoMasterRecordsText="No items in your shopping cart." CommandItemDisplay="Top" PageSize="10" ShowFooter="True" DataKeyNames="Quantity,Amount">
                            <CommandItemTemplate>
                            </CommandItemTemplate>
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="Item" HeaderText="Item" DataField="Item">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <FooterStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label Text='<%# ObjectExtension.ToEvalEncode(Eval("Item"))  %>' Style="padding-right: 5px;"
                                            ID="lblItem" runat="server" Font-Underline="false"></asp:Label>
                                        <asp:Label Text='<%# ObjectExtension.ToEvalEncode(Eval("Note")) %>'  Style="display:block; color: Red; padding-right: 5px;"
                                            ID="lblNote" runat="server" Font-Underline="false"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:LinkButton Font-Underline="true" Text="Billing FAQ" Style="color: Blue; cursor: pointer; padding-right: 5px; font-weight: bold;" ID="lnkFAQ" runat="server" OnClientClick="showFAQ();return false;"></asp:LinkButton>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:LinkButton Text="Provider List" Style="color: Blue; cursor: pointer; padding-right: 5px;" ID="lnkProviders" runat="server" OnClientClick="throwProviderList();return false;" Font-Underline="false"></asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Quantity" DataField="Quantity" FooterText="TOTAL CHARGE:" FooterStyle-Font-Bold="true" FooterStyle-Font-Size="Medium">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <FooterStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn UniqueName="Amount" HeaderText="Amount" DataField="Amount" DataFormatString="{0:c}" FooterStyle-Font-Bold="true" FooterStyle-Font-Size="Medium">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <FooterStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                        <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                        <FooterStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" Wrap="True" />
                    </telerik:RadGrid>
                    <asp:Panel runat="server" BorderColor="#C4C4C4" BorderStyle="Solid"
                        ID="panelCharges" BackColor="#EEEEEE" Width="90%" BorderWidth="1px">
                        <table width="100%" border="0">
                            <tr>
                                <td style="text-align: right; width: 92%; padding-right: 5%;">
                                    <asp:Label Font-Size="10" Font-Bold="true" runat="server"
                                        Text="Current balance as of 7/1/2014:" ID="lblCurBalance"></asp:Label>
                                </td>
                                <td style="text-align: left; width: 8%;">
                                    <asp:Label Font-Size="10" Font-Bold="true" runat="server"
                                        Text="$" ID="lblCurFees"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; width: 92%; padding-right: 5%;">
                                    <asp:Label Font-Size="10" Font-Bold="true" runat="server"
                                        Text="Monthly recurring fees scheduled on 7/1/2014:" ID="lblRecBalance"></asp:Label>
                                </td>
                                <td style="text-align: left; width: 8%;">
                                    <asp:Label Font-Size="10" Font-Bold="true" runat="server"
                                        Text="$" ID="lblRecFees"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                </td>
            </tr>
            <tr id="trBillingInfo" runat="server">
                <td colspan="2">
                    <table cellpadding="2" cellspacing="2" width="100%" border="0">
                    </table>
                    <br />
                </td>
            </tr>
            <tr id="trCreditBillingnfo" runat="server">
                <td colspan="2">
                    <div style="color: black; width: 100%; font-weight: bold; padding-left: 5px; border-bottom: solid gray 1px; text-align: left; margin-bottom: 5px;">
                        Credit Card Billing Information
                        <a style="color: GrayText"><b style="color: Red">*</b>Required Fields</a>
                    </div>
                </td>
            </tr>
             <tr id="trBillingAddress" runat="server">
                <td colspan="2">
                    <table cellpadding="2" cellspacing="2" width="100%" border="0">
                        <tr>
                            <td style="width: 20%; text-align: right; font-weight: bold;">Billing Address:
                            </td>
                            <td colspan="2">
                                <asp:Literal ID="litBilling" runat="server"></asp:Literal>
                            </td>
                            <td align="right">We accept the following credit cards:                                
                            </td>
                            <td align="left">
                                <asp:Image ID="Image5" runat="server" ImageUrl="~/images/ccgraphics.GIF" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 20%; text-align: right; font-weight: bold;">
                                <asp:Literal ID="litCreditCard" runat="server"></asp:Literal>
                            </td>
                            <td colspan="2">
                                <asp:Literal ID="litCreditCardNumber" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td style="text-align: right; width: 15%;">Email:</td>
                            <td style="text-align: left" colspan="2">
                                <telerik:RadTextBox ID="txtEmail" runat="server" MaxLength="40"
                                    Style="width: 250px;" Skin="Allscripts" EnableEmbeddedSkins="false">
                                </telerik:RadTextBox>
                                <b style="color: Red">*</b>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="maingroup"
                                    ErrorMessage="Please enter an Email address."><b style="color:Red">*</b>
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regexEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="maingroup"
                                    ValidationExpression="^([a-zA-Z0-9_\-\.\']+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
                                    ErrorMessage="Please enter a valid Email address."><b style="color:Red">Invalid Email address</b></asp:RegularExpressionValidator>
                            </td>
                            <td colspan="3">
                                <div style="border: solid 1px red; padding: 3px;">
                                    <strong style="color: Red">IMPORTANT: </strong>Your Account activation and billing information will be sent to
                                    this address.
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right; width: 15%;">Confirm Email:</td>
                            <td style="text-align: left" colspan="5">
                                <telerik:RadTextBox ID="txtEmailConfirm" runat="server" MaxLength="40"
                                    Style="width: 250px;" Skin="Allscripts" EnableEmbeddedSkins="false">
                                </telerik:RadTextBox>
                                <b style="color: Red">*</b>
                                <asp:RequiredFieldValidator ID="rfvEmailConfirm" runat="server" ControlToValidate="txtEmailConfirm" ValidationGroup="maingroup"
                                    ErrorMessage="Please enter a Confirm Email."><b style="color:Red">*</b>
                                </asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="cvConfirmEmail" runat="server" ControlToCompare="txtEmail" ValidationGroup="maingroup" ErrorMessage="Text in the confirm Email field should match the Email field."
                                    ControlToValidate="txtEmailConfirm"><b style="color:Red">*</b></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <br />
                                <div id="divShippingInfo" runat="server" style="color: black; width: 100%; font-weight: bold; padding-left: 5px; border-bottom: solid gray 1px; text-align: left; margin-bottom: 5px;"
                                    visible="true">
                                    Shipping Information
                                    <br />
                                </div>
                                <table id="tblShipping" runat="server" cellpadding="2" cellspacing="2" width="100%" border="0">
                                    <tr>
                                        <td style="width: 20%; text-align: right; font-weight: bold;">Shipping Address:
                                        </td>
                                        <td colspan="2">
                                            <asp:Literal ID="litShippingAddress" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <asp:CheckBox ID="chkTermcond" runat="server" Text="By Submitting your credit card information you indicate that you accept and agree to our"></asp:CheckBox>
                                <a runat="server" id="lnkTC" style="color: Blue; font-weight: bold; text-decoration: underline; cursor: pointer;" onclick="throwTC()">Terms and Conditions.</a>
                                <asp:CustomValidator ID="cvCheckTermcond" runat="server" ErrorMessage="You must agree to the Terms and Conditions" ClientValidationFunction="ValidateTandC" ValidationGroup="maingroup"><b style="color:Red; vertical-align:middle;">*</b></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6">
                                <asp:ValidationSummary ID="valsum" runat="server" ValidationGroup="maingroup" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="6">
                                <asp:Button ID="ButtonAddCreditCard" runat="server" Text="Add Payment"
                                    OnClick="ButtonAddCreditCard_Click" CausesValidation="true"
                                    CssClass="btnstyle" ValidationGroup="maingroup" />
                                <asp:Button ID="btCont" runat="server" CausesValidation="true"
                                    CssClass="btnstyle" OnClick="btCont_Click" Text="Process Payment" Visible="false"
                                    ValidationGroup="maingroup" />
                                <asp:Button ID="btCancel" runat="server" CausesValidation="false"
                                    CssClass="btnstyle" OnClick="btCancel_Click" Text="Cancel" />
                                <!-- test button - remove after testing -->
                                <%--<asp:Button ID="btnContinueLogOff2" runat="server" Text="Test Panel" CssClass="btnstyle" />--%>
                            </td>
                        </tr>
                    </table>
                    <ePrescribe:BillingPageFooter ID="BillingPageFooter1" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlPurchaseConfirmation" runat="server" Visible="false" Width="100%" Style="display: none">
        <br />
        <table cellpadding="0" cellspacing="0" width="100%" border="0">
            <tr>
                <td style="width: 10%"></td>
                <td style="width: 5%"></td>
                <td colspan="3" align="center" class="primaryForeColor" style="text-align: left; font-size: large; font-weight: bold">Order Confirmation:
                </td>
                <td style="width: 5%"></td>
                <td style="width: 10%"></td>
            </tr>
            <tr>
                <td colspan="7">
                    <br />
                </td>
            </tr>
            <tr style="padding-bottom: 3px;">
                <td style="width: 10%"></td>
                <td colspan="3" align="left" style="text-align: left;">Your Credit card ending in
                    <asp:Label ID="lblCClastfourdigits" runat="server" Text="8855">
                    </asp:Label>
                    will be charged for the following:
                </td>
                <td style="width: 5%"></td>
                <td style="width: 10%"></td>
            </tr>
            <tr>
                <td style="width: 10%"></td>
                <td align="left" style="width: 90%; text-align: left; border: solid 2px black;" colspan="5">
                    <table width="100%" border="0">
                        <tr>
                            <td colspan="5">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 70%; padding-left: 20px; padding-right: 50px; font-size: large; font-weight: bold">Description</td>
                            <td style="font-size: large; font-weight: bold">Price</td>
                            <td align="center" style="font-size: large; font-weight: bold">Quantity</td>
                            <td style="font-size: large; font-weight: bold">Amount</td>
                            <td style="padding-left: 20px"></td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <hr style="width: 100%; border: thin solid #808080;" />
                            </td>
                        </tr>
                        <tr id="rowItems" runat="server">
                            <td style="width: 70%; padding-left: 20px; padding-right: 50px;">
                                <asp:Label runat="server" ID="lbldesc" Text="" Font-Bold="true" Font-Size="Large"></asp:Label>
                                <asp:Label Text="" Style="color: Red; padding-right: 5px;"
                                    ID="lblNoteDesc" runat="server" Font-Underline="false"></asp:Label>
                                <br />
                                Monthly charges will vary depending on the number of active providers.
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblItemAmount" Text="" Font-Bold="true" Font-Size="Large"></asp:Label>
                            </td>
                            <td align="center">
                                <asp:Label ID="lblUserCount" runat="server" Text="12" Font-Size="Large" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblItemTotal" runat="server" Text="12" Font-Size="Large" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="padding-left: 20px"></td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <br />
                            </td>
                        </tr>
                        <tr id="rowEpcsSetup" runat="server">
                            <td style="width: 70%; padding-left: 20px; padding-right: 50px;">
                                <asp:Label runat="server" ID="lblEpcsSetupDesc" Text="" Font-Bold="true" Font-Size="Large"></asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblEpcsSetupAmount" Text="" Font-Bold="true" Font-Size="Large"></asp:Label>
                            </td>
                            <td align="center">
                                <asp:Label ID="lblEpcsSetupCount" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblEpcsSetupTotal" runat="server" Text="" Font-Size="Large" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="padding-left: 20px"></td>
                        </tr>
                        <tr>
                            <td colspan="6" style="padding-top: 20px">
                                <hr style="width: 100%; border: thin solid #808080;" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 70%; padding-left: 20px; padding-right: 50px;"></td>
                            <td></td>
                            <td align="center">
                                <asp:Label ID="lblcharge" runat="server" Text="Item total" Font-Bold="true" Font-Size="Large"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblamount" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
                            </td>
                            <td style="padding-left: 20px"></td>
                        </tr>
                        <tr>
                            <td colspan="6" style="padding-top: 20px">

                                <asp:Panel runat="server" ID="panelConfirmCharges" BackColor="#EEEEEE" Width="100%">
                                    <hr style="width: 100%; border: thin solid #808080; margin-top: -10px" />
                                    <table width="98%" border="0">
                                        <tr>
                                            <td style="text-align: right; width: 90%; padding-right: 2%;">
                                                <asp:Label Font-Size="Large" Font-Bold="true" runat="server"
                                                    Text="Current balance as of 7/1/2014:" ID="lblConfirmCurBalance"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 10%;">
                                                <asp:Label Font-Size="Large" Font-Bold="true" runat="server"
                                                    Text="$" ID="lblConfirmCurFees"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: right; width: 90%; padding-right: 2%;">
                                                <asp:Label Font-Size="Large" Font-Bold="true" runat="server"
                                                    Text="Monthly recurring fees scheduled on 7/1/2014:" ID="lblConfirmRecBalance"></asp:Label>
                                            </td>
                                            <td style="text-align: left; width: 10%;">
                                                <asp:Label Font-Size="Large" Font-Bold="true" runat="server"
                                                    Text="$" ID="lblConfirmRecFees"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 10%"></td>
            </tr>

            <tr style="padding-top: 3px;">
                <td style="width: 10%"></td>
                <td colspan="3"></td>
                <td style="width: 5%"></td>
                <td style="width: 10%"></td>
            </tr>
            <tr>
                <td colspan="7">
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="7" align="center">
                    <asp:Button ID="btProcessOrder" runat="server" Text="Process Order" OnClick="btProcessOrder_Click" OnClientClick="disableProcessOrder()" UseSubmitBehavior="false" CssClass="btnstyle" />
                    <asp:Button ID="btConfirmCancel" runat="server" Text="Cancel" OnClick="btConfirmCancel_Click" CssClass="btnstyle" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlDeluxePurchaseOrder" runat="server" Width="100%" Visible="false" Style="display: none">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr id="rowPurchaseOrderStatus" runat="server">
                <td colspan="3" align="center" class="billing-purchase_alignment">
                    <strong class="erx_font-regularsize-bold">Congratulations! Your Veradigm ePrescribe™ <span id="spnDeluxeOrBasicActive" runat="server" class="erx_font-regularsize-bold">Deluxe</span> account is now active.</strong>
                </td>
            </tr>
            <tr id="trEPCCS" runat="server" visible="false">
                <td colspan="3" align="center" class="billing-purchase-epcs_alignment">
                    <span class="billing-purchase-epcs-content">
                        Thank you for registering for EPCS. You will receive an email shortly that includes directions on how to enable EPCS functionality for use in your application. Please refer to these directions for information about the final steps you must complete to begin e-prescribing controlled substances
                    </span>
                </td>
            </tr>
            <tr id="rowPurchaseTokenOrderStatus" runat="server">
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">
                    <strong>Congratulations! Your Veradigm ePrescribe™ EPCS token order is completed.</strong>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">
                    <strong>
                        <asp:Label Font-Size="Large" ID="lblTokenOrder" runat="server"></asp:Label>
                    </strong>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center" style="font-size: large; font-weight: bold">
                    <asp:Label ID="lblemailID" runat="server" Text="name@domain.com" Font-Bold="true" Font-Size="Large">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center" valign="top">
                    <br />
                    <asp:Button ID="btPrint" runat="server" Text="Print Receipt" OnClientClick="javascript:window.print();" CssClass="btnstyle" />
                    <asp:Button ID="btnContinue" runat="server" OnClick="btnContinue_Click" Text="Continue" CssClass="btnstyle" Visible="false" />
                    <asp:Button ID="btnContinueLogOff" runat="server" Text="Logout" CssClass="btnstyle" />
                    <br />
                </td>
            </tr>
        </table>
        <table width="100%" cellpadding="4px">
            <tr id="rowPurchaseOrderLogout" runat="server">
                <td style="width: 10%"></td>
                <td colspan="2" align="left" style="font-weight: bold">You must log out before ePrescribe™ <span id="spnDeluxeOrBasicAvailable" runat="server" style="font-weight: inherit;">Deluxe</span> features are available. 
                </td>
            </tr>
            <tr>
                <td></td>
                <td align="left">
                    <hr style="width: 100%; background-color: #7AB800" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" align="left" style="font-weight: bold">Date:
                    <asp:Label ID="lbldate" runat="server" Text="03/20/2009" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" align="left" style="font-weight: bold">Order Number:
                    <asp:Label ID="lbOrderno" runat="server" Text="33344555" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Description:                                       
                    <asp:Literal ID="litPurchaseOrderDesc" Text="" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Charge: $
                    <asp:Label ID="lblOrderamount" runat="server" Text="300.00" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Card: xxxx xxxx xxxx
                    <asp:Label ID="lblOrderClastfour" runat="server" Text="8855" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="2" style="font-weight: bold">Your next payment will be charged on
                    <asp:Label ID="lblnextOrdderdate" runat="server" Text="04/20/2009" Font-Bold="true">
                    </asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>


    <asp:Panel Style="display: none" ID="panelProcess" runat="server">
        <div id="div3" runat="server" class="overlaymain">
            <div id="div5" runat="server" class="overlaysub1">
                <center>
                    Thank you! Your order is being processed. This might take several minutes.<br />
                    <img src="telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif" alt="" />
                </center>
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenProcess" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeProcess" runat="server" BehaviorID="mpeProcess" DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenProcess" PopupControlID="panelProcess"></ajaxToolkit:ModalPopupExtender>

    <asp:Button ID="btnCancelContinueLogOff" runat="server" Text="Continue" CssClass="btnstyle" Style="display: none;" />
    <ePrescribe:DeluxeEPCSLogoffControl ID="DeluxeLogOffMessageControl" runat="server" />
    <ePrescribe:DeluxeEPCSCommonPanels ID="DeluxeEPCSCommonPanels1" runat="server" />

    <a href="Help/Documents/EPCSFAQ.pdf" id="hrefFAQ" target="_blank" style="display: none;"></a>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
