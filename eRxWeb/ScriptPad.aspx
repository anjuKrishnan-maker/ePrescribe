<%@ Page Language="C#" AutoEventWireup="True" Inherits="eRxWeb.ScriptPad"
    Title="Script Pad" MasterPageFile="~/PhysicianMasterPage.master" Codebehind="ScriptPad.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/SpecialtyMedsUserWelcome.ascx" TagName="SpecialtyMedsUserWelcome"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ControlsPatMissingInfo.ascx" TagName="ControlsPatMissingInfo" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning"   TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/RxCopy.ascx" TagName="RxCopy"  TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing"  TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/RxInfoList.ascx" TagName="RxInfoList"  TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/eCoupon.ascx" TagName="eCoupon"  TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/PptPlusCoupon.ascx" TagName="PptPlusCoupon"  TagPrefix="ePrescribe" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <style type="text/css">
        .ePaAlert { font-style: italic; font-weight: bold; display: block }
    </style>
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
            function OpenNewWindow(url)
            {
                window.open(url);
            }


            function PrintInfo() {
                alert("This is a receipt of medications that have been prescribed and\\or sent electronically \n\n to the pharmacy during the patient's visit with the doctor.")
                return false;
            }

            window.onload = resizeCouponCells;
            $(window).resize(resizeCouponCells);

            function resizeCouponCells() {
                try{
                    var tdWidth1 = document.getElementById('ctl00_ContentPlaceHolder1_grdScriptPad_ctl00__0').cells[0].offsetWidth;
                    var tdWidth2 = document.getElementById('ctl00_ContentPlaceHolder1_grdScriptPad_ctl00__0').cells[1].offsetWidth;
                    var tdWidth3 = document.getElementById('ctl00_ContentPlaceHolder1_grdScriptPad_ctl00__0').cells[2].offsetWidth;
                    var tdWidth4 = document.getElementById('ctl00_ContentPlaceHolder1_grdScriptPad_ctl00__0').cells[3].offsetWidth;

                    var elements = document.getElementsByClassName("couponCell1");
                    for (var i = 0; i < elements.length; i++) {
                        elements[i].width = tdWidth1-15;
                    }

                    elements = document.getElementsByClassName("couponCell2");
                    for (i = 0; i < elements.length; i++) {
                        elements[i].width = tdWidth2 - 15;
                    }

                    elements = document.getElementsByClassName("couponCell3");
                    for (i = 0; i < elements.length; i++) {
                        elements[i].width = tdWidth3 - 17;
                    }

                    elements = document.getElementsByClassName("couponCell4");
                    for (i = 0; i < elements.length; i++) {
                        elements[i].width = tdWidth4 - 17;
                    }
                }
                catch (err)
                {
                    //catching error for IE8 and below
                }
            }
            function ViewOffer(url) {
                ifrm = document.createElement("IFRAME");
                ifrm.setAttribute("src", url);
                ifrm.style.width = 595 + "px";
                ifrm.style.height = 400 + "px";
                document.getElementById('<%=divInnerOfferContent.ClientID%>').appendChild(ifrm);
                $find("ctl00_ContentPlaceHolder1_mpeECouponOffer").show();
            }
            function ClosePopup() {
                document.getElementById('<%=divInnerOfferContent.ClientID%>').removeChild(document.getElementById('<%=divInnerOfferContent.ClientID%>').firstChild);
                $find("ctl00_ContentPlaceHolder1_mpeECouponOffer").hide();
            }
            function ViewPrescribingInfo(url) {
                ifrm = document.createElement("IFRAME");
                ifrm.setAttribute("src", url);
                ifrm.style.width = 595 + "px";
                ifrm.style.height = 400 + "px";
                document.getElementById('<%=divInnerInfoContent.ClientID%>').appendChild(ifrm);
                $find("ctl00_ContentPlaceHolder1_mpeECouponInfo").show();
            }
            function ClosePrescribingInfoPopup() {
                document.getElementById('<%=divInnerInfoContent.ClientID%>').removeChild(document.getElementById('<%=divInnerInfoContent.ClientID%>').firstChild);
                $find("ctl00_ContentPlaceHolder1_mpeECouponInfo").hide();
            }

            function ShowAngularPdfOverlay(url, title) {
                resizeCouponCells();
                if (window.parent.OpenPdfOverlay !== undefined) {
                    window.parent.OpenPdfOverlay(url, title);
                }
                return false;
            }
            function OpenGoodRxCouponInModal(htmlContent) {
                resizeCouponCells();
                window.parent.ShowExternalHtmlOverlay(htmlContent,'GoodRx - Coupon');
            }
            function OpenUrlInModal(url, title) {
                resizeCouponCells();
                window.parent.OpenUrlInModal(url, title);
            }
            function destionationOptionChanged(btnHiddenTriggerChangePptPharmacy, btnHiddenTriggerUpdateCouponControl) {
                btnHiddenTriggerChangePptPharmacy.click();
                btnHiddenTriggerUpdateCouponControl.click();
            }

            function TogglePharmacy(ddldest, lblPharmacy)
            {
                if(ddldest.value =="PHARM" || ddldest.value ==  "DOCPHARM")
                {
                    lblPharmacy.style.display = "inline";
                }
                else {
                    lblPharmacy.style.display = 'none';
                }
            }

            function PDMPCheckRegistry() {
                var chkRegistryChecked = document.getElementById('<%=chkRegistryChecked.ClientID%>')
                if (chkRegistryChecked != null) {
                    chkRegistryChecked.checked = true;
                }
            }

        </script>
    </telerik:RadCodeBlock>   
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr class="h1title">
            <td colspan="1">
                <asp:Label ID="lblPageHeader" runat="server" CssClass="Phead indnt" Text="Script Pad"></asp:Label>                
            </td>
            <td colspan="1" align="center" valign="middle">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ePrescribe:Message ID="ucRxCopyMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessageHeader" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessageRxHeader" runat="server" Visible="false"/>
            </td>
        </tr>
        
        <tr>
            <td colspan="2">
                <table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <asp:Panel ID="pnlButtons" runat="server">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr class="h4title">
                                        <td style="height: 18px">
                                        </td>
                                        <td colspan="7" style="height: 18px">
                                            &nbsp;&nbsp;
                                            <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back"
                                                CausesValidation="false" Visible="false" OnClick="btnReturn_Click" />
                                            <asp:Button ID="btnSelectMed" CssClass="btnstyle" runat="server" Text="Select Med"
                                                CausesValidation="false" OnClick="btnSelectMed_Click" />
                                            <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" Text="Choose Patient"
                                                CausesValidation="false" OnClick="btnCancel_Click" />
                                            <asp:Button ID="btnChangePharm" CssClass="btnstyle" runat="server" Text="Change Pharmacy"
                                                CausesValidation="false" OnClick="btnChangePharm_Click" />
                                            <asp:Button ID="btnProcess" CssClass="btnStyleOneArrow" Width="145px" runat="server" Text="Process Script Pad"
                                                CausesValidation="false" OnClick="btnProcess_Click" />
                                             <asp:Button ID="btnCheckRegistry" runat="server" CausesValidation="false" CssClass="btnstyle" Text="Check Registry" Width="145px" ToolTip="Click here to connect to your state’s Prescription Monitoring Program" />        
                                            <asp:CheckBox ID="chkRegistryChecked" runat="server" Text="State Registry Checked" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%--<div id="divScriptPadGrid" runat="server" style="height:600px;
                                position: relative; border: 0 #000000 solid; text-align: left; padding: 2px">--%>
                                <%--<div style="overflow-y: scroll;height:300px;">--%> 
                                <telerik:RadGrid ID="grdScriptPad" runat="server" Skin="Allscripts" EnableEmbeddedSkins="False"
                                    GridLines="None" 
                                    OnItemDataBound="grdScriptPad_OnItemDataBound" OnDataBound="grdScriptPad_OnDataBound" AllowMultiRowSelection="True"
                                    AllowPaging="True" AllowSorting="True" Style="width: 100%;height:100%;" PageSize="50" 
                                    AutoGenerateColumns="False" ondatabinding="grdScriptPad_DataBinding" CellSpacing="0">

                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <MasterTableView GridLines="None" NoMasterRecordsText="Script Pad is currently empty"
                                        Style="width: 100%" CommandItemDisplay="None"
                                        ClientDataKeyNames="RxId"
                                        DataKeyNames="RxId" TableLayout="Fixed">
                                        <HeaderStyle Font-Bold="true"/>
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderText="Rx Date">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litRxDate" runat="server" />
                                                </ItemTemplate>
                                                <HeaderStyle Width="80px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn>
                                                <ItemTemplate>
                                                    <asp:Image ID="BenefitImage" runat="server" Visible="false" ImageUrl="~/images/dollar.png"/>
                                                </ItemTemplate>
                                                <HeaderStyle Width="40px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Medication & Sig">
                                                <ItemTemplate>
                                                <div style="word-wrap: break-word;"  >
                                                    <asp:Image ID="imgEPAScript" runat="server" Visible="False" ImageUrl="~/images/e-p-a.png" AlternateText="ePA"/>
                                                    <asp:Literal ID="litMedicationAndSig" runat="server"></asp:Literal>
                                                </div>
                                                </ItemTemplate>
                                                <HeaderStyle Width="50%" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn  HeaderText="Send Notification">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="cbSendToADM" runat="server" Width="100" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn>
                                                <HeaderTemplate>
                                                    <table cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="vertical-align: middle">
                                                                <b>Destination</b>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="white-space: nowrap">
                                                        <asp:DropDownList ID="ddlDest" Width="150px" runat="server">
                                                        </asp:DropDownList>
                                                        <input type="hidden" ID="btnHiddenTriggerChangePptPharmacy" runat="server" />
                                                        <input type="hidden" ID="btnHiddenTriggerUpdateCouponControl" runat="server" />
                                                       <div style="padding-top:5px">
                                                         <asp:Label ID="lblGoodRxPharmacy" style="display:none" runat="server"></asp:Label>
                                                       </div>
                                                    </div>
                                                </ItemTemplate>
                                                <HeaderStyle Width="20%" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:Image ID="imgCS_Alert" runat="server" ForeColor="red" Visible="false" ImageUrl="~/images/ControlSubstance_sm.gif" />
                                                    <asp:Image ID="imgDateAlert" runat="server" Visible="false" ImageUrl="~/images/warning-16x16.png" />
                                                    <asp:Image ID="imgMDD_Alert" runat="server" Visible="False" ImageUrl="~/images/MDD.gif" />
                                                    <div class="tooltipContainer" style="float: left; margin-right: -10px;">
                                                        <asp:Image ID="imgSpecialtyMedAlert" Visible="False" ImageUrl="~/images/launch-nor-dark-16-x-16.png" runat="server"/>
                                                        <div class="divTooltip">
                                                            Please select “Patient Access Services” in destination dropdown for iAssist Workflows.
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>Actions</HeaderTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                                <ItemTemplate>
                                                    <div style="width: 72px">
                                                        <div style="float: left; width: 24px">
                                                            <asp:ImageButton ID="btnEdit" ImageUrl="~/images/Edit.gif" OnClick="btnEdit_Click" runat="server" AlternateText="Edit this script" ToolTip="Edit this script"></asp:ImageButton>
                                                        </div>
                                                        <div style="float: left; width: 24px">
                                                            <asp:ImageButton ID="btnCopy" ImageUrl="~/images/Copy.png" OnClick="btnCopy_Click" runat="server" AlternateText="Create additional prescription" 
																ToolTip="Create additional prescription"></asp:ImageButton>
                                                        </div>
                                                        <div style="float: right; width: 24px">
													        <asp:ImageButton ID="btnRemove" ImageUrl="~/images/Delete.gif" OnClick="btnRemove_Click" runat="server" AlternateText="Delete this script" ToolTip="Delete this script"></asp:ImageButton>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                        <DetailItemTemplate>
                                            <ePrescribe:RxInfoList ID ="ucRxInfoList" runat="server" RxID='<%# ObjectExtension.ToEvalEncode(Eval("RxId")) %>'/>
                                            <ePrescribe:PptPlusCoupon ID ="ucPptPlusCoupon" runat ="server" RxID='<%# ObjectExtension.ToEvalEncode(Eval("RxId")) %>'/>
                                            <ePrescribe:eCoupon ID ="ucEcoupon" runat ="server" RxID='<%# ObjectExtension.ToEvalEncode(Eval("RxId")) %>'/>
                                        </DetailItemTemplate>
                                    </MasterTableView>
                                </telerik:RadGrid>
                                <%--</div>--%>
                                <br />
                                &nbsp&nbsp&nbsp&nbsp<asp:Label ID="lblDUR" runat="server" ForeColor="red" Visible="false" Text="* - Script with DUR warning and requires physician approval."></asp:Label>
                                <br />
                                <asp:Label ID="lblCSLegend" runat="server" ForeColor="red" Visible="false">CS - Federal and state specific controlled substances cannot be sent electronically.</asp:Label>                                
                                <br/>
                                <div id="divBottomBoxes" runat="server" class="scriptPadBottomRow">                                    
                                    <div id="divPatientReceipt" runat="server" class="patientReceipt mainBox firstBox">
                                        <p class="header">
                                            <asp:CheckBox ID="ChkPatientReceipt" runat="server" Text="Print Patient Receipt" />
                                        </p>
                                        <p>
                                            <asp:HyperLink ID="hlLearnMore" runat="server" onclick="return PrintInfo();" Text="(Click here to learn more)"
                                                NavigateUrl="~/Spa.aspx"></asp:HyperLink>
                                        </p>
                                    </div>
                                    <div style="clear: both">
                                    </div>                                   
                                </div>
                                <br />
                                <br />
                                <asp:Label ID="lblCouponDisClaimer" runat="server"  Visible="false" Font-Bold="true" > *** Final coupon eligibility determined at Pharmacy or upon activation *** </asp:Label>
     
                                <div id="divHealthSheets" runat="server" style="display:none;">
                                    <asp:Panel ID="HealthSheetPanel" runat="server" CssClass="eRxHealthSheet_Panel" Visible="false">
                                        <asp:HyperLink ID="lnkPatientEducation" runat="server" CssClass="eRxHealthSheet_Link"
                                            Style="color:Black" ToolTip="eRx HealthSheets™ is a new feature on ePrescribe that is designed to deliver Krames patient education at the point of an electronic prescription initiation. Relevant patient education is automatically aligned to frequently prescribed medications while you’re completing the script process -- there’s no searching required. Simply print-on-demand for each of your patients."
                                            Text="Print eRx HealthSheets™" Target="_blank"></asp:HyperLink>
                                    </asp:Panel>
                                    <asp:Label ID="lblHealthSheetPrint" runat="server" Text="*eRx HealthSheets™ will be printed automatically"
                                        ForeColor="Red" Visible="false"></asp:Label>
                                </div>
                            <%--</div>--%>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        
    </table>
    
    <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
    <asp:Button ID="btnHiddenTrigger1" runat="server" Style="display: none;" />
    <div id="divOfferContent" runat="server" class="overlaymain" style="display: none;">
        <div id="divInnerOfferContent" runat="server"></div>
        <p style="width: 550px; text-align: center">
        <img src="images/CloseModal.png" id="ibCloseOffer" alt="Close"  style="cursor:pointer;" onclick="ClosePopup();"/>
        </p>
    </div>
    <ajaxToolkit:ModalPopupExtender ID="mpeECouponOffer" runat="server" TargetControlID="btnHiddenTrigger"
        PopupControlID="divOfferContent" BackgroundCssClass="modalBackground" DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <div id="divInfoContent" runat="server" class="overlaymain" style="display: none">
    <div id="divInnerInfoContent" runat="server"></div>
        <p style="width: 550px; text-align: center">
             <img src="images/CloseModal.png" id="Img1" alt="Close"  style="cursor:pointer;" onclick="ClosePrescribingInfoPopup();"/>
        </p>
    </div>
    <ajaxToolkit:ModalPopupExtender ID="mpeECouponInfo" runat="server" TargetControlID="btnHiddenTrigger1"
        PopupControlID="divInfoContent" BackgroundCssClass="modalBackground" DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnHiddenTrigger2" runat="server" Style="display: none;" />
    <div id="divEPASentNotice" runat="server" class="overlaymain" style="display: none;">
        <h5 style="width: auto; text-align: center; font-size: 15px;">
            The following medication(s) have been sent to the ePA task list for processing:
        </h5>
        <div style="width: auto;">
            <asp:Repeater runat="server" ID="rptEPASentList">
                <HeaderTemplate>
                    <div style="width: 80%; max-height: 100px; overflow-y: auto; margin: auto">
                </HeaderTemplate>
                <ItemTemplate>
                    <p><%# DataBinder.Eval(Container.DataItem, "Value") %></p>
                </ItemTemplate>
                <SeparatorTemplate>
                    <p style="border-bottom: 1px solid #cfcfcf; margin-top: 5px; margin-bottom: 5px;"></p>
                </SeparatorTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="width: auto; padding-top: 15px; padding-left: 20px;">
            <p>View prior authorization status in the ePA task list.</p>
            <br/>
            <p>Once the ePA process is completed the script can be processed.</p>
            <br/>
            <asp:CheckBox runat="server" ID="cbDoNotShowAgain" Text="Don't show me this again." />
        </div>
        <div style="width: auto; text-align: right; padding-top: 15px;">
            <asp:LinkButton runat="server" ID="lbCloseEPASentNotice" Text="Ok" CausesValidation="False" Style="padding-right: 10px; font-size: 14px" OnClick="lbCloseEPASentNotice_Click"></asp:LinkButton>
        </div>
    </div>
    <ajaxToolkit:ModalPopupExtender ID="mpeEPASentList" runat="server" TargetControlID="btnHiddenTrigger2"
        PopupControlID="divEPASentNotice" BackgroundCssClass="modalBackground" DropShadow="false">
    </ajaxToolkit:ModalPopupExtender>
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="true" />
    <ePrescribe:ControlsPatMissingInfo ID="ucPatMissingInfo" runat="server"/>
    <eprescribe:SpecialtyMedsUserWelcome ID="ucSpecialtyMedsUserWelcome" runat="server"/>
	<ePrescribe:RxCopy ID="ucRxCopy" runat="server" />
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <ePrescribe:SecureProcessing ID="secureProcessingControl" runat="server" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="radAjaxManager" runat="server">
        <ClientEvents OnResponseEnd="disableBtnSubmitEPCS()" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnProcess">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlButtons" LoadingPanelID="LoadingPanel2" />
                    <telerik:AjaxUpdatedControl ControlID="grdScriptPad" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID = "ucEPCSDigitalSigning" />
                    <telerik:AjaxUpdatedControl ControlID = "ucSpecialtyMedsUserWelcome" />
                    <telerik:AjaxUpdatedControl ControlID = "ucMessage" />
                    <telerik:AjaxUpdatedControl ControlID="rptEPASentList" />
                    <telerik:AjaxUpdatedControl ControlID="ucPatMissingInfo" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">     
</asp:Content>
