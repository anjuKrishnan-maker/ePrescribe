<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_PatientSearch" Codebehind="PatientSearch.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript">
//<![CDATA[
    function showMenu(e) {
        $("#ctl00_ContentPlaceHolder1_ucPatientSearchWithResults_ucPatientSearch_searchTypeMenu_detached").css('z-index', '700000');
        var contextMenu = $find("<%= searchTypeMenu.ClientID %>");
        var vBtnSearch = document.getElementById("<%= btnSearch.ClientID %>");
        if (vBtnSearch != null && vBtnSearch.disabled == false) {
            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }
            $telerik.cancelRawEvent(e);
        }
    }
    function ShowOnTop(sender, args) {

        var contextMenu = $find("<%= searchTypeMenu.ClientID %>");

        if (contextMenu != null) {
            contextMenu.get_element().style.zIndex = "100001";
        } else {
            alert("APS_ShowOnTop failed.");
        }
    }  
//]]>
</script>
<asp:Panel ID="searchError" runat="server" Style="padding: 5px 10px; width: auto;
    height: auto; background-color: White; border: solid 1px red">
    <span id="spanSearchError" runat="server" style="color: Red"></span>
</asp:Panel>
<table border="0" cellpadding="0" cellspacing="5" style="display: inline">
    <tr>
        <td align="left" valign="middle">
            <asp:Panel ID="baseSearch" runat="server" Wrap="false" Direction="LeftToRight" DefaultButton="btnSearch">
                <table border="0" cellpadding="0" cellspacing="5">
                    <tr>
                        <td>
                            <asp:Label ID="lblLastName" runat="server" Text="Last Name: " CssClass="Phead"></asp:Label>
                            <telerik:RadTextBox ID="txtLastName" runat="server" EnableEmbeddedSkins="false" Skin="Allscripts"
                                Width="82px" ToolTip="Enter patient's last name" TabIndex="1">
                            </telerik:RadTextBox>
                            <asp:Label ID="lblFirstName" runat="server" Text="First Name: " CssClass="Phead"></asp:Label>
                            <telerik:RadTextBox ID="txtFirstName" runat="server" EnableEmbeddedSkins="false"
                                Skin="Allscripts" Width="82px" ToolTip="Enter patient's first name" TabIndex="2">
                            </telerik:RadTextBox>
                            <asp:Label ID="lblDOB" runat="server" Text="DOB: " CssClass="Phead"></asp:Label>
                            <telerik:RadDateInput ID="rdiDOB" runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy"
                                Skin="Allscripts" Width="75px" ToolTip="Enter Patient's Date of Birth (mm/dd/yyyy)"
                                EnableEmbeddedSkins="false" EmptyMessage="mm/dd/yyyy" EmptyMessageStyle-ForeColor="GrayText"
                                EmptyMessageStyle-Font-Italic="true" TabIndex="3">
                            </telerik:RadDateInput>
                            <asp:Label ID="lblPatientID" runat="server" Text="Patient ID: " CssClass="Phead"></asp:Label>
                            <telerik:RadTextBox ID="txtPatientID" runat="server" EnableEmbeddedSkins="false"
                                Skin="Allscripts" Width="75px" ToolTip="Enter patient's MRN" TabIndex="4">
                            </telerik:RadTextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td align="left" valign="middle">
            <asp:Panel ID="Search" runat="server" CssClass="combobtnstylespan" Style="padding-right: 0px; margin-top: 5px;">
                <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle;">
                    <tr>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" CssClass="combobtnstyle" OnClick="btnSearch_Click"
                                Text="Search" ToolTip="Search patient" CausesValidation="false" Style=""
                                TabIndex="5" />
                            <img id="Img1" runat="server" onclick="showMenu(event)" src="../images/arrowdown-nor-dark-12-x-12.png"
                                          class="combobtnstyle" style="display: inline; height: 8px; width: auto; margin-left: 5px; margin-right: 5px;"
                                           tabindex="6"/>
                        </td>
                        <td>
                            <telerik:RadContextMenu ID="searchTypeMenu" runat="server" OnItemClick="setTypeAndSearch"
                                Style="z-index: 100002">
                                <Items>
                                    <telerik:RadMenuItem Text="Search Active Patients" Value="0" />
                                    <telerik:RadMenuItem Text="Search Active + Inactive Patients" Value="1" />
                                </Items>
                            </telerik:RadContextMenu>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td align="left" valign="middle">
            <asp:Panel ID="AddPanel" runat="server" CssClass="combobtnstylespan" Style="margin-top: 5px;">
                <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle; ">
                    <tr>
                        <td>
                            <asp:Button ID="btnAddPatient" runat="server" CssClass="combobtnstyle" OnClick="btnAddPatient_Click"
                                Text="Add Patient" ToolTip="Add a new patient" CausesValidation="false" Style="vertical-align: middle;
                                display: inline;" TabIndex="7" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
