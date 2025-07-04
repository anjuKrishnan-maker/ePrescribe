<%@ Page Language="C#" EnableViewState="true" AutoEventWireup="True" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.AddPatient" Title="Add Patient" Codebehind="AddPatient.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Joel.Net.Refresh" Namespace="Joel.Net" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <cc1:Refresh ID="Refresh1" runat="server" />
    <script type="text/javascript" language="javascript">

        function CalculateLbsAndOzsFromKgs()
        {
            var kgValue = $('#<%= txtWeightKg.ClientID %>').val(); 

            if (kgValue == "")
                kgValue = 0;

            var ozs = Math.round(((kgValue * 2.20462) % 1) * 16);
            var lbs = Math.floor(kgValue * 2.20462);
            if(ozs === 16)
            {
                ozs = 0;
                lbs = lbs+1;
            }
            $('#<%= txtWeightLbs.ClientID %>').val(lbs);
            $('#<%= txtWeightOzs.ClientID %>').val(ozs);
        }

        function UpdateKgFromLbsAndOzs()
        {
            var lbs = document.getElementById( '<%= txtWeightLbs.ClientID %>').value;
            var ozs = document.getElementById('<%= txtWeightOzs.ClientID %>').value;

            if (lbs == "")
                lbs = 0;
            if (ozs == "")
                ozs = 0;

            var kgs = lbs * .45359237;
            var kgs = kgs + (ozs / 16) * .45359237;
            $('#<%= txtWeightKg.ClientID %>').val(kgs.toFixed(2));
            
        }

        function UpdateCmFromFtAndIns()
        {
            var feet = document.getElementById('<%= txtHeightFt.ClientID %>').value;
            var inches = document.getElementById('<%= txtHeightIn.ClientID %>').value;
            if (feet == "")
                feet = 0;
            if (inches == "")
                inches = 0;
            var cm = (feet * 30.48) + (inches * 2.54);
            $('#<%= txtHeightCm.ClientID %>').val(cm.toFixed(1));
        }

        function CalculateFtAndInsFromCm()
        {
            var cm = document.getElementById('<%= txtHeightCm.ClientID %>').value;

            if (cm == "") {
                $('#<%= txtHeightFt.ClientID %>').val(0);
                $('#<%= txtHeightIn.ClientID %>').val(0);
            }
            //Begin conversions
            var cmValue = $('#<%= txtHeightCm.ClientID %>').val();
            if (cmValue !== "") {
                cmValue /= 2.54;
                var feet = parseInt(Math.floor(cmValue / 12));
                var inches = parseInt(Math.round(cmValue % 12));
                if(inches === 12)
                {
                    inches = 0;
                    feet++;
                }
                $('#<%= txtHeightFt.ClientID %>').val(feet);
                $('#<%= txtHeightIn.ClientID %>').val(inches);
            }
        }
       
        function digitValidation(evt) 
        {
            if (evt.charCode > 31 && (evt.charCode < 48 || evt.charCode > 57))
            {
                return false;
            }
            if (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
            {
                return false;
            }
        }
       
        function digitDecimalValidation(evt) {
            if (evt.charCode == 46)
                return true;
            if (evt.keyCode == 46)
                return true;

            if (evt.charCode > 31 && (evt.charCode < 48 || evt.charCode > 57)) {
                return false;
            }
            if (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
            {
                return false;
            }
        }

	</script>
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
     <asp:HiddenField ID="isFromAngularModal" runat="server" Value="false" ClientIDMode="Static"/>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr class="h1title indnt">
                    <td colspan="2">
                        &nbsp;<span id="heading" runat="server" class="highlight">Add New Patient </span>
                    </td>
                </tr>               
                <tr class="h4title">
					<td colspan="2" style="padding:5px">
					    <asp:Button ID="btnBack" runat="server" CausesValidation="False" CssClass="btnstyle" Text="Back" visible="true" ToolTip="Click to go back to the Choose Patient page." OnClick="btnBack_Click" />
					        <asp:Button ID="btnEditSave" runat="server" CssClass="btnstyle" Text="Save" OnClick="btnEditSave_Click" ToolTip="Click to save details." />
					        <asp:Button ID="btnPatientAdd" runat="server" CssClass="btnstyle" OnClick="btnPatientAdd_Click" Text="Save" ToolTip="Click to save details." />
					        <asp:Button ID="btnSaveAndPrescribe" runat="server" CssClass="btnstyle" OnClick="btnSaveAndPrescribe_Click" Text="Save & Prescribe" ToolTip="Click to save details." Visible="False" />
                            <asp:Button ID="btnAddAllergy" runat="server" CausesValidation="True" CssClass="btnstyle" OnClick="btnAddAllergy_Click" Text="Patient Allergy" ToolTip="Click to view list of medications to which patient is allergic or to add a new allergy." />
					        <asp:Button ID="btnAmendments" Text="Amendments" CssClass="btnstyle" OnClick="btnAmendments_OnClick" runat="server"/>
					</td>
				</tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" border="0" cellpadding="0" style="width:100%">
                            <tr>
                                <td style="height: inherit;">
                                    <table width="100%" border="0" cellpadding="0">
										<tr>
											<td>
												<ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
										    </td>
										</tr>
                                        <tr>
                                            <td>
                                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table width="100%" border="0" cellspacing="5px" cellpadding="0">
													<tr>
														<td align="right" style="width:10%">
															Phone:</td>
														<td style="width:30%">
															<asp:TextBox ID="txtPhone"  runat="server" MaxLength="14" onkeypress="return numericKeyPressOnly(this, event);" onfocus="parseNumberInput(this)" onblur="formatPhoneInput(this);ValidatorOnChange(event);"></asp:TextBox>
															<asp:RegularExpressionValidator
																ID="revPhone" runat="server" ErrorMessage="Please enter a valid 10 digit Phone No (###)###-#### or ###-###-####"
																ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$"
																Width="1px" ControlToValidate="txtPhone" Display="dynamic">*</asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="right" style="width:15%">
                                                            Mobile Phone:
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtMobilePhone" runat="server" MaxLength="14" onkeypress="return numericKeyPressOnly(this, event);" onfocus="parseNumberInput(this)" onblur="formatPhoneInput(this);ValidatorOnChange(event);"></asp:TextBox>
                                                            <asp:RegularExpressionValidator
																ID="revMobilePhone" runat="server" ErrorMessage="Please enter a valid 10 digit Mobile Phone No (###)###-#### or ###-###-####"
																ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$"
																Width="1px" ControlToValidate="txtMobilePhone" Display="dynamic">*</asp:RegularExpressionValidator>
                                                        </td>
													</tr>														
                                                    <tr>
                                                        <td align="right">
                                                            <span style="color:Red">*</span> First Name:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtFName" runat="server" MaxLength="35" Width="200px" onblur="capitilizeInitial(this)">
                                                            </asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFName"
                                                                ErrorMessage="Please enter a first name." Height="5px" Width="1px">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtFName"
                                                                ErrorMessage="Please enter a valid first name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*"
                                                                Width="1px">*</asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="right">
															Middle Name:
                                                        </td>
                                                        <td>
															<asp:TextBox ID="txtMName" runat="server" MaxLength="35" onblur="capitilizeInitial(this)"></asp:TextBox>
															<asp:RegularExpressionValidator ID="revMName" runat="server" ErrorMessage="Please enter a valid middle name." ValidationExpression="^[a-zA-Z]{0,35}$"
																Width="1px" ControlToValidate="txtMName">*</asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <asp:Panel ID="pnlMaternalPaternal" runat="server">
                                                        <tr>
                                                            <td nowrap="nowrap" align="right">
                                                                <span style="color:Red">*</span> Paternal Last Name:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtPaternalName" runat="server" MaxLength="20" Width="200px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                                                <asp:RequiredFieldValidator ID="rfvPaternalName" runat="server" ControlToValidate="txtPaternalName" Display="dynamic"
                                                                    ErrorMessage="Please enter a paternal name.">*</asp:RequiredFieldValidator>
                                                                <asp:RegularExpressionValidator ID="revPaternalName" runat="server" ControlToValidate="txtPaternalName" Display="dynamic"
                                                                    ErrorMessage="Please enter a valid paternal name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*">*</asp:RegularExpressionValidator>
                                                            </td>
                                                            <td nowrap="nowrap" align="right">
                                                                Maternal Last Name:
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMaternalName" runat="server" MaxLength="15" Width="200px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="revMaternalName" runat="server" ControlToValidate="txtMaternalName" Display="dynamic"
                                                                    ErrorMessage="Please enter a valid maternal name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*">*</asp:RegularExpressionValidator>
                                                            </td>
                                                        </tr>
                                                    </asp:Panel>
                                                    <tr>
                                                        <td nowrap="nowrap" align="right">
                                                            <asp:Label ID="lblLastNameAsterisk" runat="server" ForeColor="red">*</asp:Label> Last Name:
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtLName" runat="server" MaxLength="35" Width="200px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLName"
                                                                ErrorMessage="Please enter a last name." Height="9px" Width="6px">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="revLastName" runat="server" ControlToValidate="txtLName"
                                                                ErrorMessage="Please enter a valid last name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*">*</asp:RegularExpressionValidator>
                                                        </td>
                                                        <td align="right">
															Patient allows Medication History:
                                                        </td>
                                                        <td>
															<asp:DropDownList ID="ddlMedHistory" runat="server" Width="80px">
                                                               <asp:ListItem Value="false">Yes</asp:ListItem>
                                                               <asp:ListItem Value="true">No</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr><td></td> <td></td>
                                                        <td align="right">
															Patient allows Disclosures to Health Plan:
                                                        </td>
                                                        <td>
															<asp:DropDownList ID="ddlPlanDisclosure" runat="server" Width="80px">
                                                               <asp:ListItem Value="true">Yes</asp:ListItem>
                                                               <asp:ListItem Value="false">No</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <span style="color:Red">*</span> Date of Birth:</td>
                                                        <td>
                                                            <telerik:RadDateInput ID="txtDOB" runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy" Skin="Allscripts" MinDate="01/01/1900"
                                                                Width="100px" ToolTip="Enter Patient's Date of Birth (mm/dd/yyyy)" EnableEmbeddedSkins="false" EmptyMessage="mm/dd/yyyy" SelectionOnFocus="None"  >
                                                            </telerik:RadDateInput>
                                                            <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ControlToValidate="txtDOB"
                                                                ErrorMessage="Please enter a valid date of birth." Width="1px">*</asp:RequiredFieldValidator>
                                                           
                                                        </td>  
                                                        <td align="right">
                                                            Patient Preferred&nbsp;<br />
                                                            Language:&nbsp;</td>
                                                        <td colspan="3" style="padding-top:3px">
                                                            <asp:DropDownList ID="ddlPatientPreferredLang" runat="server" Width="100px"></asp:DropDownList>
                                                        </td>                                                      
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <span style="color:Red">*</span> Gender:
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLGender" runat="server" Width="80px">
                                                                <asp:ListItem Value="none">--Select--</asp:ListItem>                                                                
                                                                <asp:ListItem Value="M">Male</asp:ListItem>
                                                                <asp:ListItem Value="F">Female</asp:ListItem>
                                                                <asp:ListItem Value="U">Unknown</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:RegularExpressionValidator ID= "RegExpValGender" runat="server" ControlToValidate="DDLGender" ErrorMessage=" Please select Gender" ValidationExpression="[A-Z]{1}">*</asp:RegularExpressionValidator>

                                                        </td>
                                                        <td align="right">
                                                            <div style="padding-top: 10px">
                                                                Current Weight:&nbsp;                                                             
                                                            </div>
                                                        </td>
                                                        <td style="padding-top:3px;">
                                                            <asp:TextBox runat="server" ID="txtWeightLbs" Width="40px"  MaxLength="4" onfocusout="UpdateKgFromLbsAndOzs()" onkeypress="return digitValidation(event)" onkeyup ="UpdateKgFromLbsAndOzs()"></asp:TextBox>&nbsp;lb
                                                        &nbsp;<asp:TextBox runat="server" ID="txtWeightOzs" Width="20px" MaxLength="2" onfocusout="UpdateKgFromLbsAndOzs()" onkeypress="return digitValidation(event)" onkeyup ="UpdateKgFromLbsAndOzs()"></asp:TextBox>&nbsp;oz
                                                   </td> 
                                                    </tr>
                                                    <tr style="padding-top:20px">
                                                        <td align="right">
                                                            Patient ID (MRN):
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtMRN" runat="server" MaxLength="25"></asp:TextBox>
                                                        </td>   
                                                        <td>
                                                           
                                                        </td>   
                                                        <td style="padding-top:3px;">
                                                            <asp:TextBox runat="server" ID="txtWeightKg" Width="60px" MaxLength="6" onfocusout="CalculateLbsAndOzsFromKgs()" onkeypress="return digitDecimalValidation(event)" onkeyup="CalculateLbsAndOzsFromKgs()" ></asp:TextBox>&nbsp;kg
                                                            <asp:RegularExpressionValidator ID="kgDecimalValidation" runat="server" ErrorMessage="Invalid weight" ValidationExpression="^(([0-9]{0,3}\.[0-9]{0,2})|^(\d{0,3}))$" ControlToValidate="txtWeightKg">*</asp:RegularExpressionValidator> 
                                                    </tr>
                                                    <tr>
                                                    <asp:Panel ID="pnlPatientStatus" runat="server">
                                                            <td align="right">
                                                                Patient Status:
                                                            </td>
                                                            <td align="left" colspan="2">
                                                                <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Text="Active" Selected="True" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                                <span style="color:Gray; font-size:smaller; font-style:italic">Inactive patients will not be included in searches, reports and will not be checked for duplicity.</span>
                                                            </td> 
                                                          
                                                      </asp:Panel> 
                                                        </tr>
                                                    <tr>
                                                        <td align="right">
                                                           <span style="color:Red">*</span> Address 1:
                                                        </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddress1" runat="server" MaxLength="40" Width="300px"  onblur="capitilizeInitial(this)"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1"
                                                                ErrorMessage="Please enter Address." Width="1px">*</asp:RequiredFieldValidator>
                                                        </td>
                                                        <td align="right">
                                                            <div style="padding-top: 10px">
                                                                Current Height:&nbsp;                                                             
                                                            </div>
                                                            </td>
                                                            <td style="padding-top:3px;" rowspan="2">
                                                                <asp:TextBox runat="server" ID="txtHeightFt" Width="40px" MaxLength="2" onfocusout="UpdateCmFromFtAndIns()" onkeypress="return digitValidation(event)" onkeyup="UpdateCmFromFtAndIns()"  ></asp:TextBox>&nbsp;ft
                                                                <asp:TextBox runat="server" ID="txtHeightIn" Width="20px" MaxLength="3" onfocusout="UpdateCmFromFtAndIns()" onkeypress="return digitValidation(event)" onkeyup="UpdateCmFromFtAndIns()"></asp:TextBox>&nbsp;in
                                                                <div style="padding-top:7px">
                                                                    <asp:TextBox runat="server" ID="txtHeightCm" Width="60px" MaxLength="7" onfocusout="CalculateFtAndInsFromCm()" onkeypress="return digitDecimalValidation(event)" onkeyup="CalculateFtAndInsFromCm()"></asp:TextBox>&nbsp;cm
                                                                    <asp:RegularExpressionValidator ID="CmDecimalValidation" runat="server" ErrorMessage="Invalid height" ValidationExpression="^(([0-9]{0,5}\.[0-9]{0,1})|^(\d{0,5}))$" ControlToValidate="txtHeightCm">*</asp:RegularExpressionValidator>
                                                            </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            Address 2:</td>
                                                        <td colspan="2">
                                                            <asp:TextBox ID="txtAddress2" runat="server" MaxLength="40" Width="300px" onblur="capitilizeInitial(this)"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                           <span style="color:Red">*</span> City:</td>
                                                        <td style="">
                                                            <asp:TextBox ID="txtCity" runat="server" MaxLength="35" Width="200px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revCity" runat="server" ControlToValidate="txtCity"
                                                                ErrorMessage="Please enter a valid city." ValidationExpression="^([a-zA-Z]+[\s-'.]{0,20})*">*</asp:RegularExpressionValidator>                                                       
                                                            <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity"
                                                                ErrorMessage="Please enter a valid city." Width="1px">*</asp:RequiredFieldValidator>
                                                          </td>
                                                            <td align="right">
                                                           <span style="color:Red">*</span> State:</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlState" runat="server" Width="50px"></asp:DropDownList>
                                                             <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState"
                                                                ErrorMessage="Please enter State." Width="1px">*</asp:RequiredFieldValidator>
                                                        </td>
                                                        
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            E-mail:</td>
                                                        <td >
                                                           <asp:TextBox ID="txtEmail" runat="server" MaxLength="80" Width="200px"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                                                                ErrorMessage="Please enter a valid Email address." ValidationExpression="^([a-zA-Z0-9_\-\.\']+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
                                                                Width="1px" Height="1px">*</asp:RegularExpressionValidator> &nbsp
                                                            </td>

                                                        <td align="right" >
                                                            <span style="color:Red">*</span> ZIP Code:
                                                        </td>
                                                        <td style="width:270px">
															<asp:TextBox ID="txtZip" runat="server" MaxLength="9" Width="97px" onkeypress="return numericKeyPressOnly(this, event);"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="txtZip"
                                                                ErrorMessage="Please enter a valid 5 or 9 digit ZIP code." ValidationExpression="^(\d{5})(?:\d{4})?$">*</asp:RegularExpressionValidator>
                                                            <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="txtZip"
                                                                ErrorMessage="Please enter a ZIP code." Width="1px">*</asp:RequiredFieldValidator>
                                                       </td>

                                                    </tr>
                                                     <% if (base.SessionLicense.EnterpriseClient.ShowPharmacy) { %>
                                                    <tr>
                                                        <td nowrap="nowrap" align="right">
                                                            Selected Pharmacy:</td>
                                                        <td colspan="5" style="padding-top:3px">
                                                            <asp:Label ID="lblPharmacy" runat="server" Text="None Entered" Font-Bold="true"></asp:Label>&nbsp&nbsp
                                                            <asp:LinkButton ID="lnkEditPharmacy" runat="server" Text="[edit]" OnClick="lnkEditPharmacy_Click"></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                    <% } %>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblEmployerInd" runat="server" Text="Employer(s):" Visible="false"></asp:Label>&nbsp&nbsp</td>
                                                        <td colspan="5" style="padding-top:3px">
                                                            <asp:Label ID="lblEmployer" runat="server" Text="None Entered" Visible="false"></asp:Label>&nbsp&nbsp
                                                            <asp:LinkButton ID="lnkEditEmployer" runat="server" Text="[edit]" OnClick="lnkEditEmployer_Click" Visible="false"></asp:LinkButton>
                                                        </td>
                                                    </tr> 
                                                    </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" style="height: 138px">
						<asp:MultiView ID="InsurancePlanMultiView" runat="server" OnLoad="InsurancePlanMultiView_Load" >
							<asp:View ID="SponsoredView" runat="server">
							    <table border="0" cellspacing="5" cellpadding="2" visible="false" width="100%">
							        <tr>
							            <td>
                                            <telerik:RadGrid ID="gridCoverages" runat="server" DataSourceID="CoverageDataSource" Skin="Allscripts" EnableEmbeddedSkins="false" AllowSorting="true" OnItemDataBound="gridCoverages_ItemDataBound"
                                                AllowPaging="false" AllowAutomaticDeletes="True" AutoGenerateColumns="False" OnEditCommand="gridCoverages_EditCommand" OnDeleteCommand="gridCoverages_DeleteCommand" OnItemCommand="gridCoverages_ItemCommand">
                                                <MasterTableView DataSourceID="CoverageDataSource" DataKeyNames="SelectedCoverageID,PayorName,BIN,PCN,mob_nabp,PharmacyBenefit,MailOrderBenefit,LTC_Benefit,SpecialtyBenefit,PlanNamePharmacy,PlanNameMOB,PlanNameSpecialty,PlanNameLTC" >
                                                    <HeaderStyle Font-Bold="true" />
                                                    <DetailTables>
                                                        <telerik:GridTableView DataKeyNames="SwitchMessageID" ShowHeader="false" DataSourceID="SwitchMessageDataSource" runat="server">
                                                            <ParentTableRelation>
                                                                <telerik:GridRelationFields DetailKeyField="SelectedCoverageID" MasterKeyField="SelectedCoverageID" />
                                                            </ParentTableRelation>
                                                            <Columns>
                                                                <telerik:GridTemplateColumn>
                                                                    <ItemStyle BackColor="LightYellow" />
                                                                    <ItemTemplate>
                                                                        <asp:Panel ID="pnlDemographics" runat="server" Visible='<%# ((Eval("Name")).ToString().Length > 0 ? true : false) %>'>
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr><td colspan="5">The following information is on record with this insurance plan:</td></tr>
                                                                                <tr>
                                                                                    <td><b>Name:</b></td>
                                                                                    <td><asp:Label ID="lblName" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Name")) %>'></asp:Label></td>
                                                                                    <td>&nbsp</td>
                                                                                    <td><b>Gender, DOB:</b></td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblGender" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Gender")) %>'></asp:Label>,&nbsp
                                                                                        <asp:Label ID="lblDOB" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("DOB")) %>'></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td><b>Address:</b></td>
                                                                                    <td colspan="4">
                                                                                        <asp:Label ID="lblAddress" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Address")) %>'></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>&nbsp</td>
                                                                                    <td colspan="4"><asp:Label ID="lblCityStateZIP" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("CityStateZIP")) %>'></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="5">
                                                                                        <asp:Label ID="lblDemographicWarning" runat="server" ForeColor="red" Text="Demographic differences were found for this patient. Please verify the patient demographic information and make any necessary changes to the patient's profile." Visible='<%# ((Eval("DemographicWarning")).ToString() == "Y" ? true : false) %>'></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </asp:Panel>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                            </Columns>
                                                        </telerik:GridTableView>
                                                    </DetailTables>
                                                    <Columns>
                                                        <telerik:GridTemplateColumn HeaderText="Insurance Plan Information" UniqueName="Name">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblPlanNameDisplayField" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("DisplayField")) %>' Font-Bold="true" Font-Size="Larger"></asp:Label>
                                                                <div style="padding-left:20px">
                                                                    <asp:Repeater ID="repeaterCoverageInfo" runat="server">
                                                                        <ItemTemplate>
                                                                            <li><b><%# ObjectExtension.ToEvalEncode(Eval("CoverageType")) %>:</b> <%# ObjectExtension.ToEvalEncode(Eval("CoverageDescription"))%></li>
                                                                            
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </div>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn UniqueName="Payor" HeaderText="Payor" Visible="false">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblPayorName" runat="server"></asp:Label><br />
                                                                <asp:Label ID="lblBIN" runat="server"></asp:Label><br />
                                                                <asp:Label ID="lblPCN" runat="server"></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>                                                                                                
                                                        <telerik:GridTemplateColumn>
                                                            <ItemTemplate>
                                                                <b>Group Number:</b> <asp:Label ID="lblGroupNumber" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("GroupNumber")) %>'></asp:Label><br />
                                                                <b>Group Name:</b> <asp:Label ID="lblGroupName" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("GroupName")) %>'></asp:Label><br />
                                                                <b>Member ID:</b> <asp:Label ID="lblMemberID" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("CardHolderID")) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridButtonColumn CommandName="Edit" Text="Edit" UniqueName="Edit"></telerik:GridButtonColumn> 
                                                        <telerik:GridButtonColumn CommandName="Delete" ConfirmText="Are you sure you want to delete this plan?" Text="Delete" UniqueName="Delete"></telerik:GridButtonColumn>                     
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                            <asp:ObjectDataSource ID="CoverageDataSource" runat="server" SelectMethod="GetPatientCoverages"
                                                TypeName="Allscripts.Impact.PatientCoverage">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="patientID" SessionField="PatientID" Type="String" />
                                                    <asp:SessionParameter Name="licenseID" SessionField="LicenseID" Type="String" />
                                                    <asp:SessionParameter Name="userID" SessionField="UserID" Type="String" />
                                                    <asp:Parameter Name="showInlineCoverageTypes" DefaultValue="false" Type="Boolean" />
                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                            <asp:ObjectDataSource ID="SwitchMessageDataSource" runat="server" SelectMethod="GetSwitchMessageByCoverageID" TypeName="Allscripts.Impact.Patient">
                                                <SelectParameters>
                                                    <asp:Parameter Name="SelectedCoverageID" Type="Int64" />
                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
							            </td>
							        </tr>
							        <tr>
							            <td><asp:Button ID="btChangeInsurance" runat="server" Text="Add Plan" OnClick="btnChangeInsurance_Click" CssClass="btnstyle" /></td>
							        </tr>
                                </table>
							</asp:View>
							<asp:View ID="NonSponsoredView" runat="server">
							</asp:View>
						</asp:MultiView>
                    </td>
                </tr>
            </table>
        <script type="text/javascript">    
                 RegisterBackButton("<%= btnBack.ClientID %>");
            setAngulrModalStatus();  
            $(document).keypress(function (e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                    return false;
                }
            });
        </script>
    </asp:Content>
    <asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
      <%--  <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
        </asp:UpdatePanel>--%>
</asp:Content>
<asp:Content ID="Content3" runat="server" contentplaceholderid="HeaderPlaceHolder">
    <style type="text/css">
        .auto-style1 {
            width: 61px;
        }
    </style>
</asp:Content>

