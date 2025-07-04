<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.ApproveScriptMessagePatient" Title=" ePrescribe Script Message" Codebehind="ApproveScriptMessagePatient.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ObsoletePARClassMappingChange.ascx" TagName="ObsoletePARClassMappingChange"
    TagPrefix="ePrescribe" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <script type="text/javascript">
         //EAK added for LimitTextbox
         //
         // Limit the text input in the specified field.
         //
         function LimitInput(targetField, maxLen) {
             var isPermittedKeystroke;
             var enteredKeystroke;
             var maximumFieldLength;
             var currentFieldLength;
             var inputAllowed = true;
             var selectionLength = parseInt(document.selection.createRange().text.length);

             if (maxLen != null) {
                 // Get the current and maximum field length
                 currentFieldLength = parseInt(targetField.value.length);
                 maximumFieldLength = parseInt(maxLen);

                 // Allow non-printing, arrow and delete keys
                 enteredKeystroke = window.event.keyCode;
                 isPermittedKeystroke = ((enteredKeystroke < 32)                                // Non printing
                     || (enteredKeystroke >= 33 && enteredKeystroke <= 40)    // Page Up, Down, Home, End, Arrow
                     || (enteredKeystroke == 46))                            // Delete

                 // Decide whether the keystroke is allowed to proceed
                 if (!isPermittedKeystroke) {
                     if ((currentFieldLength - selectionLength) >= maximumFieldLength) {
                         inputAllowed = false;
                     }
                 }
             }

             window.event.returnValue = inputAllowed;
             return (inputAllowed);
         }

         //
         // Limit the text input in the specified field.
         //

         function LimitPaste(targetField, maxLen) {
             var clipboardText;
             var resultantLength;
             var maximumFieldLength;
             var currentFieldLength;
             var pasteAllowed = true;
             var selectionLength = parseInt(document.selection.createRange().text.length);

             if (maxLen != null) {
                 // Get the current and maximum field length
                 currentFieldLength = parseInt(targetField.value.length);
                 maximumFieldLength = parseInt(maxLen);

                 clipboardText = window.clipboardData.getData("Text");
                 resultantLength = currentFieldLength + clipboardText.length - selectionLength;
                 if (resultantLength > maximumFieldLength) {
                     pasteAllowed = false;
                 }
             }

             window.event.returnValue = pasteAllowed;
             return (pasteAllowed);
         }

         function enableControl(who, how) {
             who = ObjectExtension.ToEvalEncode(eval('document.forms[0].' + who));
             if (who != null) {
                 if (how == true) {
                     who.disabled = false;
                 }
                 else {
                     who.disabled = true;
                 }
             }
         }

         var prevRow;
         var savedClass;

         function onRowClickPat(row) {
             if (prevRow == null || prevRow != row) {
                 if (prevRow != null) {
                     prevRow.className = savedClass;
                 }
                 savedClass = row.className;
                 row.className = 'SelectedRow';
                 prevRow = row;

                 cleanWhitespace(row);
                 row.firstChild.childNodes[0].checked = true;

                 //var txt = document.getElementById("ctl00$ContentPlaceHolder1$selectedPat");
                 var txt = document.getElementById("<%=selectedPat.ClientID%>");

        if (txt != null) {
            txt.value = row.firstChild.childNodes[0].value;
        }
        else {
            //txt = document.getElementById("ctl00_ContentPlaceHolder1_selectedPat");
            txt = document.getElementById("<%=selectedPat.ClientID%>");
            if (txt != null) {
                txt.value = row.childNodes[0].childNodes[0].value;
            }
        }

        //var btn = document.getElementById("ctl00$ContentPlaceHolder1$btnUpdate");
        var btn = document.getElementById("<%=btnUpdate.ClientID%>");
        if (btn != null) {
            btn.click();
        }
        else {
            //btn = document.getElementById("ctl00_ContentPlaceHolder1_btnUpdate");
            btn = document.getElementById("<%=btnUpdate.ClientID%>");
                     if (btn != null) {
                         btn.click();
                     }
                 }

             }
         }

         function checkPat(source, args) {
             //var txt = document.getElementById("ctl00$ContentPlaceHolder1$selectedPat");
             var txt = document.getElementById("<%=selectedPat.ClientID%>");
             if (txt != null) {
                 if (txt.value != null && txt.value != "") {
                     args.IsValid = true;
                 }
                 else {
                     args.IsValid = false;
                 }
             }
         }


         var timerID;
         function startwait() {
             if (goodToGo()) {
                 timerid = setTimeout("resubmit()", 1500);
             }
             else {
                 $find('mpePatient').show();
             }
         }

         function resubmit() {

             var btn = document.getElementById("<%=btnSearch.ClientID%>");
    if (btn != null) {
        btn.click();
    }
    else {
        btn = document.getElementById("<%=btnSearch.ClientID%>");
                 if (btn != null) {
                     btn.click();
                 }
             }
             clearTimeout(timerID);
         }

         function goodToGo() {

             var validator = document.getElementById('<%=revPhone.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=rfvFirstName.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revFname.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=rfvLastName.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revLastName.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revMName.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revDOB.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=rfvDOB.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=cvDOB.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }

    validator = document.getElementById('<%=revCity.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revZipCode.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=rfvZipCode.ClientID %>');
    if (!validator.isvalid) {
        return false;
    }
    validator = document.getElementById('<%=revEmail.ClientID %>');
             if (!validator.isvalid) {
                 return false;
             }

             enableControl("ctl00$ContentPlaceHolder1$btnAdd", false);
             return true;
         }

         function setSelectedPat(patid) {
             var form = document.forms[0];
             var row;
             if (form.rdSelectRow) {
                 if (form.rdSelectRow.length) {
                     for (var index = 0; index < form.rdSelectRow.length; index++) {
                         if (form.rdSelectRow[index].value == patid) {
                             row = form.rdSelectRow[index].parentNode.parentNode;
                             break;
                         }
                     }
                 }
                 else {
                     if (form.rdSelectRow.value == patid)
                         row = form.rdSelectRow.parentNode.parentNode;
                 }
             }
             if (row) {
                 onRowClickPat(row);
             }

         }

         function cleanWhitespace(node) {
             for (var x = 0; x < node.childNodes.length; x++) {
                 var childNode = node.childNodes[x]
                 if ((childNode.nodeType == 3) && (!/\S/.test(childNode.nodeValue))) {
                     // that is, if it's a whitespace text node
                     node.removeChild(node.childNodes[x])
                     x--
                 }
                 if (childNode.nodeType == 1) {
                     // elements can have text child nodes of their own
                     cleanWhitespace(childNode)
                 }
             }
         }

         function ShowPrivacyOverriddenOverlay(id) {
             if (window.parent.PatientSelected != undefined) {
                 window.parent.PatientSelected(id);
             }
         }

         function closeBackDrop() {
             if (window.parent.CloseOverlay != undefined) {
                 window.parent.CloseOverlay();
             }
         }
    </script>
<script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <table border="0" cellpadding="2" cellspacing="0" width="100%">
        <tr>
            <td colspan="2" class="h1title">
                <ePrescribe:Message ID="ucMessageHeader" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table align="center" class="normal" border="1" bordercolor="#b4b4b4" cellpadding="0"
                    cellspacing="0" width="100%">
                    <tr>
                        <td colspan="2">
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h4title">
                                    <td colspan="6">
                                        <asp:Button ID="btnChangePatient" runat="server" CssClass="btnstyle" OnClick="btnChangePatient_Click"
                                            Text="Back" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnNext" runat="server" CssClass="btnstyle" ValidationGroup="maingroup" OnClick="btnNext_Click" Text="Next" />
                                            </td>
                                </tr>
                            </table>
                            <asp:Label ID="lblSched" runat="server" />
                        </td>
                    </tr>
                    <tr height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                        <td style="margin-left: 10px">
                        
                            <table id="tblPharmacyHeaderInfo" runat="server" Visible="true" border="0" cellpadding="0" cellspacing="0" width="800" style="padding-top: 6px;">
                                <tr>
                                    <td align="left">
                                        <b>Pharmacy:</b>&nbsp;<asp:Label runat="server" ID="lblPharmacyName" /></td>
                                     <td align="left">
                                        <b>Address:</b>&nbsp;<asp:Label runat="server" ID="lblPharmacyAddress" /></td>
                                    <td align="right">
                                        <b>Phone:</b>&nbsp;<asp:Label runat="server" ID="lblPharmacyPhone" /></td>
                                </tr>
                            </table>
                                                        
                            <table id="tblReferralHeaderInfo" runat="server" Visible="false" border="0" cellpadding="0" cellspacing="0" width="800" style="padding-top: 6px;">
                                <tr>
                                    <td align="left">
                                        <b>Sent By:</b>&nbsp;<asp:Label runat="server" ID="lblReferralSenderName" /></td>
                                    <td align="right">
                                        <b>Practice:</b>&nbsp;<asp:Label runat="server" ID="lblReferralSenderPractice" /></td>
                                </tr>
                            </table>
                            
                            <hr />
                            
                            <table border="0" cellpadding="0" cellspacing="0" width=800>
                                <tr>
                                    <td width=300>
                                        <b>Patient info in request:</b></td>
                                    <td>
                                        <b>Lookup patient in ePrescribe</b></td>
                                </tr>
                                <tr>
                                    <td >
                                        <table border=0 cellpadding=0 cellspacing=0 width=100% >
                                            <tr><td><b>Name:</b></td><td width="170" align="left">
                                            <asp:Label runat="server" ID="lblPatientName" /></td></tr>
                                            <tr id="trGender" runat="server" visible="false">
												<td><b>Gender:</b></td>
												<td align="left"><asp:Label ID="lblPatientGender" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <b>DOB:</b></td>
                                                <td align="left">
                                                    <asp:Label runat="server" ID="lblDOB" /></td>
                                            </tr>
                                            <tr valign="top">
                                                <td>
                                                    <b>Address:</b></td>
                                                <td align="left">
                                                    <asp:Label runat="server" ID="lblAddress" /></td>
                                            </tr>
                                        </table>                                        
                                    </td>
                                    
                                    <td>
                                     <asp:UpdatePanel runat="server" ID="upSearch" UpdateMode="Conditional" ChildrenAsTriggers="True">
                                            <contenttemplate>
                                            <asp:ObjectDataSource ID="PatObjDataSource" runat="server"
                                                OldValuesParameterFormatString="original_{0}" TypeName="Allscripts.Impact.CHPatient"
                                                SelectMethod="PatientSearch_Reconcile" OnSelected="PatObjDataSource_Selected">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                                    <asp:ControlParameter Name="LastName" Type="String" ControlID="txtLastNameSearch" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                    <asp:ControlParameter Name="FirstName" Type="String" ControlID="txtFirstNameSearch" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                    <asp:ControlParameter Name="DateOfBirth" Type="String" ControlID="lblDOB" DefaultValue="" ConvertEmptyStringToNull="True"/>
                                                    <asp:Parameter Name="ChartID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                    <asp:Parameter Name="WildCard" Type="String" DefaultValue="" ConvertEmptyStringToNull="False"/>
                                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                                    <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                                                    <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                                    <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="true"/>
                                                    <asp:Parameter Name="includeInactive" Type="boolean" DefaultValue="false"/>
                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        <ePrescribe:Message ID="ucSearchMessage" runat="server" Visible="false" />
                                        <table width="100%">
                                            <tr valign="top">
                                                <td valign="top">
                                                    <b>Last:</b> <asp:TextBox id="txtLastNameSearch" runat="server"></asp:TextBox>
							                        <b>First:</b> <asp:TextBox id="txtFirstNameSearch" runat="server"></asp:TextBox>
							                        <asp:Button id="btnSearch" runat="server" CssClass="btnstyle" Text="Search"></asp:Button>
							                    </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:GridView id="grdViewPatients" EnableViewState="false" runat="server" Width="600" Height="200px" DataSourceID="PatObjDataSource"
							                            AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CaptionAlign="Left" GridLines="None" EmptyDataText=" No patients match"
							                            OnRowDataBound="grdViewPatients_RowDataBound" DataKeyNames="PatientID" PageSize="5" AutoPostBack="true">
                                                        <Columns>
                                                            <asp:TemplateField>
                                                            <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
                                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                            <ItemTemplate>
                                                            <input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>'/>												
                                                            </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Name" SortExpression="Name" HeaderText="Patient Name">
                                                            <ItemStyle></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="DOB" HeaderText="DOB">
                                                            <ItemStyle width=70px ></ItemStyle>
                                                            </asp:BoundField>                                                          
                                                            <asp:BoundField DataField="Phone" HeaderText="Phone Number">
                                                            <ItemStyle></ItemStyle>
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Address" HeaderText="Street Address">
                                                            <ItemStyle></ItemStyle>
                                                            </asp:BoundField>
                                                        </Columns>
                                                    </asp:GridView> 

                                                </td>
                                            </tr>
                                        </table>
							                                                    
                                        </contenttemplate>
									            																	
                                        </asp:UpdatePanel>
                                        
                                                    <asp:LinkButton id="btnAdd" runat="server" Text="Click to add patient"></asp:LinkButton> 
                                                    <asp:Panel style="DISPLAY: none;" id="panelAdd" runat="server">
                                                        <div class="overlaymain">
                                                        <div class="overlayTitle">
                                                            Add Patient
                                                        </div>
                                                        <div class="overlayContent">
                                                        <table id="TABLE1" style="border-collapse:collapse; ">
                                                            <tr>
                                                                <td align="right">Phone:</td>
                                                                <td>
                                                                    <asp:TextBox onblur="formatPhoneInput(this);ValidatorOnChange(event);" id="txtPhone" onfocus="parseNumberInput(this)" onkeypress="return numericKeyPressOnly(this, event);" runat="server" CssClass="txtboxcolor" Width="100px" MaxLength="14"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revPhone" runat="server" Width="1px" ControlToValidate="txtPhone" ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$" ErrorMessage="Please enter a valid 10-digit phone number (###)###-#### or ###-###-####" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                    <asp:Label id="lblPhoneInfo" runat="server" CssClass="LoginFailed" Text=""></asp:Label>
                                                                </td>
                                                                <td align="right" style="white-space:nowrap">Mobile Phone:</td>
                                                                <td colspan="3">
                                                                    <asp:TextBox onblur="formatPhoneInput(this);ValidatorOnChange(event);" id="txtMobilePhone" onfocus="parseNumberInput(this)" onkeypress="return numericKeyPressOnly(this, event);" runat="server" CssClass="txtboxcolor" Width="100px" MaxLength="14"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revMobilePhone" runat="server" Width="1px" ControlToValidate="txtMobilePhone" ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$" ErrorMessage="Please enter a valid 10-digit phone number (###)###-#### or ###-###-####" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                    <asp:Label id="lblMobilePhoneInfo" runat="server" CssClass="LoginFailed" Text=""></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width:110px" align="right">First Name:</td>
                                                                <td>
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtFName" runat="server" CssClass="txtboxcolor" Width="126px" MaxLength="35"></asp:TextBox>
                                                                    <asp:RequiredFieldValidator id="rfvFirstName" runat="server" Width="1px" Height="5px" ControlToValidate="txtFName" Display="dynamic" ErrorMessage="Please enter a first name" ValidationGroup="addpat">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator id="revFname" runat="server" Width="1px" ControlToValidate="txtFName" Display="dynamic" ValidationExpression="^([a-zA-Z]+[\s-'.]{0,35})*" ErrorMessage="Please enter a valid first name" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                </td>
                                                                <td style="width:110px" align="right">Last Name:</td>
                                                                <td>
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtLName" runat="server" CssClass="txtboxcolor" Width="122px" MaxLength="60"></asp:TextBox></td><td>
                                                                    <asp:RequiredFieldValidator id="rfvLastName" runat="server" Width="6px" Height="9px" Display="dynamic" ControlToValidate="txtLName" ErrorMessage="Please enter a last name" ValidationGroup="addpat">*</asp:RequiredFieldValidator>
                                                                    <asp:RegularExpressionValidator id="revLastName" runat="server" ControlToValidate="txtLName" Display="dynamic" ValidationExpression="^([a-zA-Z]+[\s-'.]{0,35})*" ErrorMessage="Please enter a valid last name" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                </td>
                                                               <%-- <td align="right">MI: </td>--%><%--<asp:Label runat="server" Text="MI:"></asp:Label>--%>
                                                                <td>
                                                                    <asp:Label runat="server" Text="MI:"></asp:Label>
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtMName" runat="server" Width="25px" MaxLength="1"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revMName" runat="server" Width="1px" ControlToValidate="txtMName" Display="dynamic" ValidationExpression="^[a-zA-Z]{1}$" ErrorMessage="Please enter a letter for middle initial" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <asp:Panel ID="pnlPaternalMaternal" runat="server">
                                                                <tr>
                                                                    <td style="width:100px" align="right">Paternal:</td>
                                                                    <td>
                                                                        <asp:TextBox onblur="capitilizeInitial(this)" id="txtPaternalName" runat="server" CssClass="txtboxcolor" Width="126px" MaxLength="20"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator id="rfvPaternalName" runat="server" Width="1px" Height="5px" ControlToValidate="txtPaternalName" Display="dynamic" ErrorMessage="Please enter a paternal name" ValidationGroup="addpat">*</asp:RequiredFieldValidator>
                                                                        <asp:RegularExpressionValidator id="revPaternalName" runat="server" Width="1px" ControlToValidate="txtPaternalName" Display="dynamic" ValidationExpression="^([a-zA-Z]+[\s-'.]{0,20})*" ErrorMessage="Please enter a valid paternal name" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                    </td>
                                                                    <td style="width:110px" align="right">Maternal:</td>
                                                                    <td>
                                                                        <asp:TextBox onblur="capitilizeInitial(this)" id="txtMaternalName" runat="server" CssClass="txtboxcolor" Width="122px" MaxLength="15"></asp:TextBox>
                                                                        <asp:RegularExpressionValidator id="revMaternalName" runat="server" ControlToValidate="txtMaternalName" Display="dynamic" ValidationExpression="^([a-zA-Z]+[\s-'.]{0,15})*" ErrorMessage="Please enter a valid maternal name" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                    </td>
                                                                </tr>
                                                            </asp:Panel>
                                                            <tr>
                                                                <td align="right">DOB:</td>
                                                                <td>
                                                                    <asp:TextBox id="txtDOB" runat="server" CssClass="txtboxcolor" Width="128px" MaxLength="10"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revDOB" runat="server" Width="1px" ControlToValidate="txtDOB" Display="dynamic" ValidationExpression="((^(10|12|0?[13578])([/])(3[01]|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(11|0?[469])([/])(30|[12][0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(2[0-8]|1[0-9]|0?[1-9])([/])((1[8-9]\d{2})|([2-9]\d{3}))$)|(^(0?2)([/])(29)([/])([2468][048]00)$)|(^(0?2)([/])(29)([/])([3579][26]00)$)|(^(0?2)([/])(29)([/])([1][89][0][48])$)|(^(0?2)([/])(29)([/])([2-9][0-9][0][48])$)|(^(0?2)([/])(29)([/])([1][89][2468][048])$)|(^(0?2)([/])(29)([/])([2-9][0-9][2468][048])$)|(^(0?2)([/])(29)([/])([1][89][13579][26])$)|(^(0?2)([/])(29)([/])([2-9][0-9][13579][26])$))" ErrorMessage="Please enter a valid DOB (MM/DD/YYYY)" ValidationGroup="addpat" ToolTip="Please enter a valid DOB (MM/DD/YYYY)">*</asp:RegularExpressionValidator>
                                                                    <asp:RequiredFieldValidator id="rfvDOB" runat="server" Width="1px" ControlToValidate="txtDOB" Display="dynamic" ErrorMessage="Please enter a date of birth" ValidationGroup="addpat">*</asp:RequiredFieldValidator>
                                                                    <asp:CompareValidator id="cvDOB" runat="server" Width="1px" ControlToValidate="txtDOB" Display="dynamic" ErrorMessage="Date Of Birth can not be in the future" ValidationGroup="addpat" Type="Date" Operator="LessThanEqual">*</asp:CompareValidator>
                                                                    <%--<span class="h2">(mm/dd/yyyy)</span>--%>
                                                                </td>                                                            
                                                                <td align="right">Gender: </td>
                                                                <td>
                                                                    <asp:DropDownList id="DDLGender" runat="server" Width="98px">
                                                                        <asp:ListItem Value="M">Male</asp:ListItem>
                                                                        <asp:ListItem Value="F">Female</asp:ListItem>
                                                                        <asp:ListItem Value="U">Unknown</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td align="left"  style="height:3px"><span class="h2">(mm/dd/yyyy)</span></td>


                                                            </tr>
                                                            <tr>
                                                                <td align="right">MRN: </td>
                                                                <td colspan="5">
                                                                    <asp:TextBox id="txtMRN" runat="server" Width="126px" MaxLength="25"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">Address 1: </td>
                                                                <td colspan="5">
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtAddress1" runat="server" Width="300px" MaxLength="55"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">Address 2:</td>
                                                                <td colspan="5">
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtAddress2" runat="server" Width="300px" MaxLength="55"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">City:</td>
                                                                <td>
                                                                    <asp:TextBox onblur="capitilizeInitial(this)" id="txtCity" runat="server" Width="128px" MaxLength="20"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revCity" runat="server" ControlToValidate="txtCity" Display="dynamic" ValidationExpression="^([a-zA-Z]+[\s-'.]{0,20})*" ErrorMessage="Please enter a valid city">*</asp:RegularExpressionValidator>
                                                                </td>
                                                                <td align="right">State:</td>
                                                                <td>
                                                                    <asp:DropDownList id="ddlState" runat="server" Width="50px"></asp:DropDownList>
                                                                </td>
                                                                <td align="right">ZIP: </td>
                                                                <td>
                                                                    <asp:TextBox id="txtZip" onkeypress="return numericKeyPressOnly(this, event);" runat="server" CssClass="txtboxcolor" Width="97px" MaxLength="5"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revZipCode" runat="server" ControlToValidate="txtZip" ValidationExpression="^\d{5}$" ErrorMessage="Please enter a valid 5-digit ZIP Code" ValidationGroup="addpat" Display="dynamic">*</asp:RegularExpressionValidator>
                                                                  </td>
                                                                  <td>
                                                                   <asp:RequiredFieldValidator id="rfvZipCode" runat="server" Width="1px" ControlToValidate="txtZip" ErrorMessage="Please enter a 5-digit ZIP Code" ValidationGroup="addpat" Display="dynamic" >*</asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">Email:</td>
                                                                <td colspan="5">
                                                                    <asp:TextBox id="txtEmail" runat="server" Width="300px" MaxLength="100"></asp:TextBox>
                                                                    <asp:RegularExpressionValidator id="revEmail" runat="server" Width="1px" Height="1px" ControlToValidate="txtEmail" ValidationExpression="^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-.\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,3})$" ErrorMessage="Please enter a valid email address" ValidationGroup="addpat">*</asp:RegularExpressionValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="6">
                                                                    <asp:ValidationSummary id="ValidationSummary1" runat="server" ValidationGroup="addpat" Forecolor="Orange"></asp:ValidationSummary>
                                                                </td>
                                                            </tr>
                                                      </table>
                                                            </div>
                                                        <div class="overlayFooter">
                                                            <asp:Button CausesValidation="true" id="btnSaveAdd" CssClass="btnstyle btnStyleAction" OnClientClick="return closeBackDrop();" onclick="btnSaveAdd_Click"  runat="server" Text="Add Patient" UseSubmitBehavior="true" ValidationGroup="addpat"></asp:Button>
                                                                    <asp:Button id="btnCancelAdd" runat="server" Text="Cancel" CssClass="btnstyle"></asp:Button>
                                                        </div>
                                                            </div>
                                                 </asp:Panel>
                                                 <ajaxToolkit:ModalPopupExtender
                                                    id="mpePatient"
                                                    runat="server"
                                                    BehaviorID="mpePatient"
                                                    DropShadow="false"
                                                    BackgroundCssClass="modalBackground"
                                                    CancelControlID="btnCancelAdd"
                                                    TargetControlID="btnAdd"
                                                    OnOkScript="goodToGo()"
                                                    PopupControlID="panelAdd">
                                                 </ajaxToolkit:ModalPopupExtender>
                                                 <asp:ObjectDataSource id="PatDataDataSource" runat="server" TypeName="Allscripts.Impact.CHPatient" SelectMethod="LoadPatient" OldValuesParameterFormatString="original_{0}">
										            <SelectParameters>											        
											            <asp:ControlParameter ControlID="selectedPat" ConvertEmptyStringToNull="False"
												            Name="PatientID" PropertyName="Value" Type="String" />
                                                        <asp:SessionParameter Name="userID" SessionField="USERID" />
                                                        <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" />
                                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />		        
										            </SelectParameters>
									            </asp:ObjectDataSource> <asp:DataGrid id="grdAlt" runat="server" AutoGenerateColumns="False"></asp:DataGrid>
									            
									            <asp:HiddenField id="selectedPat" runat="server"></asp:HiddenField> 
									            <asp:TextBox id="phpat" Style="display:none;width:1px;height:1px;" runat="server" ValidationGroup="maingroup"></asp:TextBox> 
									            <asp:CustomValidator id="valSelectedPat" runat="server" ControlToValidate="phpat" Text="*" ErrorMessage="You must select a patient or create a new one." ValidationGroup="maingroup" ClientValidationFunction="checkPat" ValidateEmptyText="True"></asp:CustomValidator> 
                                           <asp:UpdatePanel runat="server" ID="upData" UpdateMode="Conditional" ChildrenAsTriggers="False">
                                            <contenttemplate>
                                            <asp:Panel ID="pnlAllergyMedication" runat="server">
                                            <asp:Label id="lblCurrAllergies" runat="server"></asp:Label><asp:Label id="lblCurrMeds" runat="server"></asp:Label> 
                                            </asp:Panel>                                      
                                           </contenttemplate>
                                            <triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnUpdate" EventName="Click"></asp:AsyncPostBackTrigger>
                                            </triggers>                                                                                                
                                        </asp:UpdatePanel>
                                  
                                        <asp:Button ID=btnUpdate  CssClass=content2backcolor BorderWidth=0 BorderStyle=None Width=1 Height=1 UseSubmitBehavior=false  Text="Update" runat=server />
                                        
                                        <!--<asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />-->
                                    </td>
                                </tr>
                                
                            </table>                                                                                 
                            <asp:ValidationSummary ID="valsum"   runat="server" ValidationGroup="maingroup" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <ePrescribe:ObsoletePARClassMappingChange ID="ucObsoletePARClassMappingChange" runat="server"
        CurrentPage="ApproveScriptMessagePatient.aspx" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
<%--    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <contenttemplate>        
         <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex=0 runat="server" ContentCssClass="accordionContent" HeaderCssClass="accordionHeader">
                        <Panes>
                            <ajaxToolkit:AccordionPane ID=paneHelp runat="server">
                            <Header>Help With This Screen</Header>
                            <Content>
                                <asp:Panel ID=HelpPanel runat=server></asp:Panel>
                               </Content>
                            </ajaxToolkit:AccordionPane>                            
                        </Panes>
                    </ajaxToolkit:Accordion>
        </contenttemplate>
    </asp:UpdatePanel>    --%>
</asp:Content>
