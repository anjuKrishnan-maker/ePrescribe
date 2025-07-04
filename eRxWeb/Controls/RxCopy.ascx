<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_RxCopy" Codebehind="RxCopy.ascx.cs" %>
<script language="javascript" type="text/javascript">
    function rxCopyOverlayIsValid() {
        var daysSupply;
        daysSupply = $("#<%=txtDaysSupply.ClientID%>").val().trim();

        var quantity;
        quantity = $("#<%=txtQuantity.ClientID%>").val().trim();

        var refill;
        refill = $("#<%=txtRefill.ClientID%>").val().trim();

        var daysSupplyLength;
        daysSupplyLength = daysSupply.length;
        if (daysSupplyLength < 1) {
            alert("Days supply is required.");
            return false;
        }

        var quantityLength;
        quantityLength = quantity.length;
        if (quantityLength < 1) {
            alert("Quantity is required.");
            return false;
        }

        var refillLength;
        refillLength = refill.length;
        if (refillLength < 1) {
            alert("Refill is required.");
            return false;
        }

        if (!isWholeNumber(daysSupply)) {
            alert("Days supply must be a whole number between 1 and 365.");
            return false;
        }

        if (!isWholeNumber(refill)) {
            alert("Refills must be a whole number between 0 and 99.");
            return false;
        }

        if (!isNumeric(daysSupply)) {
            alert("Days supply is invalid.");
            return false;
        }

        if (!isNumeric(quantity)) {
            alert("Quantity is invalid.");
            return false;
        }

        if (!isNumeric(refill)) {
            alert("Refill is invalid.");
            return false;
        }

        return true;
    }

    function isWholeNumber(number) {
        if (number.toString().indexOf(".") > -1)
            return false; // it is float
        else
            return true; //it is integer
    }

    function isNumeric(sText) {
        sText = sText.replace(/^\s+|\s+$/g, '');
        var validChars = "0123456789.";
        var isNumber = true;
        var char;

        if (sText == "") {
            isNumber = false;
        }
        else {
            for (i = 0; i < sText.length && isNumber == true; i++) {
                char = sText.charAt(i);
                if (validChars.indexOf(char) == -1) {
                    isNumber = false;
                }
            }
        }

        return isNumber;
    }
</script>
<asp:Panel ID="pnlRxCopyPopUp" runat="server" CssClass="overlaymainwide" Style="display: none;
    position: relative;" DefaultButton="btnSaveAndReviewRxCopy">
    <div>
        <div class="overlayTitle">
            Create Additional Prescription
        </div>
        <asp:Panel ID="pnlMainRxCopy" runat="server">
            <div class="overlayContent">
            <table border="0" cellpadding="2" cellspacing="0">
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblInstructions" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center" style="padding-top: 15px">
                        <asp:Label ID="lblMedication" runat="server" Font-Size="Medium" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center" style="padding-bottom: 15px">
                        <asp:Label ID="lblSig" runat="server" Font-Size="Medium" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="lblDaysSupplyAsterisk" runat="server" ForeColor="Red">*</asp:Label>
                        <asp:Label ID="lblDaysSupply" runat="server" Text="Days Supply:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDaysSupply" runat="server" MaxLength="3" autocomplete="off" Width="50px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Panel ID="pnlNonPillMed" runat="server" Width="100%">
                            <table>
                                <tr>
                                    <td style="text-align: right">
                                        Choose Package/Unit:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCustomPack" runat="server" DataTextField="PackageDescription"
                                            Width="264px" DataValueField="PSPQ" OnPreRender="ddlCustomPack_PreRender">
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
                        <asp:Label ID="Label2" runat="server" Text="Quantity:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtQuantity" autocomplete="off" runat="server" MaxLength="8" Width="50px"></asp:TextBox>
                        <asp:DropDownList ID="ddlUnit" runat="server" Visible="false">
                            <asp:ListItem>ML</asp:ListItem>
                            <asp:ListItem>EA</asp:ListItem>
                            <asp:ListItem Value="UN">Units</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Label ID="lblQuantity" runat="server" ForeColor="Red"></asp:Label>
                        <asp:TextBox ID="txtQuantityUnits" runat="server" Style="display: none; visibility: hidden"
                            TabIndex="-1">0</asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <span style="color: Red">*</span>
                        <asp:Label ID="lblRefills" runat="server" Text="Refills:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtRefill" autocomplete="off" runat="server" MaxLength="2" Width="50px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkDAW" runat="server" Text="Dispense As Written: " TextAlign="Left" />
                        <asp:Label ID="lblMDD" runat="server" Text="&nbsp;&nbsp;&nbsp;&nbsp; MDD:" Visible="false"></asp:Label>
                        <asp:TextBox ID="txtMDD" runat="server" autocomplete="off" MaxLength="20" Visible="false" Width="264px"></asp:TextBox>
                        <asp:Label ID="lblPerDay" runat="server" Text="Per Day" Visible="false"></asp:Label>

                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div id="maxCharacterWarning" runat="server" style="margin-top: 10px;">
                            Special instructions to pharmacist:
                            <br />
                            <span class="smalltext">Note: should not be used for patient instructions or comments</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 79px">
                        <asp:TextBox ID="txtPharmComments" runat="server" TextMode="MultiLine" Width="684px"
                            Height="57px" MaxLength="210"></asp:TextBox>
                        <div id="maxCharacterRemaining" runat="server" class="normaltext">
                            (Maximum <span id="maxCharacters" runat="server">210</span> Characters / <span id="charsRemaining"
                                runat="server">210</span> characters remaining)
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 79px">
                        <div id="DivBridge" runat="server" style="margin-top: 10px;">
                            If this prescription is being sent to a mail order pharmacy, please use the Special
                            instructions to the pharmacist field above to indicate on both prescriptions (mail
                            and retail) that this prescription has also been sent to retail / mail.
                            <br />
                        </div>
                    </td>
                </tr>
            </table>
                </div>
            <div class="overlayFooter">
                <table border="0" cellpadding="2" cellspacing="0">
                            <tr>
                                <td align="right" width="500px">
                                    <asp:Button ID="btnCancelRxCopy" runat="server" CausesValidation="False" CssClass="btnstyle"
                                        OnClick="btnCancelRxCopy_Click" Text="Back" />
                                </td>
                                <td align="left" width="200px">
                                    <asp:Button ID="btnSaveAndReviewRxCopy" runat="server" CssClass="btnstyle btnStyleAction" OnClick="btnSaveAndReviewRxCopy_Click"
                                        OnClientClick="if (!rxCopyOverlayIsValid()) return false;" Text="Save & Review"
                                        Width="200px" />
                                </td>
                            </tr>
                        </table>
            </div>
        </asp:Panel>
    </div>
    <input id="maxlenContainer" runat="server" type="text" style="display: none;" value="210" />
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpeRxCopy" runat="server" TargetControlID="btnRxCopyPopUpTrigger"
    DropShadow="false" PopupControlID="pnlRxCopyPopUp" BackgroundCssClass="modalBackground">
</ajaxToolkit:ModalPopupExtender>
<asp:Button ID="btnRxCopyPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />