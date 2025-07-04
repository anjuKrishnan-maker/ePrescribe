<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Sig"
    ViewStateEncryptionMode="Never" EnableViewStateMac="false" Title="Choose Sig"
    MasterPageFile="~/PhysicianMasterPage.master" Culture="auto" meta:resourcekey="PageResource1"
    UICulture="auto" Codebehind="Sig.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/MedicationHistoryCompletion.ascx" TagName="MedicationHistoryCompletion"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/FactsAndComparison.ascx" TagName="IFC" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning"
    TagPrefix="ePrescribe" %>
<%@ Register Src="~/Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed"
    TagPrefix="ePrescribe" %>
<%--<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing" TagPrefix="ePrescribe" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="js/sigUtil.js?version=<%=SessionAppVersion%>"></script>
    <script type="text/javascript">
        function OpenNewWindow(url) {
            window.open(url);
        }
    </script>
    <table border="0" cellspacing="0" cellpadding="2" style="width: 100%;">
        <tr class="h1title">
            <td>
                <span runat="server" id="heading" class="Phead">Choose Preferred SIG</span>
                <ePrescribe:Message ID="ucMessageHeader" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessageRxHeader" runat="server" Visible="false"/>
            </td>
        </tr>
        <tr class="h2title">
            <td class="Shead indnt">
                <asp:Label CssClass="Shead indnt" ID="lblMedInfo" runat="server" Text="Choose or write a SIG"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table cellspacing="0" style="width: 100%">
                    <tr class="h4title">
                        <td>
                        </td>
                        <td colspan="5">
                            <asp:Panel ID="pnlButtons" runat="server">
                                <asp:Button ID="btnCancel" runat="server" CausesValidation="False" CssClass="btnstyle"
                                    OnClick="btnCancel_Click" Text="Back" />
                                <input id="btnPatientEdu" runat="server" class="btnstyle" type="button" value="Patient Ed Sheet"
                                    onclick="return btnPatientEdu_onclick()" style="width: 111px" />
                                <asp:Button ID="btnChangeMed" runat="server" CausesValidation="False" CssClass="btnstyle"
                                    OnClick="btnChangeMed_Click" Text="Change Med" />
                                <asp:Button ID="btnChooseDestination" runat="server" CssClass="btnStyleOneArrow" OnClick="btnChooseDestination_Click"
                                    Text="Add to Script Pad" Width="170px" />
                                <asp:Button ID="btnAddReview" runat="server" CssClass="btnStyleTwoArrows" Enabled="True" OnClick="btnChooseDestination_Click"
                                    Text="Add & Review" ToolTip="Add to script pad and review" Width="170px" />
                                <asp:Button ID="btnCheckRegistry" runat="server" CausesValidation="false" CssClass="btnstyle" Text="Check Registry" ToolTip="Click here to connect to your state’s Prescription Monitoring Program" Width="145px" />
                                <asp:CheckBox ID="chkRegistryChecked" runat="server" Text="State Registry Checked" Visible="false"/>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0" width="100%">
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
                                                            <asp:ListBox ID="LstPreferedSig"  runat="server" CssClass="listSelect" Width="750px" DataTextField="EnglishDescription"
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
                                                            <table style="border-collapse:collapse; border-spacing:0">
                                                                <tr>
                                                                    <td style="margin:0; padding:10; border:0">
                                                                        <asp:ListBox ID="LstLatinSig" CssClass="listSelect" runat="server" Width="140px" DataSourceID="LatinSigObjDataSource"
                                                                            Style="display: inline" DataTextField="LatinSIGID" DataValueField="LatinSIGID"
                                                                            AutoPostBack="True" OnSelectedIndexChanged="LstLatinSig_SelectedIndexChanged"
                                                                            meta:resourcekey="LstLatinSigResource1">
                                                                            <asp:ListItem meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                                                        </asp:ListBox>
                                                                    </td>
                                                                    <td style="margin:0; padding:10; border:0">
                                                                        <asp:ListBox ID="LstSig" runat="server" CssClass="listSelect" Width="600px" DataSourceID="SigAllObjDataSource"
                                                                            Style="display: inline" DataTextField="EnglishDescription" DataValueField="SIGID"
                                                                            meta:resourcekey="LstSigResource1" onchange="setSelectToolTipOnSelect(this);"
                                                                            onmouseover="setSelectToolTip(this);"></asp:ListBox>
                                                                    </td>
                                                                    <td style="margin:0; padding:10; border:0">
                                                                        <span style="color: Red">*</span>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:ObjectDataSource ID="SigAllObjDataSource" runat="server" SelectMethod="GetSigsByLatinSigCode"
                                                                TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:ControlParameter ControlID="LstLatinSig" ConvertEmptyStringToNull="False" Name="LatinSIGID"
                                                                        PropertyName="SelectedValue" Type="String" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                            <asp:ObjectDataSource ID="LatinSigObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                                                SelectMethod="GetLatinSIGCodes" TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                                </SelectParameters>
                                                            </asp:ObjectDataSource>
                                                        </asp:Panel>
                                                        <asp:Panel ID="pnlFreeTextSig" runat="server" Width="100%" Style="display: none">
                                                            <div id="divMaxCharacters" runat="server" class="normaltext">
                                                                (Maximum 1000 Characters / <span id="charsRemaining" runat="server" name="charsRemaining">
                                                                    1000</span> characters remaining)
                                                                 <asp:Label ID="SigLengthError" runat="server" ForeColor="Red" style="display: none;" Text="Sig Text too long, please shorten"></asp:Label>
                                                            </div>
                                                            <asp:TextBox ID="txtFreeTextSig" runat="server" Width="684px" TextMode="MultiLine"
                                                                meta:resourcekey="txtFreeTextSigResource1" Height="57px" onkeypress="replaceLeadingAndTrailingZeros(event, this);" MaxLength="1000"></asp:TextBox>&nbsp;
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
                                                    <td style="text-align: right">
                                                        <asp:Label ID="lblDaysSupplyAsterisk" runat="server" ForeColor="Red">*</asp:Label>
                                                        <asp:Label ID="lblDaysSupply" runat="server" Text="Days Supply:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDaysSupply" runat="server" MaxLength="3" autocomplete="off" Width="50px"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvDaysSupply" runat="server" ControlToValidate="txtDaysSupply"
                                                            ErrorMessage="Please enter a value for Days Supply" Enabled="true">*</asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="rvDaysSupply" runat="server" ControlToValidate="txtDaysSupply"
                                                            ErrorMessage="Days supply must be a whole number between 1 and 365" MaximumValue="365"
                                                            MinimumValue="1" Type="Integer">*</asp:RangeValidator>
                                                    </td>
                                                    <td>
                                                        <asp:Panel ID="pnlNonPillMed" runat="server" Width="100%" meta:resourcekey="pnlNonPillMedResource1">
                                                            <table id="TABLE1">
                                                                <tr>
                                                                    <td style="text-align: right">
                                                                        Choose Package/Unit:
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlCustomPack" runat="server" DataTextField="PackageDescription"
                                                                            Width="264px" DataValueField="PSPQ" meta:resourcekey="ddlCustomPackResource1"
                                                                            OnPreRender="ddlCustomPack_PreRender">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:ObjectDataSource ID="MedPackObjectDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                                                ConvertNullToDBNull="False" SelectMethod="GetPackagesForMedication" TypeName="Allscripts.Impact.Medication">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter Name="ddi" SessionField="DDI" Type="String" />
                                                                    <asp:Parameter Name="dosageFormCode" Type="String" />
                                                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                                    <asp:SessionParameter Name="userID" SessionField="USERID" Type="String" />
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
                                                        <asp:TextBox ID="txtQuantity" autocomplete="off" runat="server" MaxLength="9" Width="50px"
                                                            meta:resourcekey="txtQuantityResource1"></asp:TextBox>
                                                        <asp:DropDownList ID="ddlUnit" runat="server" Visible="false">
                                                            <asp:ListItem>ML</asp:ListItem>
                                                            <asp:ListItem>EA</asp:ListItem>
                                                            <asp:ListItem Value="UN">Units</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RegularExpressionValidator ID="revtxtQuantity" runat="server" ControlToValidate="txtQuantity" Display="Dynamic" >*</asp:RegularExpressionValidator>
                                                        <asp:RequiredFieldValidator ID="ReqFdValQuantity" runat="server" ErrorMessage="Please enter a quantity"
                                                            ControlToValidate="txtQuantity" Text="*"></asp:RequiredFieldValidator>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblQuantity" runat="server" ForeColor="Red"></asp:Label>
                                                        <asp:TextBox ID="txtQuantityUnits" runat="server" Style="display: none; visibility: hidden"
                                                            TabIndex="-1">0</asp:TextBox>
                                                        <asp:RangeValidator ID="qtyUnitRangeValidator" runat="server" ControlToValidate="txtQuantityUnits"
                                                            Display="None" ErrorMessage="You are about to create a prescription that contains a quantity over 10,000 units.Please consider an alternate package size or review the prescription for accuracy."
                                                            MaximumValue="9999" MinimumValue="0" Type="Double"></asp:RangeValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="text-align: right">
                                                        <span style="color: Red">*</span>
                                                        <asp:Label ID="lblRefills" runat="server" Text="Refills:" meta:resourcekey="Label1Resource1"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtRefill" autocomplete="off" runat="server" MaxLength="2" meta:resourcekey="txtRefillResource1"
                                                            Width="50px"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="revRefillReq" runat="server" ControlToValidate="txtRefill"
                                                            ErrorMessage="Please enter a value for refills">*</asp:RequiredFieldValidator>
                                                        <asp:RangeValidator ID="RangeValidatorRefill" runat="server" ControlToValidate="txtRefill"
                                                            ErrorMessage="Refills must be a whole number between 0 and 99" MaximumValue="99"
                                                            MinimumValue="0" Type="Integer">*</asp:RangeValidator>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkDAW" runat="server" Text="Dispense As Written: " TextAlign="Left"
                                                            meta:resourcekey="chkDAWResource1"  />
                                                        <asp:Label ID="lblMDD" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; MDD:" Visible="false"></asp:Label>
                                                        
                                                        <asp:TextBox ID="txtMDD" runat="server" autocomplete="off" MaxLength="20" Visible="false" Width="264px"></asp:TextBox>
                                                        <asp:Label ID="lblPerDay" runat="server" Text="Per Day" Visible="false"></asp:Label>
                                                        <asp:Label ID="lblMDDError" runat="server" ForeColor="Red" style="display: none;" Text="Sig text + MDD text too long, please shorten sig and try again"></asp:Label>
                                                            <asp:CustomValidator ID="CvMDDText" runat="server" ControlToValidate="txtMDD"
                                                                ErrorMessage="Free Text should have fewer than 20 characters" ClientValidationFunction="ClientValidate"
                                                                meta:resourcekey="CvFreetextResource1" EnableViewState="False" Enabled="False" Display="Dynamic"></asp:CustomValidator>
                                                            <asp:RegularExpressionValidator ID="revMDDText" runat="server" ControlToValidate="txtMDD"
                                                                ErrorMessage="Invalid MDD. Please enter a valid MDD." ValidationExpression="^([0-9a-zA-Z@.\s-,()&%/]{1,20})$"
                                                                Display="Dynamic"></asp:RegularExpressionValidator>
                                                        
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
                                                    <td colspan="3">
                                                        <div id="maxCharacterWarning" runat="server" style="margin-top: 10px;">
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
                                                                runat="server">210</span> characters remaining)
                                                        </div>
                                                        <asp:RegularExpressionValidator ID="revFreeFormMed" runat="server" ControlToValidate="txtPharmComments"
                                                            ErrorMessage="Invalid comments. Please enter valid comments." ValidationExpression="^([0-9a-zA-Z@.\s-:,()&%/]{1,210})$"
                                                            Display="None">
                                                        </asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 80px" colspan="3">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Width="511px" meta:resourcekey="ValidationSummary1Resource1" />
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
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
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

    <input id="defaultResponseType" runat="server" type="text" style="display: none;"
        value="N" />
    <input id="maxlenContainer" runat="server" type="text" style="display: none;" value="210" />
    <input id="sigType" runat="server" type="text" style="display: none;" />
    <ePrescribe:IFC ID="ifcControl" runat="server" />
    <ePrescribe:MedicationHistoryCompletion ID="ucMedicationHistoryCompletion" runat="server"
        CurrentPage="Sig.aspx" OnOnMedHistoryComplete="ucMedicationHistoryCompletion_OnMedHistoryComplete" />
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="false" />
    <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server" />
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
    <ajaxToolkit:ModalPopupExtender ID="mpeAd" runat="server" BehaviorID="mpeAd" DropShadow="false"
        BackgroundCssClass="modalBackground" TargetControlID="btnHiddenTrigger" PopupControlID="panelAd"
        CancelControlID="btnAdCancel">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
    <asp:HiddenField ID="isNewRxWorkflow" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="hiddenPharmacyNote" Value="" ClientIDMode="Static" runat="server"></asp:HiddenField>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <!-- start script pad-->    
    <!-- end script pad-->
    <!--start cov/copay panel-->
    <asp:Panel ID="panelCovCopayHeader" class="accordionHeader" runat="server" Width="100%">
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
    <!--end cov/copay panel-->   
</asp:Content>
