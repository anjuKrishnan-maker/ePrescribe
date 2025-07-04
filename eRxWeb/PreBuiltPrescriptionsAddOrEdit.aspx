<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master"
    CodeBehind="PreBuiltPrescriptionsAddOrEdit.aspx.cs" Inherits="eRxWeb.PreBuiltPrescriptionsAddOrEdit" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="UrgentMessages" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
     <script src="jquery/js/jquery-1.4.4.min.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            $(document).ready(function () {
                $("#ctl00_ContentPlaceHolder1_gridGroupPrescriptions_ctl00_ctl03_ctl01_ChangePageSizeLabel").hide();
                if (document.getElementById("ctl00_ContentPlaceHolder1_divUrgentMessage").style.display == "block") {
                    $("#ctl00_ContentPlaceHolder1_divUrgentMessage > table").css("background", "#fffacd");
                    $("#ctl00_ContentPlaceHolder1_divUrgentMessage > table").css("border", "0px");
                    document.getElementById("trdivUrgentMessage").style.display = "block";
                }
                else {
                    document.getElementById("trdivUrgentMessage").style.display = "none";
                }
                $(".divRadGrid input:[type=checkbox]").change(function () {
                    CheckBoxHandle();
                });
                function CheckBoxHandle() {
                    if (($(".divRadGrid input:[type=checkbox]:checked").length < 1)) {
                        $("#ctl00_ContentPlaceHolder1_btnDeleteSelected").attr("disabled", true);
                    }
                    else {

                        $("#ctl00_ContentPlaceHolder1_btnDeleteSelected").attr("disabled", false);
                    }
                }
            });
           
        </script>
    </telerik:RadCodeBlock>
    <div style="height: 650px;">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr class="h1title">
                <td class="Phead indnt">
                    <asp:Label ID="lblheader" Style="color: White; font-weight: bold;" runat="server"
                        Text="Create Prescription Group" />
                </td>
            </tr>
            <tr id="trdivUrgentMessage" style="height:22px;width:inherit;display:none;background:#fffacd;">
                <td>
                    <div style="display: none;" id="divUrgentMessage" runat="server">
                        <ePrescribe:UrgentMessages ID="ucMessage" runat="server" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="3">
                        <tr class="h2title" align="left">
                            <td>
                                <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/PreBuiltPrescriptions.aspx"
                                    CausesValidation="false" CssClass="btnstyle" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="5">
                        <tr style="height: 20px; background-color: #FFFFFF;">
                            <td>
                                &nbsp; &nbsp; Group Name :
                                <asp:TextBox ID="txtGroupName" runat="server" with="350" Width="251px" MaxLength="50"
                                    ValidationGroup="SaveGroup"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqdfldValtxtGroupName" runat="server" ControlToValidate="txtGroupName"
                                    ValidationGroup="SaveGroup" ErrorMessage="*Required" ForeColor="Red"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="rglrExpValtxtGroupName" runat="server" ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*"
                                    ValidationGroup="SaveGroup" ErrorMessage="*Invalid group name" ControlToValidate="txtGroupName"
                                    ForeColor="Red"></asp:RegularExpressionValidator>
                                &nbsp; &nbsp; &nbsp;
                                <asp:CheckBox ID="chkboxInActive" runat="server" Text="Inactive" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnSaveGroup" CssClass="btnstyle" runat="server" Text="Save" Width="75px" OnClick="btnSaveGroup_Click"
                                    ValidationGroup="SaveGroup" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="3">
                        <tr class="h2title" align="left">
                            <td>
                                <asp:Panel ID="pnlAddDeletPrescription" Enabled="false" runat="server">
                                    &nbsp;<asp:Button ID="btnAddPrescription" runat="server" Text="Add Scripts" OnClick="btnAddPrescription_Click" Width="110px"
                                        CssClass="btnStyleOneArrow" />
                                    &nbsp;&nbsp;<asp:Button ID="btnDeleteSelected" runat="server" Text="Delete Selected"  disabled
                                        CssClass="btnstyle" OnClick="btnDeleteSelected_Click" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div class="divRadGrid">
            <telerik:RadGrid ID="gridGroupPrescriptions" runat="server" AutoGenerateColumns="false"
                EnableEmbeddedSkins="false" Skin="Allscripts" AllowSorting="true" PageSize="50" AllowPaging="true"  OnPageIndexChanged="gridGroupPrescriptions_PageIndexChanged"
                AllowMultiRowSelection="true" ShowHeader="false" OnItemCommand="gridGroupPrescriptions_ItemCommand"
                OnItemDataBound="gridGroupPrescriptions_RowDataBound">
                <MasterTableView DataKeyNames="ID,GroupID,DDI,MedicationName,Strength,StrengthUOM,RouteOfAdminCode,DosageFormCode,SIGID,SIGText,RefillQuantity,Quantity,DaysSupply,PackageSize,PackageQuantity,PackageUOM,PackageDescription,DAW"
                    AllowNaturalSort="true">
                    <NoRecordsTemplate>
                        <div>
                            <p>
                                No prescriptions added
                            </p>
                        </div>
                    </NoRecordsTemplate>
                    <HeaderStyle Font-Bold="true" />
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <div>
                                  <input type="checkbox" name="radGridCheckBox" id="radGridCheckBoxId" runat="server" />
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <div>
                                    <asp:Literal ID="litMedicationAndSig" runat="server"></asp:Literal>
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="images/Edit.gif" Text="Edit"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" UniqueName="Edit"
                            ItemStyle-Width="10%" CommandName="Edit">
                        </telerik:GridButtonColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings Scrolling-AllowScroll="true" Scrolling-ScrollHeight="500px"></ClientSettings>
                <PagerStyle Mode="NextPrevAndNumeric" PagerTextFormat="{4}" AlwaysVisible="false"  />
            </telerik:RadGrid>
        </div>
    </div>
</asp:Content>
