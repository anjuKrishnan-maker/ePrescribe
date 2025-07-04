<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_EPCSDigitalSigning" Codebehind="EPCSDigitalSigning.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="pnlEPCSPopUp" runat="server" CssClass="overlaymainwide" Style="display:inline; position:relative;">
<script type="text/javascript" src="../js/epcsDigitalSigningUtil.js"></script>
    <script language="javascript" type="text/javascript">
        function disableEffectiveDate(checkBox, effectiveDate) {
            var eff = $find(effectiveDate);
            if (eff != null) {
                if (checkBox.checked) {
                    eff.set_enabled(false);
                } else {
                    eff.set_enabled(true);
                }
            }
        }

        function pageIsValid() {
            var isValid;
            isValid = true;

            var btnSubmitEPCS = document.getElementById("<%=btnSubmitEPCS.ClientID %>");
            if (btnSubmitEPCS != null) {
                btnSubmitEPCS.disabled = false;
            }
                 
            var passwordLength;
            passwordLength = $("#<%=txtPassword.ClientID%>").val().length;
            if (passwordLength < 8) {
                btnSubmitEPCS.disabled = false;
                alert("Password must be at least 8 characters long.");
                isValid = false;
            }

            var otpLength;
            otpLength = $("#<%=txtOTP.ClientID%>").val().length;
            if (otpLength < 6) {
                btnSubmitEPCS.disabled = false;
                alert("One Time Password (OTP) must be at least 6 characters long.");
                isValid = false;
            }

            var grid = $find("<%=grdEPCSMedList.ClientID %>");
            if (grid.MasterTableView.get_selectedItems().length < 1) {
                btnSubmitEPCS.disabled = false;
                alert("Please select at least one medication.");
                isValid = false;
            }

            return isValid;
        }

        function OnRowSelected(sender, args) {
            var eff = sender.get_masterTableView().get_dataItems()[args.get_itemIndexHierarchical()].findElement("radDatePickerEffectiveDate");
            if (eff != null) {
                if ($find(eff.id) != null) {
                    $find(eff.id).set_enabled(false);
                }
            }

            var btnSubmitEPCS = document.getElementById("<%=btnSubmitEPCS.ClientID %>");
            if (btnSubmitEPCS != null) {
                btnSubmitEPCS.disabled = false;
            }
        }

        function onRowDeSelected(sender, args) {
            var eff = sender.get_masterTableView().get_dataItems()[args.get_itemIndexHierarchical()].findElement("radDatePickerEffectiveDate");
            if (eff != null) {
                if ($find(eff.id) != null) {
                    $find(eff.id).set_enabled(true);
                }
            }

            var grid = $find("<%=grdEPCSMedList.ClientID %>");
            if (grid.MasterTableView.get_selectedItems().length <= 1) {

                var btnSubmitEPCS = document.getElementById("<%=btnSubmitEPCS.ClientID %>");
                if (btnSubmitEPCS != null) {
                    btnSubmitEPCS.disabled = true;
                }
            }
        }

        function disableBtnSubmitEPCS(){
            $("#" + "<%=btnSubmitEPCS.ClientID %>").attr("disabled", "disabled");
        }

        RegisterPopupOvelay('mpeEPCSBhv');
    </script>
    <!-- Update address/city for EPCS send to pharmacy -->
    <asp:Panel ID="panelPatientDemographics" runat="server" Visible="false">
        <div class="overlayTitle">
            EPCS Required Data
        </div>
        <div style="width: 90%; margin: 0 auto">
            <p style="font-size: 11pt; font-weight: normal;">
                DEA regulations require that controlled substance medications sent electronically
                contain city and address 1 for the patient. Fill in required information and press
                <span style="font-size: 11pt; font-weight: bold; background-color: #dddddd;">"Save"</span>.
                If you don’t have required information press <span style="font-size: 11pt; font-weight: bold;
                    background-color: #dddddd;">"Cancel"</span> to print.
            </p>
            <br />
            <br />
        </div>
        <div style="width: 75%; margin:10px">
            <table width="100%" style="padding: 10px 0px 10px 0px; margin-left:10px">
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: red;">*</span> First Name:
                    </td>
                    <td>
                        <asp:TextBox ID="tbFirstName" runat="server" MaxLength="35" CssClass="EpcsPanelTB"
                            ToolTip="First Name" Width="200px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbFirstName" runat="server" Display="Dynamic" ControlToValidate="tbFirstName"  ErrorMessage="First Name is required" ValidationGroup="patDemo"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="retbFirstName" Display="Dynamic" runat="server" ControlToValidate="tbFirstName" ErrorMessage="Please enter a valid first name."
                             ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})$" ValidationGroup="patDemo"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: red;">*</span> Last Name:
                    </td>
                    <td>
                        <asp:TextBox ID="tbLastName" runat="server" MaxLength="60" Width="200px" CssClass="EpcsPanelTB"
                            ToolTip="Last Name"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbLastName" runat="server" Display="Dynamic" ControlToValidate="tbLastName" ErrorMessage="Last Name is required" ValidationGroup="patDemo"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="retbLastName" runat="server" Display="Dynamic" ControlToValidate="tbLastName" ErrorMessage="Please enter a valid Last name."
                            ValidationGroup="patDemo" ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel" style="white-space: nowrap;">
                        <span style="color: red;">*</span> Date of Birth:
                    </td>
                    <td>
                        <telerik:RadDateInput ID="tbDOB" runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy"
                            Skin="Allscripts" MinDate="01/01/1900" Width="100px" ToolTip="Enter Patient's Date of Birth (mm/dd/yyyy)"
                            EnableEmbeddedSkins="false" EmptyMessage="mm/dd/yyyy" SelectionOnFocus="None">
                        </telerik:RadDateInput>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbDOB" runat="server" ValidationGroup="patDemo" ControlToValidate="tbDOB" ErrorMessage="Date of Birth is required."></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: red;">*</span> Gender:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlGender" CssClass="EpcsPanelTB" Style="width: 106px;" runat="server"
                            ToolTip="Gender">
                            <asp:ListItem Text="Male" Value="M" />
                            <asp:ListItem Text="Female" Value="F" />
                            <asp:ListItem Text="Unknown" Value="U" Selected="True" />
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel" style="white-space: nowrap;">
                        <span style="color: Red;">*</span> Address 1:
                    </td>
                    <td>
                        <asp:TextBox ID="tbAddress" MaxLength="55" Width="200px" CssClass="EpcsPanelTB" runat="server"
                            ToolTip="Address"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbAddress" runat="server" ControlToValidate="tbAddress" ValidationGroup="patDemo" ErrorMessage="Address is required."></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel" style="white-space: nowrap;">
                        Address 2:
                    </td>
                    <td>
                        <asp:TextBox ID="tbAddress2" Width="200px" MaxLength="55" CssClass="EpcsPanelTB" runat="server"
                            ToolTip="Address"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: Red;">*</span> City:
                    </td>
                    <td>
                        <asp:TextBox ID="tbCity" MaxLength="35" Width="200px" CssClass="EpcsPanelTB" runat="server" ToolTip="City"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbCity" runat="server" ControlToValidate="tbCity" Display="Dynamic" ErrorMessage="City is required." ValidationGroup="patDemo"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="retbCity" runat="server" ControlToValidate="tbCity" Display="Dynamic" ErrorMessage="Please enter a valid city." ValidationGroup="patDemo" 
                            ValidationExpression="^([a-zA-Z]+[\s-'.]{0,20})*$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: Red;">*</span>State:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="EpcsPanelTB" Style="width: 106px;">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="EpcsPanelLabel">
                        <span style="color: red;">*</span> Zip Code:
                    </td>
                    <td>
                        <asp:TextBox ID="tbZip" MaxLength="5" Width="200px" CssClass="EpcsPanelTB" Style="width: 100px;"
                            runat="server" ToolTip="Zip Code"></asp:TextBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="rftbZip" runat="server" Display="Dynamic" ControlToValidate="tbZip" ErrorMessage="Zip is required." ValidationGroup="patDemo"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="retbZip" runat="server" Display="Dynamic" ControlToValidate="tbZip" ErrorMessage="Please enter a valid 5-digit ZIP code." 
                            ValidationExpression="^\d{5}$" ValidationGroup="patDemo"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;<span style="color: Red;">*</span> = required fields
                    </td>
                    <td style="white-space: nowrap;">
                    </td>
                </tr>
            </table>
        </div>
         <div class="overlayFooter">
         <table width="100%">
            <tr>
                <td align="right" style=" width:"90%">
                <asp:Button ID="btnSavePatientDemo" CssClass="btnstyle btnStyleAction" runat="server" Width="70px" CausesValidation="true" Text="Save" OnClick="btnSavePatientDemo_Click" ValidationGroup="patDemo" />&nbsp;
                </td>          
                
                <td align="left"  style="width:"10%">                 
                  <asp:Button ID="btnCancelFromPatientDemo" CssClass="btnstyle" Width="70px" runat="server" Text="Cancel" OnClick="btnCancelFromPatientDemo_Click" />
                </td>
          </tr>
           </table>
                  </div>
    </asp:Panel>
    <!-- if the enterprise edit patient option is false, display this overlay instead of the Address1/City overlay -->
    <asp:Panel ID="panelEpcsEnterpriseEditPatientOff" Visible="false" runat="server" Style="text-align: center">
        <div>
            <h3>
                EPCS Required Data
            </h3>
            <p style="font-weight:bold; color:Red; padding-bottom:15px">This patient is missing address 1 and/or city.</p>
            <p>
                Your enterprise setting to Edit patients is turned OFF.<br />
                As per DEA regulations, address 1 and city are required at this point.
                <br /><br />
                Until this patient is updated with a valid address 1 and city, you can only print controlled substance medications.<br />
            </p>
        </div>
        <div style="margin-top: 15px">
            <asp:Button ID="btnEpcsEnterpriseEditPatientOffOk" runat="server" CssClass="btnstyle"
                Style="width: 70px;" Text="OK" OnClick="btnEpcsEnterpriseEditPatientOffOk_Click" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlSecondFactorAlertPopUp" runat="server" Visible="false">
        <div style="text-align: left;">
            <h3 style="text-align: center;">
                Select Second Factor Form</h3>
            You have the following second factor forms. Please choose which form you want to
            use for this transaction.<br />
            <br />
            <asp:RadioButtonList ID="rblOtpForms" runat="server">
            </asp:RadioButtonList>
            <br />
            <br />
        </div>
        <div style="text-align: right">
            <asp:Button ID="btnSelectSecondFactorForm" runat="server" Text="Proceed" OnClick="btnSelectOtpForm_Click" />
            <asp:Button ID="btnCancelSecondFactorForm" runat="server" Text="Cancel" OnClick="btnCancelSecondFactorForm_Click" />
        </div>
    </asp:Panel>
    <!-- select DEA number and meds and then submit for digital signing -->
    <asp:Panel ID="panelEPCSDigitalSigning" runat="server" Visible="false" DefaultButton="btnSubmitEPCS">
        <div style="width: 100%">
            <div class="overlayTitle">
                Electronic Prescribing of a Controlled Substance Confirmation
            </div>
        </div>
        <div style="margin-top: 10px; margin-bottom: 10px; margin-left: 10px; margin-right: 10px;
            width: 100%">
            <table id="epcsHeaderTable1" style="width: 100%">
                <tr>
                    <td style="text-align: right">
                        <span style="font-weight: bold">Rx Date:</span>
                    </td>
                    <td>
                        <asp:Literal ID="litDate" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <span style="font-weight: bold">Provider:</span>
                    </td>
                    <td>
                        <asp:Literal ID="litProviderInfo" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <span style="font-weight: bold">Patient:</span>
                    </td>
                    <td>
                        <asp:Literal ID="litPatientInfo" runat="server"></asp:Literal>
                    </td>
                </tr>
                <asp:Panel ID="divSupervisor" runat="server">
                    <tr>
                        <td style="text-align: right">
                            <span style="font-weight: bold">Supervisor:</span>
                        </td>
                        <td>
                            <asp:Literal ID="litSupervisorInfo" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="divPharmacy" runat="server">
                    <tr>
                        <td style="text-align: right">
                            <span style="font-weight: bold">Pharmacy:</span>
                        </td>
                        <td>
                            <asp:Literal ID="litPharmacy" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </div>
        <div style="margin: 10px 10px 10px 10px; position:relative">
            <telerik:RadGrid ID="grdEPCSMedList" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="true"
                Skin="Allscripts" EnableEmbeddedSkins="false" GridLines="None" OnItemDataBound="grdEPCSMedList_ItemDataBound"
                Width="100%" onprerender="grdEPCSMedList_PreRender" CssClass="epcs-sign">
                <ClientSettings>
                    <Selecting AllowRowSelect="True"></Selecting>
                    <ClientEvents OnRowSelecting="OnRowSelected" OnRowDeselecting="onRowDeSelected" />
                    <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                </ClientSettings>
                <MasterTableView GridLines="None" ShowHeader="true" ShowHeadersWhenNoRecords="true"
                    TableLayout="Fixed" DataKeyNames="RxID, ScriptMessageID">
                    <HeaderStyle Font-Bold="true" />
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" />
                        <telerik:GridTemplateColumn HeaderText="Medication and Sig" UniqueName="MedicationAndSig">
                            <ItemTemplate>
                                <asp:Literal ID="litMedicationAndSig" runat="server"></asp:Literal>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Quantity" DataField="QuantityFormattedString"
                            UniqueName="Quantity">
                            <ItemStyle HorizontalAlign="Center" />
                        </telerik:GridBoundColumn>
                        <telerik:GridCheckBoxColumn HeaderText="DAW" DataField="DAW" UniqueName="DAW">
                            <ItemStyle HorizontalAlign="Center" />
                        </telerik:GridCheckBoxColumn>
                        <telerik:GridBoundColumn HeaderText="Refills" DataField="Refills" UniqueName="Refills">
                            <ItemStyle HorizontalAlign="Center" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Days" DataField="DaysSupply" UniqueName="Days">
                            <ItemStyle HorizontalAlign="Center" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Effective Date" UniqueName="EffectiveDate">
                            <ItemTemplate>
                                <telerik:RadDatePicker ID="radDatePickerEffectiveDate" runat="server" SelectedDate='<%# Bind("EffectiveDate") %>'
                                    ZIndex="100002" Enabled="false" Width="120px" >
                                    <DateInput DateFormat="dd-MMM-yyyy" />
                                </telerik:RadDatePicker>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </div>
        <div style="margin-bottom: 10px; margin-left: 10px; margin-right: 10px; width: inherit">
            <p style="margin: 0px auto;" class="smalltext">
                * By completing the two-factor authentication protocol at this time, you are legally
                signing the prescription(s) and authorizing the transmission of the above information
                to the pharmacy for dispensing. The two-factor authentication protocol may only
                be completed by the practitioner whose name and DEA registration number appear here.
            </p>
            <div style="margin-top: 10px; width: inherit">
                <div style="float: right; width: 40%; text-align: center">
                    <asp:Button ID="btnSubmitEPCS" runat="server" Text="Electronically Sign and Send"
                        OnClick="btnSubmitEPCS_Click" CssClass="btnstyle btnStyleAction" Height="55px" Width="150px"
                        Enabled="True" OnClientClick="btnSubmitEpcsClientClick(this)" UseSubmitBehavior="false" />
                    <br />
                    <br />
                    <asp:Button ID="btnCancelEPCS" runat="server" Text="Cancel" CssClass="btnstyle" Width="150px"
                        OnClick="btnCancelEPCS_Click" />
                </div>
                <div style="width: 50%;">
                    <table style="margin: 0px auto; width: 100%">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblAuthenticationStatus" runat="server" ForeColor="Red" Font-Bold="true"
                                    Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <b>User Name:</b>
                            </td>
                            <td>
                                <asp:Literal ID="litUserName" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <b>DEA Number:</b>
                            </td>
                            <td>
                                <asp:Literal ID="litDEAList" runat="server">
                                </asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <label>
                                    <b>Password:</b></label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: right">
                                <label>
                                    <b>One Time Password (O.T.P):</b></label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtOTP" runat="server" TextMode="Password"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpeEPCS" runat="server" TargetControlID="btnPopUpTrigger"
    DropShadow="false" PopupControlID="pnlEPCSPopUp" BackgroundCssClass="modalBackground" BehaviorID="mpeEPCSBhv">
</ajaxToolkit:ModalPopupExtender>
<asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
