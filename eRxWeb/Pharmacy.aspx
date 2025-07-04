<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Pharmacy"
    ViewStateEncryptionMode="Never" EnableViewStateMac="false" Title="Choose Pharmacy"
    MasterPageFile="~/PhysicianMasterPage.master" Codebehind="Pharmacy.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="~/Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed"
    TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
            function onRowClick(row,transMethod)
            {              
                var grdViewPharmacy = document.getElementById('<%=grdViewPharmacy.ClientID%>');

                for (var rowIndex = 0; rowIndex < grdViewPharmacy.rows.length; rowIndex++)
                {
                       var currentRow = grdViewPharmacy.rows[rowIndex];              

                      if (currentRow != null && row.id != currentRow.id) {
                            currentRow.className = '';
                            $(currentRow).find('input:radio').attr('checked', false);
                            }
                }

                cleanWhitespace(row);

                $(row).find('input:radio').attr('checked', true);
                //angular function calling for rightpanel
                var pharmid =   $(row).find('input:radio').attr('value');
                if(pharmid!= undefined && pharmid.length>0)
                    selectPharmacy(pharmid)
                row.className = 'SelectedRow ' + row.className;
                
                var imgSendScript = document.getElementById("ctl00_ContentPlaceHolder1_btnSendScript");
                var canSendRx = <%= ((Session["SPI"] != null) ? "true" : "false") %>;
    
                if(imgSendScript != null && canSendRx)
                {
                    imgSendScript.disabled = false;
        
                    if (transMethod == "SURESCRIPTS")
	                    imgSendScript.value = 'Send Script';
                }

                var btnSubmitRefillRequest = document.getElementById("ctl00_ContentPlaceHolder1_btnSubmitRefillRequest");
                if(btnSubmitRefillRequest != null)
                {
                    btnSubmitRefillRequest.disabled = false;
                }

                var btnSetPatientPharm = document.getElementById("ctl00_ContentPlaceHolder1_btnSetPatientPharm");
                if(btnSetPatientPharm != null)
                {
                    btnSetPatientPharm.disabled = false;
                }
                
                var btnAddPracticeFavorite = document.getElementById("ctl00_ContentPlaceHolder1_btnAddPracticeFavorite");
                if(btnAddPracticeFavorite != null)
                {
                    btnAddPracticeFavorite.disabled = false;
                }
                
                var btnAddAsSitePharmacy = document.getElementById("ctl00_ContentPlaceHolder1_btnAddAsSitePharmacy");
                if(btnAddPracticeFavorite != null)
                {
                    btnAddAsSitePharmacy.disabled = false;
                }
                
                var btnContinue = document.getElementById('<%=btnContinue.ClientID%>');
                if (btnContinue != null)
                {
                    btnContinue.disabled = false;
                }

                var txt = document.getElementById('<%=selectedPharmacy.ClientID%>');
	            if (txt != null)
	            {   
	                txt.value = $(row).find('input:radio').attr('value');
	            }
	            else
	            {
		            txt = document.getElementById('<%=selectedPharmacy.ClientID%>');
		            if (txt != null)
		            {
			            txt.value = $(row).find('input:radio').attr('value');
		            }
	            }
	
	            var btn = document.getElementById("ctl00$ContentPlaceHolder2$btnUpdate");
                if (btn != null)
                {
	                btn.click();
                }
                else
                {
	                btn = document.getElementById("ctl00_ContentPlaceHolder2_btnUpdate");
	                if (btn != null)
	                {
		                btn.click();
	                }
                }
            }

            function cleanWhitespace(node)
            {
                for (var x = 0; x < node.childNodes.length; x++) 
                {
                    var childNode = node.childNodes[x]
                    if ((childNode.nodeType == 3)&&(!/\S/.test(childNode.nodeValue))) 
                    {
                        // that is, if it's a whitespace text node
                        node.removeChild(node.childNodes[x])
                        x--
                    }
                    if (childNode.nodeType == 1) 
                    {
                        // elements can have text child nodes of their own
                        cleanWhitespace(childNode)
                    }
                }
            }

            function autoTab(obj,evt,len,next_field)
            {
                var phone_field_length
   
                if (evt == 'down') {
                    phone_field_length=obj.value.length;
                }
                else if (evt == 'up') 
                {
                    if (obj.value.length != phone_field_length) 
                    {
                        phone_field_length=obj.value.length;
                        if (phone_field_length == len) 
                        {
                            var nextElement = document.getElementById(next_field);
                            
                            if(nextElement != null) nextElement.focus();
                        }
                    }
                }
             }
 
            function clearSearchCriteria()
            {
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtNameSearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtAddressSearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtCitySearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_ddlStateSearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '--';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtZipSearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtPhoneAreaCodeSearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }
    
                var txtNameSearch = document.getElementById("ctl00_ContentPlaceHolder1_txtPhoneBodySearch");
                if (txtNameSearch != null)
                {
                    txtNameSearch.value = '';
                }    
            }
        </script>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
            function EndRequestHandler(sender, args) {
                if (args.get_error() != undefined) {
                    if (args.get_error().message.substring(0, 51) ==
               "Sys.WebForms.PageRequestManagerParserErrorException") {
                        window.location.reload();
                    }
                    args.set_errorHandled(true);
                }
            }
        </script>
    </telerik:RadCodeBlock>
    <table width="100%" cellspacing="0">
        <tr>
            <td colspan="2">
                <table class="h4title" width="100%" cellspacing="0">
                    <tr>
                        <td class="Pheadblack indnt" valign="baseline" colspan="2">
                            Pharmacy Search:&nbsp&nbsp&nbsp&nbsp&nbsp<span class="h4titletext" style="font-style: italic">Not
                                all fields are required.</span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                            <asp:ValidationSummary ID="ValidationSummary2" runat="server" BackColor="White" BorderColor="Red"
                                BorderStyle="Solid" BorderWidth="1px" ShowSummary="true" ValidationGroup="PharmacySearch" />
                        </td>
                    </tr>
                    <tr>
                        <td class="h4titletext" style="padding-left: 25px; width: 92px; vertical-align: middle"
                            align="right">
                            <b>Search by:</b>
                        </td>
                        <td style="vertical-align: middle">
                            <table>
                                <tr>
                                    <td style="vertical-align: middle; width: 289px;">
                                        <asp:Panel ID="pnlSearchBy" runat="server" GroupingText="Retail Pharmacies" CssClass="h4titletext"
                                            Width="290px">
                                            <asp:RadioButtonList ID="rblSearchBy" runat="server" RepeatDirection="horizontal"
                                                AutoPostBack="true" OnSelectedIndexChanged="rblSearchBy_SelectedIndexChanged">
                                                <asp:ListItem Selected="true" Text="Patient History" Value="PatHis"></asp:ListItem>
                                                <asp:ListItem Text="Practice Favorites" Value="PracFav"></asp:ListItem>
                                                <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </asp:Panel>
                                    </td>
                                    <td style="vertical-align: middle; padding-top: 7px">
                                        <asp:RadioButton ID="rblMailOrder" runat="server" Text="Mail Order Pharmacies" AutoPostBack="true"
                                            OnCheckedChanged="rblMailOrder_CheckedChanged" GroupName="MailOrder" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="h4titletext" style="padding-left: 25px; width: 92px; vertical-align: middle;
                            height: 26px;" align="right">
                            <b>
                                <asp:Label ID="lblName" runat="server" Text="Name:" Font-Bold="True" ForeColor="Black"
                                    CssClass="h4titletext"></asp:Label></b>
                        </td>
                        <td style="height: 26px">
                            <asp:TextBox MaxLength="70" ID="txtNameSearch" runat="server" Width="102px" Height="17px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="minCharRegularExpressionValidator" runat="server"
                                ErrorMessage="Please enter at least 2 valid characters for pharmacy name." ControlToValidate="txtNameSearch"
                                ValidationExpression="[0-9a-zA-Z\s-,&.']{2,}" Display="None" ValidationGroup="PharmacySearch"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="h4titletext" style="padding-left: 25px; width: 92px; vertical-align: middle;
                            height: 41px;" align="right" valign="bottom">
                            <b>
                                <asp:Label ID="lblStreet" runat="server" CssClass="h4titletext" Font-Bold="True"
                                    Text="Street Address: " Height="31px" Width="91px"></asp:Label></b>
                        </td>
                        <td style="height: 41px">
                            <asp:TextBox ID="txtAddressSearch" runat="server" MaxLength="40" Width="103px" Height="16px"></asp:TextBox>&nbsp
                            &nbsp
                            <asp:Label ID="lblCity" runat="server" CssClass="h4titletext" Font-Bold="True" Text=" City:"></asp:Label>
                            &nbsp;
                            <asp:TextBox ID="txtCitySearch" runat="server" MaxLength="35" Width="75px"></asp:TextBox>&nbsp
                            <asp:Label ID="lblState" runat="server" CssClass="h4titletext" Font-Bold="True" Text="State:"></asp:Label>
                            <asp:Label ID="lblStateAsterisk" runat="server" ForeColor="red">*</asp:Label>
                            <asp:DropDownList ID="ddlStateSearch" runat="server" Width="45px">
                            </asp:DropDownList>
                            &nbsp &nbsp
                            <asp:Label ID="lblZipCode" runat="server" CssClass="h4titletext" Font-Bold="True"
                                Font-Strikeout="False" Text="ZIP Code:"></asp:Label>
                            <asp:TextBox ID="txtZipSearch" runat="server" CssClass="searchTextBox" Width="37px"
                                MaxLength="5"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="txtZipSearch"
                                ErrorMessage="Please enter a valid 5-digit ZIP code." ValidationGroup="PharmacySearch" ValidationExpression="^\d{5}$"
                                Display="none" ></asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="cvStateZipSearch" runat="server" ValidationGroup="PharmacySearch" ErrorMessage="Please either select a state or enter a 5-digit ZIP code."
                                Display="none" OnServerValidate="cvStateZipSearch_ServerValidate"></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="h4titletext" style="padding-left: 25px; width: 92px; vertical-align: middle"
                            align="right">
                            <b>
                                <asp:Label ID="lblPhone" runat="server" CssClass="h4titletext" Font-Bold="True" Text="Phone:"></asp:Label></b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPhoneAreaCodeSearch" runat="server" MaxLength="3" onkeyup="autoTab(this,'up','3','ctl00$ContentPlaceHolder1$txtPhoneBodySearch');"
                                Width="30px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revPhoneAreaCodeSearch" runat="server" ControlToValidate="txtPhoneAreaCodeSearch"
                                ErrorMessage="Please enter a valid 3-digit area code." ValidationGroup="PharmacySearch" ValidationExpression="^\d{3}$"
                                Display="none"></asp:RegularExpressionValidator>
                            &nbsp -
                            <asp:TextBox ID="txtPhoneBodySearch" runat="server" MaxLength="8" Width="90px"></asp:TextBox>
                            &nbsp &nbsp &nbsp
                            <asp:Button ID="btnGo" CssClass="btnstyle" runat="server" Text="Search" OnClick="btnGo_Click" 
                                ToolTip="Click here to search for a pharmacy." Width="125px" ValidationGroup="PharmacySearch"/>
                            <asp:RegularExpressionValidator ID="revPhoneBody" runat="server" ControlToValidate="txtPhoneBodySearch"
                                ErrorMessage="Please enter a valid 7-digit phone number in the format XXX-XXXX." ValidationGroup="PharmacySearch"
                                ValidationExpression="^(?!\d[1]{2}|[5]{3})([2-9]\d{2})([. -]*)\d{4}$" Display="none"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr style="background-color: #fffacd">
            <td colspan="2" style="padding-left: 5px" valign="bottom">
                <table width="100%">
                    <tr>
                        <td style="width: 8px">
                            <img src="images/information.png" alt="" />
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblInfo" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" border="1" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td colspan="5">
                                        <asp:Panel ID="pnlPharmacyButtons" runat="server">
                                            <asp:Button ID="btnNewRx" runat="server" CssClass="btnstyle" OnClick="btnNewRx_Click"
                                                Text="New Rx" ToolTip="Click here to create a new script." Visible="False" />
                                            <asp:Button ID="btnSubmitRefillRequest" Enabled="false" runat="server" CssClass="btnstyle"
                                                OnClick="SubmitRefillRequest_Click" Text="Renew Prescription" />
                                            <asp:Button ID="btnPrint" runat="server" CssClass="btnstyle" OnClick="btnPrint_Click"
                                                Text="Print" CausesValidation="false" ToolTip="Click here to print the script locally (on your printer)." />
                                            <asp:Button ID="btnSendScript" runat="server" CssClass="btnstyle" Enabled="False"
                                                OnClick="btnSendScrip_Click" Text="Send Script ►" ToolTip="Click here to send the script to the chosen pharmacy." />
                                            <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" Visible="false" Text="Back"
                                                CausesValidation="false" OnClick="btnCancel_Click" />
                                            <asp:Button ID="btnCancelRx" runat="server" CssClass="btnstyle" OnClick="btnCancelRx_Click"
                                                CausesValidation="false" Text="Back" ToolTip="Click here to cancel." />
                                            <asp:Button ID="btnContinue" runat="server" CssClass="btnstyle" OnClick="btnContinue_Click"
                                                Enabled="false" CausesValidation="false" Text="Continue" Visible="False" />
                                            <asp:Button ID="btnSetPatientPharm" runat="server" CssClass="btnstyle" Enabled="false"
                                                CausesValidation="false" Text="Set as Patient Pharmacy" Visible="False" OnClick="btnSetPatientPharm_Click" />
                                            <asp:Button ID="btnAddPracticeFavorite" runat="server" CssClass="btnstyle" CausesValidation="false"
                                                Text="Add to Practice Favorites" Visible="False" OnClick="btnAddPracticeFavorite_Click"
                                                Enabled="False" />
                                            <asp:Button ID="btnAddAsSitePharmacy" runat="server" CssClass="btnstyle" CausesValidation="false"
                                                Style="display: none;" Text="Add as Site Pharmacy" Visible="False" OnClick="btnAddAsSitePharmacy_Click"
                                                Enabled="False" />
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="h4title">
                        <td>
                            <div runat="server" id="divSendNotiication">
                                <asp:CheckBox ID="cboSendToADM" runat="server" Visible="true" Text="Send Notification" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div runat="server" id="divRefillsend">
                                <asp:CheckBox ID="chkRefillSend" runat="server" Visible="false" Text="Would you like your refills to be sent to Advance Paradigm's Mail Order Service?" />
                            </div>
                        </td>
                    </tr>
                    <tr id="pharmacyGridTableRow" runat="server">
                        <td>
                            <div id="divgrdviewPharmacy" runat="server" >
                                <asp:GridView ID="grdViewPharmacy" runat="server" AllowPaging="True" AllowSorting="True"
                                    AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
                                    EmptyDataText="No Pharmacies Found" OnRowDataBound="grdViewPharmacy_RowDataBound"
                                    DataKeyNames="PharmacyID,NABP,TransMethod,EpcsEnabled" OnSelectedIndexChanged="grdViewPharmacy_SelectedIndexChanged"
                                    PageSize="50" OnRowCreated="grdViewPharmacy_RowCreated" OnLoad="grdViewPharmacy_Load">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rbSelectedRow" runat="server" Width="20px" />
                                            </ItemTemplate>
                                            <ItemStyle Width="20px" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:ImageField DataImageUrlField="MOP_SRC" Visible="False" DataAlternateTextField="MOP_Text"
                                            ReadOnly="True">
                                        </asp:ImageField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Image ID="imgEpcs" runat="server" Style="position: relative; top: 3px; left: 5px;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Name" HeaderText="Destination" SortExpression="Name" HeaderStyle-HorizontalAlign="Left" />
                                        <asp:TemplateField HeaderText="Address">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblAddress"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="City" HeaderText="City"  HeaderStyle-HorizontalAlign="Left"/>
                                        <asp:BoundField DataField="State" HeaderText="State"  HeaderStyle-HorizontalAlign="Left"/>
                                        <asp:TemplateField HeaderText="Phone">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblPhone"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <asp:ObjectDataSource ID="PatientPharmacyObjDataSource" runat="server" SelectMethod="GetPharmaciesUsed"
                                TypeName="Allscripts.Impact.Patient" OldValuesParameterFormatString="original_{0}">
                                <SelectParameters>
                                    <asp:SessionParameter Name="PatientID" SessionField="PatientID" />
                                    <asp:SessionParameter Name="LicenseID" SessionField="LicenseID" />
                                    <asp:SessionParameter Name="UserID" SessionField="UserID" />
                                    <asp:ControlParameter ControlID="txtNameSearch" ConvertEmptyStringToNull="True" Name="name"
                                        PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtAddressSearch" Name="address" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtCitySearch" Name="city" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ddlStateSearch" Name="state" PropertyName="Text"
                                        ConvertEmptyStringToNull="True" />
                                    <asp:ControlParameter ControlID="txtZipSearch" Name="zip" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneAreaCodeSearch" Name="phoneareacode" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneBodySearch" Name="phonenumber" PropertyName="Text" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="PharmacyFavoriteObjDataSource" runat="server" SelectMethod="SearchFavoritePharmacy"
                                TypeName="Allscripts.Impact.Pharmacy" OldValuesParameterFormatString="original_{0}">
                                <SelectParameters>
                                    <asp:SessionParameter SessionField="LicenseID" Name="licenseID" />
                                    <asp:SessionParameter SessionField="SiteID" Name="siteID" />
                                    <asp:ControlParameter ControlID="txtNameSearch" ConvertEmptyStringToNull="True" Name="name"
                                        PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtAddressSearch" Name="address" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtCitySearch" Name="city" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ddlStateSearch" Name="state" PropertyName="Text"
                                        ConvertEmptyStringToNull="True" />
                                    <asp:ControlParameter ControlID="txtZipSearch" Name="zip" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneAreaCodeSearch" Name="phoneareacode" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneBodySearch" Name="phonenumber" PropertyName="Text" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="PharmacyObjDataSource" runat="server" SelectMethod="Search"
                                TypeName="Allscripts.Impact.Pharmacy" OldValuesParameterFormatString="original_{0}"
                                OnSelected="PharmacyObjDataSource_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNameSearch" ConvertEmptyStringToNull="True" Name="name"
                                        PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtAddressSearch" Name="address" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtCitySearch" Name="city" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ddlStateSearch" Name="state" PropertyName="Text"
                                        ConvertEmptyStringToNull="True" />
                                    <asp:ControlParameter ControlID="txtZipSearch" Name="zip" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneAreaCodeSearch" Name="phoneareacode" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneBodySearch" Name="phonenumber" PropertyName="Text" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="MailOrderPharmacyObjDataSource" runat="server" SelectMethod="SearchMailOrderPharmacy"
                                TypeName="Allscripts.Impact.Pharmacy" OldValuesParameterFormatString="original_{0}"
                                EnableCaching="true" CacheDuration="Infinite" CacheExpirationPolicy="Sliding">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNameSearch" Name="name" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtAddressSearch" Name="address" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtCitySearch" Name="city" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ddlStateSearch" Name="state" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtZipSearch" Name="zip" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneAreaCodeSearch" Name="phoneareacode" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="txtPhoneBodySearch" Name="phonenumber" PropertyName="Text" />
                                    <asp:SessionParameter Name="PatientID" SessionField="PatientID" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="false" />
    <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server"
        HideContactMe="true" HideContactMeText="true" YesMessage="Click <b>YES</b> below to print the prescription." />
    <asp:Panel Style="display: none" ID="pnlAddToFavorites" runat="server">
        <table class="overlayBasic" style="border-collapse: collapse; "
            cellpadding="5" cellspacing="20" width="450">
            <tr>
                <td>
                    This pharmacy is not yet part of this site's pharmacy favorite list. Would you like
                    to add it?
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnAddToFavorites" CssClass="btnstyle" CausesValidation="false" runat="server" Text="Yes, add this to my site favorites"
                        OnClick="btnAddToFavorites_Click" />&nbsp<asp:Button ID="btnDoNotAddToFavorites"
                            CssClass="btnstyle" runat="server" CausesValidation="false" Text="No thanks, continue" OnClick="btnDoNotAddToFavorites_Click" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="modalAddToFavoritesPopup" runat="server" TargetControlID="btnHiddenTrigger"
        PopupControlID="pnlAddToFavorites" BackgroundCssClass="modalBackground" DropShadow="false" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline" EnableAJAX="false">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rblSearchBy">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rblSearchBy" />
                    <telerik:AjaxUpdatedControl ControlID="rblMailOrder" />
                    <telerik:AjaxUpdatedControl ControlID="lblName" />
                    <telerik:AjaxUpdatedControl ControlID="txtNameSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblStreet" />
                    <telerik:AjaxUpdatedControl ControlID="txtAddressSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblCity" />
                    <telerik:AjaxUpdatedControl ControlID="txtCitySearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblState" />
                    <telerik:AjaxUpdatedControl ControlID="ddlStateSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblStateAsterisk" />
                    <telerik:AjaxUpdatedControl ControlID="cvStateZipSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblZipCode" />
                    <telerik:AjaxUpdatedControl ControlID="txtZipSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblPhone" />
                    <telerik:AjaxUpdatedControl ControlID="txtPhoneAreaCodeSearch" />
                    <telerik:AjaxUpdatedControl ControlID="txtPhoneBodySearch" />
                    <telerik:AjaxUpdatedControl ControlID="btnGo" />
                    <telerik:AjaxUpdatedControl ControlID="lblInfo" />
                    <%--<telerik:AjaxUpdatedControl ControlID="pnlPharmacyButtons" />--%>
                    <telerik:AjaxUpdatedControl ControlID="divgrdviewPharmacy"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rblMailOrder">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rblSearchBy" />
                    <telerik:AjaxUpdatedControl ControlID="rblMailOrder" />
                    <telerik:AjaxUpdatedControl ControlID="lblName" />
                    <telerik:AjaxUpdatedControl ControlID="txtNameSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblStreet" />
                    <telerik:AjaxUpdatedControl ControlID="txtAddressSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblCity" />
                    <telerik:AjaxUpdatedControl ControlID="txtCitySearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblState" />
                    <telerik:AjaxUpdatedControl ControlID="ddlStateSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblStateAsterisk" />
                    <telerik:AjaxUpdatedControl ControlID="cvStateZipSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblZipCode" />
                    <telerik:AjaxUpdatedControl ControlID="txtZipSearch" />
                    <telerik:AjaxUpdatedControl ControlID="lblPhone" />
                    <telerik:AjaxUpdatedControl ControlID="txtPhoneAreaCodeSearch" />
                    <telerik:AjaxUpdatedControl ControlID="txtPhoneBodySearch" />
                    <telerik:AjaxUpdatedControl ControlID="btnGo" />
                    <telerik:AjaxUpdatedControl ControlID="lblInfo" />
                    <%--<telerik:AjaxUpdatedControl ControlID="pnlPharmacyButtons" />--%>
                    <telerik:AjaxUpdatedControl ControlID="divgrdviewPharmacy"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnGo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="divgrdviewPharmacy"/>
                    <telerik:AjaxUpdatedControl ControlID="lblInfo" />
                    <telerik:AjaxUpdatedControl ControlID="ValidationSummary2" />
                    <telerik:AjaxUpdatedControl ControlID="rblSearchBy" />
                    <telerik:AjaxUpdatedControl ControlID="ddlStateSearch" />
                </UpdatedControls>
            </telerik:AjaxSetting>          
        </AjaxSettings>
    </telerik:RadAjaxManager>
    

    <asp:HiddenField ID="selectedPharmacy" runat="server"></asp:HiddenField>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">

          
</asp:Content>
