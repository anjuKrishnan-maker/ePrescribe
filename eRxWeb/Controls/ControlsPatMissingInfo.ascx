<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.ControlsPatMissingInfo" CodeBehind="ControlsPatMissingInfo.ascx.cs" %>
<%@ Register Src="/Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>

<asp:Panel ID="pnlPatientDemographicsEditPopUp" runat="server" CssClass="overlaymainwide" Style="display:none; position: relative;">
    <!-- Update address/city for send to MO pharmacy -->
    <asp:Panel ID="panelPatientDemographics" runat="server">
        <script type="text/javascript" language="javascript">

        function digitValidation(evt) 
        {
            if(evt.keyCode >= 96 && evt.keyCode <= 105)//numpad
                return true;
          
            if (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
                return false;
        }
       
        function digitDecimalValidation(evt) 
        {
            if (evt.keyCode == 190 || evt.keyCode == 110)
                return true;

            if(evt.keyCode >= 96 && evt.keyCode <= 105)//numpad
                return true;
           
            if (evt.keyCode > 31 && (evt.keyCode < 48 || evt.keyCode > 57))
                return false;
    }

        function convertKgToLbsAndOzs() {
            var kgValue = $('#<%= txtWeightKg.ClientID %>').val();
                if (kgValue !== "") {
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
        }
        function convertCmToFtAndInches() {
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
                    inches = 0 ;
                    feet++;
                }
                $('#<%= txtHeightFt.ClientID %>').val(feet);
                $('#<%= txtHeightIn.ClientID %>').val(inches);
            }
         }

        function updateKg() {
            var lbs = document.getElementById( '<%= txtWeightLbs.ClientID %>').value;
            var ozs = document.getElementById('<%= txtWeightOzs.ClientID %>').value;
            if (lbs !== "" || ozs !== "") {
                var kgs = lbs * .45359237;
                var kgs = kgs + (ozs / 16) * .45359237;
                $('#<%= txtWeightKg.ClientID %>').val(kgs.toFixed(2));
            }
        }
            
        function updateCm() {
             var feet = document.getElementById('<%= txtHeightFt.ClientID %>').value;
            var inches = document.getElementById('<%= txtHeightIn.ClientID %>').value;
            if (inches == "")
                inches = 0;
                var cm = (feet * 30.48) + (inches * 2.54);
                $('#<%= txtHeightCm.ClientID %>').val(cm.toFixed(1));
        }
        
	</script>
        <div class="overlayTitle">
            Prescription Required Data
        </div>
        <div class="overlayContent">
            <div style="margin: 0 auto">
                <p style="font-size: 11pt; font-weight: normal;">
                    Prescriptions delivered to pharmacies electronically requires all the patient data. Please fill in the required information and press 
                    <span style="font-size: 11pt; font-weight: bold; background-color: #dddddd;">"Save"</span>.
                    If you do not have the required information press <span style="font-size: 11pt; font-weight: bold; background-color: #dddddd;">"Cancel"</span>.
                </p>
                <br/>
                <br/>
            </div>
            <div style = " width: 70%; margin: 0 auto">
                <table style="width: 100%; padding: 10px 0 10px 0;">
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>
                            First Name:
                        </td>
                        <td class="valgn">
                            <asp:TextBox ID="txtFirstName" runat="server" MaxLength="35"
                                         ToolTip="First Name"></asp:TextBox>
                            <span id="errFirstName" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                        <td>
                            <span id="errWeight" runat="server" visible="false">
                            <img class="valgn" src="/images/required.png" /></span>
                            Current Weight
                        </td>
                        <td style="padding-top:3px;">
                        <asp:TextBox runat="server" ID="txtWeightLbs" Width="40px"  MaxLength="4" onfocusout="updateKg()" onkeyup="updateKg()" onkeydown="return digitValidation(event)"></asp:TextBox>&nbsp;lb
                             <span id="errWeightLb" runat="server" visible="false">
                             <img class="valgn" src="/images/required.png" /></span>
                    &nbsp;<asp:TextBox runat="server" ID="txtWeightOzs" Width="20px" MaxLength="2" onfocusout="updateKg()" onkeyup="updateKg()" onkeydown="return digitValidation(event)"></asp:TextBox>&nbsp;oz
                             <span id="errWeightOz" runat="server" visible="false">
                             <img class="valgn" src="/images/required.png" /></span>
                    </td> 
                    </tr>
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>
                            Last Name:
                        </td>
                        <td>
                            <asp:TextBox ID="txtLastName" runat="server" MaxLength="60"
                                         ToolTip="Last Name"></asp:TextBox>
                            <span id="errLastName" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                        <td>
                        </td>
                        <td style="padding-top:3px;">
                            <asp:TextBox runat="server" ID="txtWeightKg" Width="60px" MaxLength="6" onfocusout="convertKgToLbsAndOzs()" onkeydown="return digitDecimalValidation(event)" onkeyup="convertKgToLbsAndOzs()" ></asp:TextBox>&nbsp;kg
                                <span id="errKg" runat="server" visible="false">
                                <img class="valgn" src="/images/required.png" /></span>
                           </td> 
                    </tr>
                    <tr>
                        <td style="white-space: nowrap;">
                            <img class="valgn" src="/images/required.png"/>
                            Date of Birth:
                        </td>
                        <td>
                            <telerik:RadDateInput ID="txtDOB" runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy"
                                                  Skin="Allscripts" MinDate="01/01/1900" Width="100px" ToolTip="Enter Patient's Date of Birth (mm/dd/yyyy)"
                                                  EnableEmbeddedSkins="false" EmptyMessage="mm/dd/yyyy" SelectionOnFocus="None">
                            </telerik:RadDateInput>
                            <span id="errDOB" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                        <td>
                            <span id="errHeight" runat="server" visible="false">
                            <img class="valgn" src="/images/required.png" /></span>
                            Current Height
                        </td>
                        <td style="padding-top:3px;">
                            <asp:TextBox runat="server" ID="txtHeightFt" Width="40px" MaxLength="2" onfocusout="updateCm()" onkeyup="updateCm()" onkeydown="return digitValidation(event)"></asp:TextBox>&nbsp;ft
                                <span id="errHeightFt" runat="server" visible="false">
                                <img class="valgn" src="/images/required.png" /></span>
                            &nbsp;<asp:TextBox runat="server" ID="txtHeightIn" Width="20px" MaxLength="3" onfocusout="updateCm()" onkeyup="updateCm()" onkeydown="return digitValidation(event)"></asp:TextBox>&nbsp;in
                                <span id="errHeightIn" runat="server" visible="false">
                                <img class="valgn" src="/images/required.png" /></span>
                        </td>                                  
                    </tr>
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>
                            Gender:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlGender" Style="width: 106px;" runat="server"
                                              ToolTip="Gender">
                                <asp:ListItem Text="Male" Value="M"/>
                                <asp:ListItem Text="Female" Value="F"/>
                                <asp:ListItem Text="Unknown" Value="U" Selected="True"/>
                            </asp:DropDownList>
                        </td>
                        <td>
                        </td>
                        <td>
                        <div style="padding-top:7px">
                            <asp:TextBox runat="server" ID="txtHeightCm" Width="60px" MaxLength="7" onfocusout="convertCmToFtAndInches()" onkeyup="convertCmToFtAndInches()" onkeydown="return digitDecimalValidation(event)"></asp:TextBox>&nbsp;cm
                                 <span id="errCm" runat="server" visible="false">
                                <img class="valgn" src="/images/required.png" /></span>
                       </td>
                    </tr>
                    <tr>
                        <td style="white-space: nowrap;">
                            <img class="valgn" src="/images/required.png"/>
                            Address 1:
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddress" MaxLength="40" runat="server"
                                         ToolTip="Address"></asp:TextBox>
                            <span id="errAddress1" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="white-space: nowrap;">Address 2:
                        </td>
                        <td>
                            <asp:TextBox ID="txtAddress2" MaxLength="40" runat="server"
                                         ToolTip="Address"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>
                            City:
                        </td>
                        <td>
                            <asp:TextBox ID="txtCity" MaxLength="35" runat="server" ToolTip="City"></asp:TextBox>
                            <span id="errCity" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>State:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlState" runat="server" Style="width: 106px;">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <img class="valgn" src="/images/required.png"/>
                            Zip Code:
                        </td>
                        <td>
                            <asp:TextBox ID="txtZip" MaxLength="5" Style="width: 100px;"
                                         runat="server" ToolTip="Zip Code"></asp:TextBox>
                            <span id="errZip" runat="server" style="visibility: hidden;">
                            <img class="valgn" src="/images/required.png" /></span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;<img class="valgn" src="/images/required.png"/>
                            = required fields
                        </td>
                        <td style="white-space: nowrap;"></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <ePrescribe:Message ID="msgErr" runat="server" Visible="false"/>
                        </td>
                    </tr>
                </table>
            </div></div>
        <div class="overlayFooter">
            <asp:Button ID="btnSavePatientDemo" CssClass="btnstyle btnStyleAction" runat="server" Width="70px" Text="Save" OnClick="btnSavePatientDemo_Click" />&nbsp;
            <asp:Button ID="btnCancelFromPatientDemo" CausesValidation="False" CssClass="btnstyle" Width="70px" runat="server" Text="Cancel" OnClick="btnCancelFromPatientDemo_Click" />
        </div>
    </asp:Panel>
    <!-- if the enterprise edit patient option is false, display this overlay instead of the Address1/City overlay -->
    <asp:Panel ID="panelEnterpriseEditPatientOff" runat="server" Style="text-align: center">
        <div>
            <h3>Mail Order Required Data
            </h3>
            <p style="font-weight: bold; color: Red; padding-bottom: 15px">This patient is missing address 1 and/or city.</p>
            <p>
                Your enterprise setting to Edit patients is turned OFF.<br />
                As per Mail Order requirements, address 1 and city are required at this point.
                <br />
                <br />
                Until the patient is updated with a valid address 1 and city, you cannot electronically send this prescription to a Mail order Pharmacy.<br />
            </p>
        </div>
        <div style="margin-top: 15px">
            <asp:Button ID="btnEnterpriseEditPatientOffOk" runat="server" CssClass="btnstyle"
                Style="width: 70px;" Text="OK" OnClick="btnEnterpriseEditPatientOffOk_Click" />
        </div>
    </asp:Panel>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpePatientDemoEdit" runat="server" TargetControlID="btnPopUpTrigger"
    DropShadow="false" PopupControlID="pnlPatientDemographicsEditPopUp" BackgroundCssClass="modalBackground">
</ajaxToolkit:ModalPopupExtender>
<asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
