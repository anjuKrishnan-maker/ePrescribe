<%@ Page Title="Search Medication" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" CodeBehind="PreBuiltSelectMed.aspx.cs" Inherits="eRxWeb.PreBuiltSelectMed" %>

<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function IsValidSearchText() {
                var txtSearchMed = document.getElementById('<%=txtSearchMed.ClientID%>');
                if (txtSearchMed != null && txtSearchMed.value != "") {
                    return true;
                }
                else {
                    alert("Please enter at least 2 valid characters for Medication Name.");
                    return false;
                }
            }

            function medSelected(source, eventArgs) {
                GridSingleSelect(eventArgs.getDataKeyValue("DDI").toString(), "DDI", "<%= rgMedication.MasterTableView.ClientID %>", true);
                getMedInfo();
            }

            function getMedInfo() {
                var btnSelectSig = document.getElementById("<%=btnSelectSig.ClientID %>");
                if (btnSelectSig != null) {
                    btnSelectSig.disabled = false;
                }

            }

            function medSelectRadio(ddi) {
                GridSingleSelect(ddi, "DDI", "<%= rgMedication.MasterTableView.ClientID %>", true)
                getMedInfo();
            }

            function GridSingleSelect(keyValue, dataKeyName, gridClientID, selectOnGrid) {
                var controlIndex = 0;
                var items = $find(gridClientID).get_dataItems();
                for (i = 0; i < items.length; i++) {
                    if (keyValue.toString() == items[i].getDataKeyValue(dataKeyName).toString()) {
                        selectRadio(keyValue, dataKeyName)
                        if (selectOnGrid) {
                            items[i].set_selected(true);
                        }
                    }
                    else {
                        items[i].set_selected(false);
                    }
                }
            }

            function selectRadio(keyValue, dataKeyName) {
                for (j = 0; j < document.forms[0].elements.length; j = j + 1) {
                    var control = document.forms[0].elements[j];
                    if (control.type == "radio") {
                        if (control.getAttribute(dataKeyName) != null) {
                            if (control.getAttribute(dataKeyName).toString() == keyValue) {
                                control.checked = "checked";
                            }
                            else {
                                control.checked = "";
                            }
                        }
                    }
                }
            }

        </script>
    </telerik:RadCodeBlock>
    <table width="100%" cellspacing="0" border="0" cellpadding="0">
        <tr>
            <td>
                <table cellspacing="0" border="0" cellpadding="0" width="100%">
                    <tr class="h1title">
                        <td class="Phead indnt">
                            <asp:Label ID="lblheader" Style="color: White; font-weight: bold;" runat="server"
                                Text="Edit Prescription Group" />
                        </td>
                    </tr>
                    <tr>
                        <td class="message">
                            <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr valign="middle">
                        <td>
                            <table border="0" style="border-collapse: collapse; border-bottom-color: #E7E7E7;"
                                cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h2title" align="left">
                                    <td style="color: White;width:150px" class="valgn">&nbsp;&nbsp;<b>Choose Medication</b>
                                    </td>
                                    <td align="left">
                                        <table border="0" style="border-collapse: collapse; border-bottom-color: #E7E7E7;"
                                            cellpadding="2" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <table border="0" style="border-collapse: collapse; border-bottom-color: #E7E7E7;"
                                                        cellpadding="2" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox autocomplete="off" MaxLength="50" ID="txtSearchMed" CssClass="searchTextBox"
                                                                    Width="185px" ToolTip="Enter Partial or Full Medication" runat="server" TabIndex="1"></asp:TextBox>
                                                                <ajaxToolkit:AutoCompleteExtender ID="aceMed" runat="server" TargetControlID="txtSearchMed"
                                                                    ServiceMethod="queryMeds" ServicePath="erxnowmed.asmx" CompletionInterval="1000"
                                                                    MinimumPrefixLength="3" EnableCaching="true">
                                                                </ajaxToolkit:AutoCompleteExtender>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="5">&nbsp;&nbsp;
                                                </td>
                                                <td class="valgn">
                                                    <asp:Button ID="btnSearch" ToolTip="Shows all medications matching the text entered in the Search field. Enter partial or full medication name in the text-box."
                                                        runat="server" Text="Search" CssClass="btnstyle" Width="80px" TabIndex="2" OnClick="btnSearch_Click"
                                                        OnClientClick="return IsValidSearchText()" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table width="100%" border="0" cellspacing="0">
                                            <tr>
                                                <td style="height: 14px">
                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                        <tr style="border: 1px solid #000000; background-color: #d2d8db;">
                                                            <td>
                                                                <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" TabIndex="3" OnClick="btnBack_Click" />
                                                                <asp:Button ID="btnSelectSig" runat="server" CssClass="btnStyleOneArrow" Width="100px" Text="Select SIG"
                                                                    Enabled="False" ToolTip="Click to select a SIG." OnClick="btnSelectSig_Click" TabIndex="4" />&nbsp;&nbsp;
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <telerik:RadGrid ID="rgMedication" runat="server" EnableEmbeddedSkins="false" Skin="Allscripts"
                                            ShowStatusBar="false" AllowAutomaticUpdates="False" AllowMultiRowSelection="false"
                                            EnableViewState="true" AllowPaging="True" AllowSorting="false" Style="width: 100%"
                                            PageSize="50" AutoGenerateColumns="False" OnPageIndexChanged="rgMedication_PageIndexChanged"
                                            OnItemDataBound="rgMedication_OnItemDataBound">
                                            <PagerStyle Mode="NextPrevAndNumeric" />
                                            <MasterTableView GridLines="None" NoMasterRecordsText="No Medication Found" Style="width: 100%"
                                                CommandItemDisplay="None" DataKeyNames="DDI,MedicationName,Strength,StrengthUOM,DosageFormCode,RouteOfAdminCode" ClientDataKeyNames="DDI">
                                                <HeaderStyle Font-Bold="true" Font-Size="13px" />
                                                <Columns>
                                                    <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                                                        <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
                                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                        <ItemTemplate>
                                                            <input id="rbSelect" runat="server" type="radio" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn DataField="MedicationName" HeaderText="Drug Name" />
                                                    <telerik:GridBoundColumn DataField="Strength" HeaderText="Strength" />
                                                    <telerik:GridBoundColumn DataField="StrengthUOM" HeaderText="Unit" />
                                                    <telerik:GridBoundColumn DataField="DosageFormCode" HeaderText="Dosage Form" />
                                                    <telerik:GridBoundColumn DataField="RouteOfAdminCode" HeaderText="Route" />
                                                </Columns>
                                            </MasterTableView>
                                            <ClientSettings>
                                                <ClientEvents OnRowClick="medSelected" />
                                                <Selecting AllowRowSelect="True"></Selecting>
                                            </ClientSettings>
                                        </telerik:RadGrid>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   
</asp:Content>

