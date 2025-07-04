<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.SiteManagement" Title="Site Management" Codebehind="SiteManagement.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <script type="text/javascript" language="javascript" ></script>
    <table width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr class="h1title">
            <td style="padding: 3px">
                <span class="Phead indnt">Site Management</span>
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" border="0" cellspacing="0" cellpadding="3">
                    <tr class="h4title" align="left">
                        <td>
                            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_OnClick" CssClass="btnstyle" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>     
        <tr>
            <td>
                <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableOutsideScripts="True"
                    EnableAJAX="true">
                    <AjaxSettings>
                        <telerik:AjaxSetting AjaxControlID="gvSite">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="gvSite"  />
                                <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                                <telerik:AjaxUpdatedControl ControlID="divValsum" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="btnUniversal">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                    </AjaxSettings>
                </telerik:RadAjaxManager>
                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                            <telerik:RadGrid ID="gvSite" runat="server" GridLines="None" 
                               AllowAutomaticUpdates="true" EnableViewState="true" AutoGenerateColumns="False"
                                DataSourceID="odsLicense" Skin="Allscripts" EnableEmbeddedSkins="false" oninsertcommand="gvSite_InsertCommand" 
                                onitemcommand="gvSite_ItemCommand" onitemcreated="gvSite_ItemCreated" 
                                onitemdatabound="gvSite_ItemDataBound" onneeddatasource="gvSite_NeedDataSource" 
                                onupdatecommand="gvSite_UpdateCommand" OnCancelCommand= "gvSite_CancelCommand" OnEditCommand= "gvSite_EditCommand" >
                             
                                <MasterTableView DataSourceID="odsLicense" CommandItemDisplay="Top" DataKeyNames="SiteID" AllowAutomaticUpdates="false">
                                    <CommandItemTemplate>
                                        <div style="padding:10px">
                                            <asp:LinkButton Style="vertical-align: bottom" ID="btnAddNew" runat="server" CommandName="InitInsert"
                                                CausesValidation="false" Font-Bold="true"><img style="border:0px;vertical-align:middle;" alt="" src="images/Insert.gif" /> Add New Site</asp:LinkButton>
                                        </div>
                                    </CommandItemTemplate>
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                    
                                        <telerik:GridEditCommandColumn ButtonType="ImageButton" EditImageUrl="images/Edit.gif" UpdateImageUrl="Images\Update.gif"
                                            CancelImageUrl="Images\Cancel.gif">
                                            <HeaderStyle Width="30px" />
                                        </telerik:GridEditCommandColumn>
                                       <telerik:GridBoundColumn DataField="PracticeName" UniqueName="colSiteName" HeaderText="Site Name" Visible= "false" ReadOnly="true">
                                                                                   
                                        </telerik:GridBoundColumn>                                                                            
                                         <telerik:GridTemplateColumn UniqueName="colSiteInfo" HeaderText="Site Name" Display="true" Visible="true">
                                            <EditItemTemplate>                                                
                                                <asp:textbox ID="txtPracticeName" runat="server" Text='<%# Bind("PracticeName") %>' visible="true"  MaxLength="70" />
                                                <asp:CheckBox ID="chkInactiveSite" runat="server" Text="Inactive" Visible="false"/>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Address1" UniqueName="colAddress1" HeaderText="Address"
                                            MaxLength="35">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Address2" UniqueName="colAddress2" HeaderText="&nbsp;&nbsp;"
                                            Display="False" MaxLength="35">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="City" UniqueName="colCity" HeaderText="City" Display="False"
                                            MaxLength="35">
                                        </telerik:GridBoundColumn>
                                        
                                        <telerik:GridDropDownColumn ListTextField="State" DataField="State" UniqueName="colState" DropDownControlType="RadComboBox" DataSourceID="odsStates"
                                            ListValueField="State" HeaderText="State" Display="True" Visible= "true" >
                                        </telerik:GridDropDownColumn>
                                        <telerik:GridBoundColumn DataField="Active" UniqueName="colSiteStatus" HeaderText="Status" ReadOnly="true"
                                            MaxLength="20">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn Text="Edit Pharmacy Favorites" CommandName="EditPharmFavs"
                                            UniqueName="colEditPharmFavs" Display="true" ButtonType="linkButton">
                                        </telerik:GridButtonColumn>
                                        <telerik:GridBoundColumn DataField="ZipCode" UniqueName="colZIP" HeaderText="ZIP Code"
                                            Display="False" MaxLength="9">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn UniqueName="colPhone" HeaderText="Phone" Display="false">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPhone" runat="server" onkeypress="return numericKeyPressOnly(this, event);" onfocus="parseNumberInput(this)" onblur="formatPhoneInput(this);ValidatorOnChange(event);"></asp:TextBox>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colFax" HeaderText="Fax" Display="false">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtFax" runat="server" onkeypress="return numericKeyPressOnly(this, event);" onfocus="parseNumberInput(this)" onblur="formatPhoneInput(this);ValidatorOnChange(event);"></asp:TextBox>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridDropDownColumn ListTextField="" DataField="" UniqueName="colTimeZone"
                                            ListValueField="" HeaderText="Time Zone" Display="False">
                                        </telerik:GridDropDownColumn>
                                        <telerik:GridTemplateColumn UniqueName="colAdmin" Display="false" HeaderText="&nbsp;">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkAllowAdmin" runat="server" Text="Allow Veradigm Remote Access" /><br />
                                                <asp:CheckBox ID="chkGenericEquivalentSearch" runat="server" Text="Perform Generic Equivalent Searches" Checked="true"
                                                    ToolTip="This feature allows generic equivalents and generic names to be returned when a brand is searched" />
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colFinancialOffers" HeaderText="&nbsp;" Display="false">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkFinancialOffers" runat="server" Text="Show and apply Branded Rx Discount Offers" />
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colInfoScripts" HeaderText="&nbsp;" Display="false">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkInfoScripts" runat="server" Text="Allow InfoScripts" Checked="true" />
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                         <telerik:GridTemplateColumn UniqueName="colPatientReceipt" HeaderText="&nbsp;" Display="false">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkAllowPatientReceipt" runat="server" Text="Allow Patient Informational Copy" />
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colAllowMDD" HeaderText="&nbsp;" Display="false">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="chkAllowMDD" runat="server" Text="Allow Maximum Daily Dose" AutoPostBack="True" OnCheckedChanged="chkMDDCheckBoxChange"/>
                                                <div span style="padding-left:20px">
                                                 <asp:RadioButtonList ID="rboMDDOption" runat="server" RepeatDirection="Horizontal" >
                                                    <asp:ListItem Text="Controlled Medications Only" Value="CS" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="All Medications" Value="AL"></asp:ListItem>
                                                </asp:RadioButtonList>
                                                    </div>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colDefaultSite" HeaderText="&nbsp;" ItemStyle-Width="5px">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblDefaultSite" runat="server" />
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colPrintingPaperOption" HeaderText="Printing Paper Preference"
                                            Display="False">
                                            <EditItemTemplate>
                                                <asp:RadioButtonList ID="rdoPaperOption" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem Text="Plain Paper" Value="P" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Security Paper" Value="S"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn UniqueName="colPrintingOption" HeaderText="Printing Preference"
                                            Display="False">
                                            <EditItemTemplate>
                                                <asp:RadioButtonList ID="rdoPrintingOption" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem Text="1Up" Value="1" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="4Up" Value="4"></asp:ListItem>
                                                    <asp:ListItem Text="4Row" Value="R"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </EditItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                    <EditFormSettings ColumnNumber="2" EditFormType="AutoGenerated">
                                        <FormTableItemStyle Wrap="False" HorizontalAlign="Left"></FormTableItemStyle>
                                        <FormCaptionStyle CssClass="EditFormHeader"></FormCaptionStyle>
                                        <FormMainTableStyle GridLines="Horizontal" CellSpacing="5" CellPadding="5" HorizontalAlign="Center"
                                            Width="95%" />
                                        <FormTableStyle CellSpacing="0" CellPadding="2" CssClass="module" Height="110px"
                                            BackColor="White" HorizontalAlign="Left" />
                                        <FormTableAlternatingItemStyle Wrap="False"></FormTableAlternatingItemStyle>
                                        <EditColumn EditFormColumnIndex="1" ButtonType="PushButton"
                                            CancelText="Cancel" InsertText="Add Site" UpdateText="Update" UniqueName="EditCommandColumn1">
                                            <FooterStyle HorizontalAlign="Left" />
                                        </EditColumn>
                                        <FormTableButtonRowStyle HorizontalAlign="Right" BackColor="#CCCCCC"></FormTableButtonRowStyle>
                                    </EditFormSettings>
                                    <ExpandCollapseColumn Visible="False" Resizable="False">
                                        <HeaderStyle Width="20px"></HeaderStyle>
                                    </ExpandCollapseColumn>
                                    <RowIndicatorColumn Visible="False">
                                        <HeaderStyle Width="20px"></HeaderStyle>
                                    </RowIndicatorColumn>
                                </MasterTableView>
                            </telerik:RadGrid>
                            <div id="divValsum" runat="server">
                            
                            <asp:ValidationSummary ID="valsum" runat="server"  ValidationGroup="addingValidationGroup"/>
                            <asp:ValidationSummary ID="valsumEdit" runat="server"   ValidationGroup="editingValidationGroup"/>
                            </div>
                            <asp:ObjectDataSource ID="odsLicense" runat="server" TypeName="Allscripts.Impact.ApplicationLicense"
                                SelectMethod="Load">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseKey" SessionField="LICENSEID" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                             <asp:ObjectDataSource ID="odsStates" runat="server" TypeName="Allscripts.Impact.RxUser"
                                SelectMethod="ChGetState">
                                <SelectParameters>
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 5px; padding-top: 20px">
                            <asp:Panel ID="userInfoPanelHeader" runat="server" BackColor="LightGray" Width="100%"
                                BorderColor="black" BorderWidth="1px">
                                <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="padding: 5px">
                                            <b>Universal Account Settings</b>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 15px">
                            <table>
                                <tr>
                                    <td>
                                        Account Name:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLicenseName" runat="server" Width="265px" MaxLength="50"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvLicenseName" runat="server" ControlToValidate="txtLicenseName"
                                        ErrorMessage="Please enter an Account Name.">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Preferences:
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkShowCheckedInPatients" runat="server" Text="Show Patient List" />
                                        <br />
                                        <asp:CheckBox ID="chkShowRxInfo" runat="server" visible ="false" Text="Show RxInfo" />
                                        <br />                                            
                                        <asp:CheckBox ID="chkShowPrebuiltRx" runat="server" visible ="false" Text="Show Pre-Built Prescriptions" />
                                        <br />                                            
                                        <asp:CheckBox ID="chkShowRePA" runat="server" visible ="false" Text="Show Retrospective ePA" />
                                    </td>
                                </tr>
                                <tr id="rowPdmp" runat="server" visible ="false">
                                    <td>
                                        Prescription Drug Monitoring Program (PDMP) :
                                    </td>
                                    <td>
                                        <asp:Label ID="lblPDMP" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                       <asp:Label ID="lblMedicationReferrenceSearch" Visible="false" runat="server">Medication Reference Search:</asp:Label>
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rdBtnWK" runat="server" Text="Facts & Comparisons" Visible="false" Checked="true" GroupName="MedicationReferrenceSearch" />
                                        <asp:RadioButton ID="rdBtnLexicomp" runat="server" Text="Lexicomp" Visible="false" GroupName="MedicationReferrenceSearch" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 15px; padding-bottom: 15px">
                            <asp:Button ID="btnUniversal" runat="server" CssClass="btnstyle" OnClick="btnUniversal_Click"
                                Text="Save" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
    </asp:UpdatePanel>
</asp:Content>
