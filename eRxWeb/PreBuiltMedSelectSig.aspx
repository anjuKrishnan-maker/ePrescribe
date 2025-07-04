<%@ Page Title="Select Sig" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" CodeBehind="PreBuiltMedSelectSig.aspx.cs" Inherits="eRxWeb.PreBuiltMedSelectSig" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/FactsAndComparison.ascx" TagName="IFC" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="js/sigUtil.js?version=<%=SessionAppVersion%>"></script>
    <table border="0" cellspacing="0" cellpadding="2" style="width: 100%;">
        <tr class="h1title">
            <td>
                &nbsp;<span runat="server" id="heading" class="Phead">Edit Prescription Group</span>
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
                        <td></td>
                        <td colspan="5">
                            <asp:Panel ID="pnlButtons" runat="server">
                                <asp:Button ID="btnBack" runat="server" CausesValidation="False" CssClass="btnstyle"
                                    Text="Back" OnClick="btnBack_Click" />
                                &nbsp    
                                <asp:Button ID="btnSave" runat="server" CssClass="btnstyle" Enabled="True" OnClick="btnSave_Click"
                                    Text="Save" ToolTip="Click to save the pre-built med." />
                                &nbsp                      
                                <asp:Button ID="btnAddAnotherMed" runat="server" CssClass="btnstyle" Enabled="True" OnClick="btnAddAnotherMed_Click"
                                    Text="Save and Add Another" ToolTip="Click to save and add another pre-built med." Width="170px" />
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
                                                    <td class="GridHeader" colspan="4" style="height: 17px">&nbsp; &nbsp;<asp:RadioButton ID="rdgPrefer" runat="server" Text="Preferred" Checked="True"
                                                        GroupName="sig" />
                                                        &nbsp;<asp:RadioButton ID="rdbAllSig" runat="server" Text="All" GroupName="sig" />
                                                        <asp:RadioButton ID="rdbFreeTextSig" runat="server" Text="Write  Free Text SIG" GroupName="sig"
                                                            ToolTip="Create  your own SIG." />&nbsp;
                                                    </td>
                                                </tr>
                                                <tr bgcolor="#FFFFFF">
                                                    <td class="adminlink1">&nbsp;
                                                    </td>
                                                    <td class="adminlink1">&nbsp;
                                                    </td>
                                                    <td class="adminlink1" style="width: 825px;">
                                                        <asp:Label ID="lblSigErrorMsg" runat="server" CssClass="SigErrorMsg" Visible="False"></asp:Label>
                                                        <asp:Panel ID="pnlPreferedSig" runat="server" Width="100%" Style="display: none">
                                                            <asp:ListBox ID="LstPreferedSig" runat="server" Width="750px" DataTextField="EnglishDescription"
                                                                DataValueField="SIGID" meta:resourcekey="LstPreferedSigResource1" OnPreRender="LstPreferedSig_PreRender"
                                                                onchange="setSelectToolTipOnSelect(this);" onmouseover="setSelectToolTip(this);"></asp:ListBox>
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
                                                            <table style="border-collapse: collapse; border-spacing: 0">
                                                                <tr>
                                                                    <td style="margin: 0; padding: 10; border: 0">
                                                                        <asp:ListBox ID="LstLatinSig" runat="server" Width="140px" DataSourceID="LatinSigObjDataSource"
                                                                            Style="display: inline" DataTextField="LatinSIGID" DataValueField="LatinSIGID"
                                                                            AutoPostBack="True" OnSelectedIndexChanged="LstLatinSig_SelectedIndexChanged"
                                                                            meta:resourcekey="LstLatinSigResource1">
                                                                            <asp:ListItem meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                                                        </asp:ListBox>
                                                                    </td>
                                                                    <td style="margin: 0; padding: 10; border: 0">
                                                                        <asp:ListBox ID="LstSig" runat="server" Width="600px" DataSourceID="SigAllObjDataSource"
                                                                            Style="display: inline" DataTextField="EnglishDescription" DataValueField="SIGID"
                                                                            meta:resourcekey="LstSigResource1" onchange="setSelectToolTipOnSelect(this);"
                                                                            onmouseover="setSelectToolTip(this);"></asp:ListBox>
                                                                    </td>
                                                                    <td style="margin: 0; padding: 10; border: 0">
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
                                                                (Maximum 1000 Characters / <span id="charsRemaining" runat="server" name="charsRemaining">1000</span> characters remaining)
                                                            </div>
                                                            <asp:TextBox ID="txtFreeTextSig" runat="server" Width="684px" TextMode="MultiLine"
                                                                meta:resourcekey="txtFreeTextSigResource1" Height="57px" MaxLength="1000"></asp:TextBox>&nbsp;
                                                            <span style="color: Red">*</span>
                                                            <asp:CustomValidator ID="CvFreetext" runat="server" ControlToValidate="txtFreeTextSig"
                                                                ErrorMessage="Free Text should have fewer than 1000 characters" ClientValidationFunction="ClientValidate"
                                                                meta:resourcekey="CvFreetextResource1" EnableViewState="False" Enabled="False"></asp:CustomValidator>
                                                            <asp:RegularExpressionValidator ID="revFreeTextSig" runat="server" ControlToValidate="txtFreeTextSig"
                                                                ErrorMessage="Invalid SIG. Please enter a valid SIG." ValidationExpression="^([0-9a-zA-Z@.\s-,()&%/]{1,1000})$"
                                                                Display="None"> </asp:RegularExpressionValidator>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr bgcolor="#ffffff">
                                        <td class="adminlink1" style="height: 15px"></td>
                                        <td class="adminlink1" style="height: 15px"></td>
                                        <td class="adminlink1">
                                            <table border="0" cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td colspan="3" height="8"></td>
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
                                                                    <td style="text-align: right">Choose Package/Unit:
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
                                                        <asp:TextBox ID="txtQuantity" autocomplete="off" runat="server" MaxLength="8" Width="50px"
                                                            meta:resourcekey="txtQuantityResource1"></asp:TextBox>
                                                        <asp:DropDownList ID="ddlUnit" runat="server" Visible="false">
                                                            <asp:ListItem>ML</asp:ListItem>
                                                            <asp:ListItem>EA</asp:ListItem>
                                                            <asp:ListItem Value="UN">Units</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RegularExpressionValidator ID="revtxtQuantity" runat="server" ControlToValidate="txtQuantity"
                                                            Display="Dynamic" ErrorMessage="Quantity must be between 0.0001 and 999.9999" ValidationExpression="(?!^0*$)(?!^0*\.0*$)^\d{0,3}(\.\d{1,4})?$">*</asp:RegularExpressionValidator>
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
                                                            meta:resourcekey="chkDAWResource1" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td colspan="2">
                                                        <a id="iFCLink" runat="server" href="#">Library - Admin & Dosage</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 80px" colspan="3">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Width="511px" meta:resourcekey="ValidationSummary1Resource1" />
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
    <input id="sigType" runat="server" type="text" style="display: none;" />
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
    <ajaxToolkit:ModalPopupExtender ID="mpeAd" runat="server" BehaviorID="mpeAd" DropShadow="false"
        BackgroundCssClass="modalBackground" TargetControlID="btnHiddenTrigger" PopupControlID="panelAd"
        CancelControlID="btnAdCancel">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
</asp:Content>

<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">

</asp:Content>