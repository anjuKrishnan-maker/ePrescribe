<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.NurseSig" Title="Choose Sig" ViewStateEncryptionMode="Never"
    EnableViewStateMac="false" Codebehind="NurseSig.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/MedicationHistoryCompletion.ascx" TagName="MedicationHistoryCompletion"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/FactsAndComparison.ascx" TagName="IFC" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%--<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing"  TagPrefix="ePrescribe" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script>
        function OpenNewWindow(url) {
            window.open(url);
        }
    </script>
<script type="text/javascript" src="js/sigUtil.js?version=<%=SessionAppVersion%>"></script>
<table border="0" cellspacing="0" cellpadding="0" style="width: 100%;">
        <tr class="h1title">
            <td>
                <span runat="server" id="heading" class="Phead">Choose Preferred SIG</span>
            </td>
        </tr>
        <tr>
            <td style="background-color: #868585">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessageHeader" runat="server" Visible="false" />
            </td>
        </tr>
        <tr class="h2title">
            <td class="Shead indnt">
                <asp:Label ID="lblMedInfo" CssClass="Shead indnt" runat="server" Text="Choose or write a SIG:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table cellspacing="0" style="width: 100%">
                    <tr class="h4title">
                        <td>
                            <asp:Panel ID="pnlButtons" runat="server">
                                <asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="btnstyle" OnClick="btnCancel_Click"
                                    CausesValidation="false" />
                                <input id="btnPatientEdu" runat="server" class="btnstyle" type="button" value="Patient Ed Sheet"
                                    onclick="btnPatientEdu_onclick(); return false;" />
                                <asp:Button ID="btnChangeMed" runat="server" CausesValidation="False" CssClass="btnstyle"
                                    OnClick="btnChangeMed_Click" Text="Change Med" />
                                <asp:Button ID="btnChooseDestination" runat="server" CssClass="btnstyle" OnClick="btnChooseDestination_Click"
                                    Text="Add to Script Pad ►" Width="170px" />
                                <asp:Button ID="btnAddReview" runat="server" CssClass="btnstyle" Enabled="True" OnClick="btnChooseDestination_Click"
                                    Text="Add & Review ►►" ToolTip="Add to script pad and review" Width="170px" />
                                <asp:Button ID="btnCheckRegistry" runat="server" CausesValidation="false" CssClass="btnstyle" Text="Check Registry" ToolTip="Click here to connect to your state’s Prescription Monitoring Program" Width="145px" />
                                <asp:CheckBox ID="chkRegistryChecked" runat="server" Text="State Registry Checked" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Panel ID="pnlSig" runat="server">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td colspan="3">
                                            <table>
                                                <tr width="100%" class="Titbarcolor4">
                                                    <td class="GridHeader" colspan="4" style="height: 17px">
                                                        &nbsp; &nbsp;<asp:RadioButton ID="rdgPrefer" runat="server" Text="Preferred" Checked="True"
                                                            GroupName="sig" />
                                                        &nbsp;<asp:RadioButton ID="rdbAllSig" runat="server" Text="All" GroupName="sig" />
                                                        <asp:RadioButton ID="rdbFreeTextSig" runat="server" Text="Write  Free Text SIG" GroupName="sig"
                                                            ToolTip="Create  your own SIG." />&nbsp;
                                                    </td>
                                                </tr>
                                                <tr bgcolor="#FFFFFF">
                                                    <td class="adminlink1" style="width: 8px">
                                                        &nbsp;
                                                    </td>
                                                    <td class="adminlink1" style="width: 8px">
                                                        &nbsp;
                                                    </td>
                                                    <td class="adminlink1" style="width: 714px">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr bgcolor="#FFFFFF">
                                                    <td class="adminlink1">
                                                        &nbsp;
                                                    </td>
                                                    <td class="adminlink1">
                                                        &nbsp;
                                                    </td>
                                                    <td class="adminlink1" style="width: 825px;">
                                                        <asp:Label ID="lblSigErrorMsg" runat="server" CssClass="SigErrorMsg" Visible="False"></asp:Label>
                                                        <asp:Panel ID="pnlPreferedSig" runat="server" Width="100%" Style="display: none">
                                                            <asp:ListBox ID="LstPreferedSig" runat="server" Width="750px" DataTextField="EnglishDescription"
                                                                DataValueField="SIGID" meta:resourcekey="LstPreferedSigResource1" OnPreRender="LstPreferedSig_PreRender"
                                                                onchange="setSelectToolTipOnSelect(this);" onmouseover="setSelectToolTip(this);" SelectionMode="Multiple">
                                                            </asp:ListBox>
                                                            <span style="color: Red">*</span>
                                                            <asp:ObjectDataSource ID="PrefSigObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                                                SelectMethod="GetSIGListForMedication" TypeName="Allscripts.Impact.Medication"
                                                                OnSelected="PrefSigObjectDataSource_Selected">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                        </asp:Panel>
                                                        <asp:Panel ID="pnlAllSig" runat="server" Width="100%" Style="display: none">
                                                            <asp:ListBox ID="LstLatinSig" runat="server" Width="147px" DataSourceID="LatinSigObjDataSource"
                                                                Style="display: inline" DataTextField="LatinSIGID" DataValueField="LatinSIGID"
                                                                AutoPostBack="True" OnSelectedIndexChanged="LstLatinSig_SelectedIndexChanged"
                                                                meta:resourcekey="LstLatinSigResource1">
                                                                <asp:ListItem meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                                            </asp:ListBox>
                                                            <span style="color: Red">*</span>
                                                            <asp:ListBox ID="LstSig" runat="server" Width="600px" DataSourceID="SigAllObjDataSource"
                                                                Style="display: inline" DataTextField="EnglishDescription" DataValueField="SIGID"
                                                                meta:resourcekey="LstSigResource1" onchange="setSelectToolTipOnSelect(this);"
                                                                onmouseover="setSelectToolTip(this);"></asp:ListBox>
                                                            <span style="color: Red">*</span>
                                                            <asp:ObjectDataSource ID="SigAllObjDataSource" runat="server" SelectMethod="GetSigsByLatinSigCode"
                                                                TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:ControlParameter ControlID="LstLatinSig" ConvertEmptyStringToNull="False" Name="LatinSIGID"
                                                                        PropertyName="SelectedValue" Type="String" />
                                                                    <asp:Parameter Name="DDI" Type="String" DefaultValue="" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                            <asp:ObjectDataSource ID="LatinSigObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                                                SelectMethod="GetLatinSIGCodes" TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:SessionParameter DefaultValue="" Name="ddi" SessionField="DDI" Type="String" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                        </asp:Panel>
                                                        <asp:Panel ID="pnlFreeTextSig" runat="server" Width="100%" Style="display: none">
                                                            <div id="divMaxCharacters" runat="server" class="normaltext">
                                                                (Maximum 1000 Characters / <span id="charsRemaining" runat="server" name="charsRemaining">
                                                                    1000</span> characters remaining)
                                                            </div>
                                                            <asp:TextBox ID="txtFreeTextSig" runat="server" Width="684px" onkeypress="replaceLeadingAndTrailingZeros(event, this);" TextMode="MultiLine"
                                                                meta:resourcekey="txtFreeTextSigResource1" Height="57px" MaxLength="140"></asp:TextBox>&nbsp;
                                                            <span style="color: Red">*</span>
                                                            <asp:CustomValidator ID="CvFreetext" runat="server" ControlToValidate="txtFreeTextSig"
                                                                ErrorMessage="Free Text should have fewer than 1000 characters" ClientValidationFunction="ClientValidate"
                                                                meta:resourcekey="CvFreetextResource1" EnableViewState="False" Enabled="False"></asp:CustomValidator>
                                                            <asp:RegularExpressionValidator ID="revFreeTextSig" runat="server" ControlToValidate="txtFreeTextSig"
                                                                ErrorMessage="Invalid SIG. Please enter a valid SIG." ValidationExpression="^([0-9a-zA-Z@.\s-,'()&%/]{1,1000})$"
                                                                Display="None"> </asp:RegularExpressionValidator>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#ffffff">
                                        <td class="adminlink1" style="height: 15px">
                                        </td>
                                        <td class="adminlink1" style="height: 15px">
                                        </td>
                                        <td class="adminlink1">
                                            <table border="0" cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td colspan="3" height="8">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblDaysSupplyAsterisk" runat="server" ForeColor="Red">*</asp:Label>
                                                        <asp:Label ID="lblDaysSupply" runat="server" Text="Days Supply:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDaysSupply" autocomplete="off" runat="server" MaxLength="3" Width="50px"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvDaysSupply" runat="server" ControlToValidate="txtDaysSupply"
                                                            Enabled="true" ErrorMessage="Please enter a value for Days Supply">*</asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="rvDaysSupply" runat="server" ControlToValidate="txtDaysSupply"
                                                            ErrorMessage="Days supply must be a whole number between 1 and 365" MaximumValue="365"
                                                            MinimumValue="1" Type="Integer">*</asp:RangeValidator>
                                                    </td>
                                                    <td>
                                                        <asp:Panel ID="pnlNonPillMed" runat="server" Width="100%" meta:resourcekey="pnlNonPillMedResource1">
                                                            <table style="font-size: 10pt; font-family: Arial" width="100%" id="TABLE1">
                                                                <tr>
                                                                    <td align="left" colspan="1" style="width: 144px;">
                                                                        Choose Package/Unit:
                                                                    </td>
                                                                    <td colspan="2">
                                                                        <asp:DropDownList ID="ddlCustomPack" runat="server" DataTextField="PackageDescription"
                                                                            Width="264px" DataValueField="PSPQ" meta:resourcekey="ddlCustomPackResource1"
                                                                            OnPreRender="ddlCustomPack_PreRender">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:ObjectDataSource ID="MedPackObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                                                SelectMethod="GetPackagesForMedication" TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="ddi" SessionField="DDI" Type="String" />
                                                                    <asp:Parameter Name="dosageFormCode" Type="String" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right">
                                                        <span style="color: Red">*</span>
                                                        <asp:Label ID="Label2" runat="server" Text="Quantity:" meta:resourcekey="Label2Resource1"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtQuantity" autocomplete="off" runat="server" MaxLength="8" Width="50px"
                                                            meta:resourcekey="txtQuantityResource1"></asp:TextBox>
                                                        <asp:DropDownList ID="ddlUnit" runat="server" Visible="false">
                                                            <asp:ListItem>ML</asp:ListItem>
                                                            <asp:ListItem>EA</asp:ListItem>
                                                            <asp:ListItem Value="UN">Units</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RegularExpressionValidator ID="revtxtQuantity" runat="server" ControlToValidate="txtQuantity">*</asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ID="ReqFdValQuantity" runat="server" ErrorMessage="Please enter quantity"
                                                            ControlToValidate="txtQuantity" Text="*"></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="text-align: right;">
                                                        <asp:Label ID="lblQuantity" runat="server" ForeColor="Red"></asp:Label>
                                                        <asp:TextBox ID="txtQuantityUnits" runat="server" Style='display: none; visibility: hidden;'
                                                            TabIndex="-1">0</asp:TextBox>
                                                        <asp:RangeValidator ID="qtyUnitRangeValidator" runat="server" ControlToValidate="txtQuantityUnits"
                                                            ErrorMessage="You are about to create a prescription that contains a quantity over 10,000 units.Please consider an alternate package size or review the prescription for accuracy."
                                                            MaximumValue="9999" MinimumValue="0" Type="Double" Display="None"></asp:RangeValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right;">
                                                        <span style="color: Red">*</span>
                                                        <asp:Label ID="lblRefills" runat="server" Text="Refills:" meta:resourcekey="Label1Resource1"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtRefill" autocomplete="off" runat="server" MaxLength="2" meta:resourcekey="txtRefillResource1"
                                                            Width="50px">0</asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="revRefillReq" runat="server" ControlToValidate="txtRefill"
                                                            ErrorMessage="Refills cannot be blank">*</asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="RangeValidatorRefill" runat="server" ControlToValidate="txtRefill"
                                                            ErrorMessage="Refills must be a whole number between 0 and 99" MaximumValue="99"
                                                            MinimumValue="0" Type="Integer">*</asp:RangeValidator>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkDAW" runat="server" Text="Dispense As Written" TextAlign="Left"
                                                            meta:resourcekey="chkDAWResource1" />
                                                        <asp:Label ID="lblMDD"  runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MDD:"  Visible="false"></asp:Label>
                                                        <asp:TextBox ID="txtMDD"  runat="server" MaxLength="20" autocomplete="off" Width="264px" Visible="false"></asp:TextBox>
                                                        <asp:Label ID="lblMDDError" runat="server" ForeColor="Red" style="display: none;" Text="Sig text + MDD text too long, please shorten sig and try again"></asp:Label>
                                                        <asp:CustomValidator ID="CvMDDText" runat="server" ControlToValidate="txtMDD"
                                                                ErrorMessage="Free Text should have fewer than 20 characters" ClientValidationFunction="ClientValidate"
                                                                meta:resourcekey="CvFreetextResource1" EnableViewState="False" Enabled="False" Display="Dynamic"></asp:CustomValidator>
                                                        <asp:RegularExpressionValidator ID="revMDDText" runat="server" ControlToValidate="txtMDD"
                                                                ErrorMessage="Invalid MDD. Please enter a valid MDD." ValidationExpression="^([0-9a-zA-Z@.\s-,()&%/]{1,20})$"
                                                                Display="Dynamic"></asp:RegularExpressionValidator>
                                                        <asp:Label ID="lblPerDay" runat="server" Text="&nbsp;&nbsp;Per Day" Visible="false"></asp:Label>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                    <td colspan="2">
                                                        <a id="iFCLink" runat="server" href="#">Library - Admin & Dosage</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right">
                                                        Provider:
                                                    </td>
                                                    <td colspan="2">
                                                        <%--<%= DelegateProviderName %>--%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <div id="maxCharacterWarning" runat="server">
                                                            <span class="Pheadblack">Special instructions to pharmacist</span> <span class="normaltext">
                                                                Note: should not be used for patient instructions or comments</span>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="height: 79px">
                                                        <asp:TextBox ID="txtPharmComments" runat="server" TextMode="MultiLine" Width="684px"
                                                            meta:resourcekey="txtCommentsResource1" Height="57px" MaxLength="210"></asp:TextBox>
                                                        <div id="maxCharacterRemaining" runat="server" class="normaltext">
                                                            (Maximum <span id="maxCharacters" runat="server">210</span> Characters / <span id="charsRemaining2"
                                                                runat="server" name="charsRemaining">210</span> characters remaining)
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="ClientValidate"
                                                            ControlToValidate="txtPharmComments" ErrorMessage="The Comments Field should have no more than 210 Characters"
                                                            Width="480px" Enabled="False"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Width="511px" />
                                                        <asp:Label ID="lblShim" runat="server" OnPreRender="lblShim_PreRender"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <span style="font-size: larger !important"><asp:Literal ID="litFreeTextError" Visible="False" Mode="PassThrough" runat="server" ></asp:Literal></span>
                                                        <br/>
                                                        <br/>
                                                        <br/>
                                                    </td>
                                                </tr>
                                            </table>
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
    <ePrescribe:MedicationHistoryCompletion ID="ucMedicationHistoryCompletion" runat="server"
        CurrentPage="NurseSig.aspx" OnOnMedHistoryComplete="ucMedicationHistoryCompletion_OnMedHistoryComplete" />
    <asp:Panel Style="display: none" ID="pnlCSWarning" runat="server">
        <table style="border-collapse: collapse; border: solid 1px black; background-color: White"
            cellpadding="5" cellspacing="20" width="600">
            <tr>
                <td>
                    You are attempting to respond to a controlled substance request. By law, this is
                    not permitted.
                    <br />
                    <br />
                    Click <b>“Yes”</b> below to print the prescription for faxing back to the pharmacy.
                    If you choose this selection, a prescription will be printed with the requesting
                    pharmacy name and fax number. You must sign the prescription and manually fax it
                    to the pharmacy requesting the renewal. A “Deny with new prescription to follow”
                    will be electronically sent to the pharmacy.
                    <br />
                    <br />
                    Click <b>“Contact Me”</b> below to deny the renewal request and inform the pharmacy
                    to contact your office by an alternate method (phone call or fax) to obtain approval.
                    A “Deny” will be sent to the pharmacy and in the comments field will contain the
                    following “Contact provider by alternate methods regarding controlled medications”.
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td align="center" style="padding-bottom: 5px">
                    <asp:Button ID="btnYes" runat="server" Text="Yes" OnClick="btnYes_Click" Width="125px"
                        CssClass="btnstyle" />
                    <asp:Button ID="btnContactMe" runat="server" Text="Contact Me" OnClick="btnContactMe_Click"
                        Width="125px" CssClass="btnstyle" />
                    <asp:Button ID="btnCancelCS" runat="server" Text="Cancel" Width="125px" CssClass="btnstyle" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
          <asp:HiddenField ID="hiddenPharmacyNote" Value="" ClientIDMode="Static" runat="server"></asp:HiddenField>
        <input id="defaultResponseType" runat="server" type="text" style="display: none;"
            value="N" />
        <input id="maxlenContainer" runat="server" type="text" style="display: none;" value="210" />
        <input id="sigType" runat="server" type="text" style="display: none;" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="modalCSWarningPopup" runat="server" TargetControlID="btnHiddenTrigger"
        PopupControlID="pnlCSWarning" BackgroundCssClass="modalBackground" DropShadow="false"
        CancelControlID="btnCancelCS" />
    &nbsp;&nbsp;
    <ePrescribe:IFC ID="ifcControl" runat="server" />
    <asp:Panel Style="display: none" ID="panelAd" runat="server">
        <div id="div4" runat="server" class="overlaymain">
            <div id="div7" runat="server" class="overlaysub1">
                <ePrescribe:AdControl ID="adControl" runat="server" Show="true" SkipTime="-1" DisplayMode="MODAL">
                </ePrescribe:AdControl>
                <br />
                <asp:Button ID="btnAdCancel" runat="server" Text="OK" CssClass="btnstyle" />
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenAd" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeAd" runat="server" BehaviorID="mpeAd" DropShadow="false"
        BackgroundCssClass="modalBackground" TargetControlID="hiddenAd" PopupControlID="panelAd"
        CancelControlID="btnAdCancel">
    </ajaxToolkit:ModalPopupExtender>
    <telerik:RadAjaxManager ID="RadAjaxManager2" runat="server" UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="LstLatinSig">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LstSig" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <asp:Image ID="Image4" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif"
            Style="border: 0; vertical-align: middle; text-align: center" />
    </telerik:RadAjaxLoadingPanel>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">  
   <%-- <asp:Panel ID="panelCovCopayHeader" class="accordionHeader" runat="server" Width="100%">
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
            <tbody>
                <tr>
                    <td align="left" width="140">
                        <div id="divCovCopay" class="accordionHeaderText">
                            Selected Medication</div>
                    </td>
                    <td align="right" width="14">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="panelCovCopay" class="accordionContentScriptPad" runat="server" Width="95%">
        <asp:Image ID="imgFormularyStatus" runat="server" ImageUrl="~/images/shim.gif" />
        <asp:Label ID="lblLevelOfPreferedness" runat="server" Font-Size="7"></asp:Label>&nbsp&nbsp
        <asp:Label ID="lblMedName" runat="server" Font-Bold="true"></asp:Label><br />
        <br />
        <asp:Label ID="lblCovCopay" runat="server"></asp:Label>
    </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeCovCopay" runat="server" TargetControlID="panelCovCopay"
        Collapsed="false" CollapsedSize="0" ExpandControlID="panelCovCopayHeader" CollapseControlID="panelCovCopayHeader"
        ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png"
        ImageControlID="covcopayclpsimg" SuppressPostBack="true">
    </ajaxToolkit:CollapsiblePanelExtender>
    --%>
</asp:Content>
