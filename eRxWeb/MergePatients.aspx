<%@ Page Language="C#" MasterPageFile="~/physicianmasterpageblank.master" AutoEventWireup="true" Inherits="eRxWeb.MergePatients" Title="Merge Patients" Codebehind="MergePatients.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
            var prevRow1;
            var prevRow2;    

            function onRowClick1(row1, isRestricted)
            {
                var hiddenPat1Restriction = document.getElementById("<%=hiddenPat1Restriction.ClientID %>");
                if (isRestricted == "1")
                {
                    hiddenPat1Restriction.value = "1";
                }
                else {
                    hiddenPat1Restriction.value = "0";
                }
                if(prevRow1!=null)
                {
                    prevRow1.className=savedClass;
                }
                savedClass=row1.className;
                row1.className='SelectedRow';
                if (prevRow1 == null || prevRow1 != row1)
                {	
		            prevRow1=row1;
		    
		            if (row1.childNodes != null)
		            {
	                    if (row1.childNodes[0].childNodes[0] != null)
	                    {
	                        row1.childNodes[0].childNodes[0].checked = true;
		                }
		            }
	            }
            }
    
            function onRowClick2(row2, isRestricted)
            {
                var hiddenPat2Restriction = document.getElementById("<%=hiddenPat2Restriction.ClientID %>");
                if (isRestricted == "1")
                {
                    hiddenPat2Restriction.value = "1";
                }
                else
                {
                    hiddenPat2Restriction.value = "0";
                }
                if(prevRow2!=null)
                {
                    prevRow2.className=savedClass;
                }
                savedClass=row2.className;
                row2.className='SelectedRow';
                if (prevRow2 == null || prevRow2 != row2)
                {	
		            prevRow2=row2;
		    
		            if (row2.childNodes != null)
		            {
	                    if (row2.childNodes[0].childNodes[0] != null)
	                    {
	                        row2.childNodes[0].childNodes[0].checked = true;
		                }
		            }
	            }
            }
    
            function ValidateChecked(oSrc, args)
            {
                var chkConfirm = document.getElementById("<%=chkConfirm.ClientID%>");
        
                if(chkConfirm.checked == false)
                {
                    alert("You must agree to the merge confirmation agreement.");
                    args.IsValid = false;
                }
            }

            function cancel() 
            {
                markChkConfirmAsChecked();

                var hiddenCancel = document.getElementById("<%=btnHiddenCancel.ClientID %>");
                if (hiddenCancel != null) {
                    hiddenCancel.click();
                }
            }

            function markChkConfirmAsChecked()
            {
                var chkConfirm = document.getElementById("<%=chkConfirm.ClientID%>");
        
                if (chkConfirm != null)
                {
                    chkConfirm.checked = true;
                }
            }
    
            function CheckEnter1_Onclick(e)
            {
                var key;
                key=(e.which) ? e.which : e.keyCode;
                if(key==13)
                {
                    document.getElementById('<%=btnSearch1.ClientID %>').focus();
                }
                return true;
            }
    
            function CheckEnter2_Onclick(e)
            {
                var key;
                key=(e.which) ? e.which : e.keyCode;
                if(key==13)
                {
                    document.getElementById('<%=btnSearch2.ClientID %>').focus();
                }
                return true;
            }
    
        </script>
    </telerik:RadCodeBlock>
    <table width="100%" cellspacing="0" border="0" cellpadding="0">
        <tr class="h1title" valign="middle">
            <td colspan="2">
                <table border="0" cellpadding="0">
                    <tr>
                        <td colspan="2">
                            <span class="indnt Phead">Merge Patients</span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" cellspacing="0" style="border-top: solid 1px #b5c4c4; border-bottom: solid 1px #b5c4c4; border-right: solid 1px #b5c4c4; border-left: solid 1px #b5c4c4">
                    <tr class="h4title">
                        <td style="height: 14px" colspan="3">
                            <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" CausesValidation="false" OnClick="btnBack_OnClick"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="message">
                            <asp:Panel ID="ucMessagePanel" runat="server">
                                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" Icon="ERROR" MessageText="Please enter at least 2 valid characters for one of the search criteria." />            
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3"><p>Use this page to merge patients. This process will merge both patients' medications, allergies, and problems. The end result of the merge will be a single patient with combined history. The demographic information of Patient 2 will remain.</p></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:CustomValidator ID="cvMergePatients" runat="server" OnServerValidate="cvMergePatients_ServerValidate" Enabled="false"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 45%">
                            <table width="100%" cellspacing="5px">
                                <tr>
                                    <td><span class="title">Step 1: Search for a patient to merge</span></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table cellpadding="1">
                                            <tr>
                                                <td align="right">Last name:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat1LastName" runat="server" onkeydown="return CheckEnter1_Onclick(event)"></asp:TextBox>   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">First name:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat1FirstName" runat="server" onkeydown="return CheckEnter1_Onclick(event)"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">Patient ID:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat1PatientID" runat="server" onkeydown="return CheckEnter1_Onclick(event)"></asp:TextBox>
                                                    <asp:Button ID="btnSearch1" runat="server" CssClass="btnstyle" Text="Search" OnClick="btnSearch1_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <input id="hiddenPat1Restriction" runat="server" type="text" style="display:none" />
                                        <input id="hiddenPat2Restriction" runat="server" type="text" style="display:none" />

                                        <telerik:RadGrid ID="grdViewPat1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" Width="100%"
                                            GridLines="None" PageSize="25" EnableViewState="true" AllowMultiRowSelection="false"
                                            Skin="Allscripts" EnableEmbeddedSkins="false" OnItemDataBound="grdViewPat1_ItemDataBound">
                                            <MasterTableView DataKeyNames="PatientID, IsVIPPatient,IsRestrictedPatient">
                                                <PagerStyle Mode="NextPrevAndNumeric" />
                                                <NoRecordsTemplate>No patients found</NoRecordsTemplate>
                                                <Columns>
                                                    <telerik:GridTemplateColumn>
                                                        <HeaderStyle HorizontalAlign="left" />
                                                        <ItemStyle HorizontalAlign="center" Width="30px" />
                                                        <ItemTemplate>
                                                            <input id="rdSelectRow1" name="rdSelectRow1" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="MRN" HeaderText="Patient ID" SortExpression="MRN">
                                                        <HeaderStyle HorizontalAlign="left" Width="85px" Font-Bold="true" />
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridTemplateColumn HeaderText="Patient" SortExpression="Name">
                                                        <ItemTemplate>
                                                            <asp:Image ID="confidentialIcon" ImageUrl="~\images\PrivacyImages\sensitivehealth-global-16-x-16.png" runat="server" Visible="false" />
                                                            <asp:Label ID="lblPatientName" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Name")) %>' Font-Bold="true"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblPatientInfo" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Sex")) + ", " + ObjectExtension.ToEvalEncode(Eval("DOB")) + ", " + ObjectExtension.ToEvalEncode(Eval("Phone")) + ", " + ObjectExtension.ToEvalEncode(Eval("Address")) %>' ForeColor="Gray"></asp:Label>
                                                            <br />
                                                            <span style="color:Gray">Updated:</span> <asp:Label ID="lblLastChange" runat="server" ForeColor="Gray"></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="left" Font-Bold="true" />
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                            <ClientSettings>
                                                <Selecting AllowRowSelect="True" />
                                            </ClientSettings>
                                        </telerik:RadGrid>
                                        <asp:ObjectDataSource ID="Pat1ObjDataSource" runat="server"
                                            OldValuesParameterFormatString="original_{0}" TypeName="Allscripts.Impact.CHPatient"
                                            SelectMethod="SearchPatient">
                                            <SelectParameters>
                                                <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                                <asp:ControlParameter Name="LastName" Type="String" ControlID="txtSearchPat1LastName" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:ControlParameter Name="FirstName" Type="String" ControlID="txtSearchPat1FirstName" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:ControlParameter Name="ChartID" Type="String" ControlID="txtSearchPat1PatientID" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:Parameter Name="WildCard" Type="String" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                                <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                                                <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                                <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="true"/>
                                                <asp:ControlParameter ControlID="chkInactive" Name="includeInactive" PropertyName="checked" Type="boolean" />
                                                <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center" style="width: 10%; height: 300px">
                            <br /><br />
                            <asp:CheckBox ID="chkInactive" runat="server" Text="Include inactive" />
                            <br /><br /><br /><br /><br />
                            <asp:Button ID="btnMerge1" CssClass="btnstyle" runat="server" Text="Merge Into >" OnClick="btnMerge1_Click" />
                        </td>
                        <td style="width: 45%">
                            <table width="100%" cellspacing="5px">
                                <tr>
                                    <td><span class="title">Step 2: Search for a patient to merge into</span></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table cellpadding="1">
                                            <tr>
                                                <td align="right">Last name:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat2LastName" runat="server" onkeydown="return CheckEnter2_Onclick(event)"></asp:TextBox>   
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">First name:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat2FirstName" runat="server" onkeydown="return CheckEnter2_Onclick(event)"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right">Patient ID:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSearchPat2PatientID" runat="server" onkeydown="return CheckEnter2_Onclick(event)"></asp:TextBox>
                                                    <asp:Button ID="btnSearch2" runat="server" CssClass="btnstyle" Text="Search" OnClick="btnSearch2_Click" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <telerik:RadGrid ID="grdViewPat2" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" Width="100%"
                                            GridLines="None" PageSize="25" EnableViewState="true" AllowMultiRowSelection="false"
                                            Skin="Allscripts" EnableEmbeddedSkins="false" OnItemDataBound="grdViewPat2_ItemDataBound">
                                            <MasterTableView DataKeyNames="PatientID,IsVIPPatient,IsRestrictedPatient">
                                                <PagerStyle Mode="NextPrevAndNumeric" />
                                                <NoRecordsTemplate>No patients found</NoRecordsTemplate>
                                                <Columns>
                                                    <telerik:GridTemplateColumn>
                                                        <HeaderStyle HorizontalAlign="left" />
                                                        <ItemStyle HorizontalAlign="center" Width="30px" />
                                                        <ItemTemplate>
                                                            <input id="rdSelectRow2" name="rdSelectRow2" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>' />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="MRN" HeaderText="Patient ID" SortExpression="MRN">
                                                        <HeaderStyle HorizontalAlign="left" Width="85px" Font-Bold="true" />
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridTemplateColumn HeaderText="Patient" SortExpression="Name">
                                                        <ItemTemplate>
                                                            <asp:Image ID="confidentialIcon1" ImageUrl="~\images\PrivacyImages\sensitivehealth-global-16-x-16.png" runat="server" Visible="false" />
                                                            <asp:Label ID="lblPatientName" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Name")) %>' Font-Bold="true"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblPatientInfo" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Sex")) + ", " + ObjectExtension.ToEvalEncode(Eval("DOB")) + ", " + ObjectExtension.ToEvalEncode(Eval("Phone")) + ", " + ObjectExtension.ToEvalEncode(Eval("Address")) %>' ForeColor="Gray"></asp:Label>
                                                            <br />
                                                            <span style="color:Gray">Updated:</span> <asp:Label ID="lblLastChange" runat="server" ForeColor="Gray"></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle HorizontalAlign="left" Font-Bold="true" />
                                                    </telerik:GridTemplateColumn>                                                
                                                </Columns>
                                            </MasterTableView>
                                            <ClientSettings>
                                                <Selecting AllowRowSelect="True" />
                                            </ClientSettings>
                                        </telerik:RadGrid>
                                        <asp:ObjectDataSource ID="Pat2ObjDataSource" runat="server"
                                            OldValuesParameterFormatString="original_{0}" TypeName="Allscripts.Impact.CHPatient"
                                            SelectMethod="SearchPatient">
                                            <SelectParameters>
                                                <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                                <asp:ControlParameter Name="LastName" Type="String" ControlID="txtSearchPat2LastName" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:ControlParameter Name="FirstName" Type="String" ControlID="txtSearchPat2FirstName" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:ControlParameter Name="ChartID" Type="String" ControlID="txtSearchPat2PatientID" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:Parameter Name="WildCard" Type="String" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                                <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                                                <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                                <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="true"/>
                                                <asp:ControlParameter ControlID="chkInactive" Name="includeInactive" PropertyName="checked" Type="boolean" />
                                                <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>  
    <asp:Panel style="DISPLAY: none" ID="pnlConfirmation" runat="server" CssClass="overlaymainwide">
        <div class="overlayTitle">
            Merge Patients Confirmation
        </div>
        <div class="overlayContent">
        <table style="border-collapse:collapse;">
            <tr>
                <td>
                    <table width="330px">
                        <tr><td colspan="2"><span style="text-decoration:underline">Patient 1</span></td></tr>
                        <tr>
                            <td><b>Name:</b></td>
                            <td><asp:Label ID="lblPat1Name" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>ID:</b></td>
                            <td><asp:Label ID="lblPat1MRN" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Gender:</b></td>
                            <td><asp:Label ID="lblPat1Gender" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>DOB:</b></td>
                            <td><asp:Label ID="lblPat1DOB" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>ZIP:</b></td>
                            <td><asp:Label ID="lblPat1ZIP" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Medications:</b></td>
                            <td><asp:Label ID="lblPat1Meds" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Allergies:</b></td>
                            <td><asp:Label ID="lblPat1Allergies" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Problems:</b></td>
                            <td><asp:Label ID="lblPat1Problems" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                    </table>
                    <br />
                     
                </td>
                <td>&nbsp</td>
                <td>
                    <table width="330px">
                        <tr><td colspan="2"><span style="text-decoration:underline">Patient 2</span></td></tr>
                        <tr>
                            <td><b>Name:</b></td>
                            <td><asp:Label ID="lblPat2Name" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>ID:</b></td>
                            <td><asp:Label ID="lblPat2MRN" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Gender:</b></td>
                            <td><asp:Label ID="lblPat2Gender" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>DOB:</b></td>
                            <td><asp:Label ID="lblPat2DOB" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>ZIP:</b></td>
                            <td><asp:Label ID="lblPat2ZIP" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Medications:</b></td>
                            <td><asp:Label ID="lblPat2Meds" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Allergies:</b></td>
                            <td><asp:Label ID="lblPat2Allergies" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                        <tr>
                            <td><b>Problems:</b></td>
                            <td><asp:Label ID="lblPat2Problems" runat="server" Text="None entered"></asp:Label></td>
                        </tr>
                    </table>
                </td>                    
            </tr>
            <tr>
                <td colspan="3">
                    <span style="color:Red">
                        <asp:CheckBox ID="chkConfirm" runat="server" Text="I agree that upon clicking Yes, all medications, allergies, and problems associated with these two patients will be merged. The demographic information of Patient 2 will remain, and you will no longer see Patient 1 in your patient search results. Once the patient information is merged, I agree to review the active patient for accuracy." /></span>
                        <asp:CustomValidator ClientValidationFunction="ValidateChecked" ID="cvConfirmChecked" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="3"><p>Are you sure you want to merge these patients?</p></td>
            </tr>
            <tr>
                <td colspan="3" align="center">
                    <asp:HiddenField ID="hiddenPat1ID" runat="server" Visible="false" /><asp:HiddenField ID="hiddenPat2ID" runat="server" Visible="false" />
                </td>
            </tr>
        </table>
            </div>
        <div class="overlayFooter">
             <asp:Button ID="btnYes" CssClass="btnstyle btnStyleAction" runat="server" OnClick="btnYes_Click" Text="Yes" Width="150px"></asp:Button>  
                    <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" OnClientClick="cancel()" OnClick="btnCancel_Click" Text="Cancel" Width="150px" CausesValidation="false"></asp:Button>
        </div>
    </asp:Panel>
    <asp:Button ID="btnHiddenTrigger" runat="server" style="display:none;" />
    <ajaxToolkit:ModalPopupExtender ID="modalConfirmationPopup" runat="server"
        TargetControlID="btnHiddenTrigger"
        PopupControlID="pnlConfirmation"
        DropShadow="false" 
        BackgroundCssClass="modalBackground"
        CancelControlID="btnCancel" />  
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSearch1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID ="ucMessagePanel" />
                    <telerik:AjaxUpdatedControl ControlID="grdViewPat1"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSearch2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID ="ucMessagePanel" />
                    <telerik:AjaxUpdatedControl ControlID="grdViewPat2"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
             <telerik:AjaxSetting AjaxControlID="grdViewPat1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewPat1"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
                         <telerik:AjaxSetting AjaxControlID="grdViewPat2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewPat2"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Button ID="btnHiddenCancel" runat="server" style="display:none;" OnClick="btnHiddenCancel_Click" CausesValidation ="false" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
<%--    <!--start help panel-->
    <asp:Panel ID="panelHelpHeader" class="accordionHeader" runat="server" Width="95%">
        <table cellspacing="0" cellpadding="0" width="95%" border="0">
            <tbody>
                <tr>
                    <td align="left" width="140">
                        <div id="Header" class="accordionHeaderText">
                            Help With This Screen</div>
                    </td>
                    <td align="right" width="14">
                        <asp:Image ID="hlpclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;</td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <asp:Panel ID="panelHelp" class="accordionContent" runat="server" Width="92%">
    </asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeHelp" runat="server" TargetControlID="panelHelp"
        Collapsed="true" CollapsedSize="0" ExpandControlID="panelHelpHeader" CollapseControlID="panelHelpHeader"
        ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png"
        ImageControlID="hlpclpsimg" SuppressPostBack="true">
    </ajaxToolkit:CollapsiblePanelExtender>
    <!--end help panel-->--%>
</asp:Content>