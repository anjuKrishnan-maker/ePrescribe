<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.SelfReportedMed" Title="Patient Self Reported Medication" Codebehind="SelfReportedMed.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<script language="javascript" type="text/javascript">
    var prevRow;

    function onRowClick(row)
    {
        if (prevRow == null || prevRow != row)
        {	
		    prevRow=row;
		    
		    if (row.childNodes != null)
		    {
	            if (row.childNodes[0].childNodes[0] != null)
	            {
	                row.childNodes[0].childNodes[0].checked = true;
		        }
		    }
		    
            var imgSaveSRM = document.getElementById("ctl00_ContentPlaceHolder1_btnSave");
            
	        if(imgSaveSRM != null)
	        {
		        imgSaveSRM.disabled = false;
        	}
        	
        	var imgSaveAndAddAnother = document.getElementById("ctl00_ContentPlaceHolder1_btnSaveAndAddAnother");
	        if(imgSaveAndAddAnother != null)
	        {
		        imgSaveAndAddAnother.disabled = false;
        	}
	    }
    }

    function RowSelected(sender, eventArgs) {
        var grid = sender;
        var rowIndex = eventArgs.get_itemIndexHierarchical();
        var masterTable = grid.get_masterTableView();
        var gridRow = masterTable.get_dataItems()[rowIndex].get_element();
        var inputElements = gridRow.getElementsByTagName("input");

        for (i = 0; i < inputElements.length; i++) {
            if (inputElements[i].type == "radio") {
                inputElements[i].checked = true;
            }
        }
    }
</script>
    <table width="100%" cellspacing="0" border="0" cellpadding="0">
        <tr class="h1title" valign="middle">
            <td colspan="2">
                <asp:Panel ID="pnlSearchSelfReportedMed" runat="server" DefaultButton="btnSearch">
                    <table border="0" cellpadding="0">
                        <tr>
                            <td>
                                <span class="indnt Phead">Choose Self Reported Medication </span>
                            </td>
                            <td style="padding-left:5px">    
                                <asp:TextBox autocomplete="off" MaxLength="50" ID="txtSearchMed" CssClass="searchTextBox"
                                    Width="185px" ToolTip="Enter Partial or Full Medication" runat="server"></asp:TextBox>
                                <ajaxToolkit:AutoCompleteExtender ID="aceMed" runat="server" TargetControlID="txtSearchMed"
                                    ServiceMethod="queryMeds" ServicePath="erxnowmed.asmx" CompletionInterval="1000"
                                    MinimumPrefixLength="3" EnableCaching="true">
                                </ajaxToolkit:AutoCompleteExtender>
                            </td>
                            <td style="padding-left:5px">
                                <asp:Button ID="btnSearch" ToolTip="Shows all medications matching the text entered in the Search field. Enter partial or full medication name in the text-box."
                                    runat="server" Text="Search" CssClass="btnstyle" Width="80px" OnClick="btnSearch_Click" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="message">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />            
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" cellspacing="0">
                    <tr>
                        <td style="height: 14px">
                            <table width="100%" cellspacing="0">
                                <tr class="h4title">
                                    <td colspan="5">
                                        <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" CausesValidation="false" OnClick="btnBack_Click" />&nbsp;&nbsp;
                                        <asp:Button ID="btnSave" runat="server" CssClass="btnstyle"
                                            Text="Save and Exit" Enabled="False" ToolTip="Save selected Self Reported Med and navigate back to Review History page" OnClick="btnSave_Click" />&nbsp;&nbsp;
                                        <asp:Button ID="btnSaveAndAddAnother" runat="server" CssClass="btnstyle"
                                            Text="Save and Add Another" Enabled="False" ToolTip="Save selected Self Reported Med and add another one" OnClick="btnSaveAndAddAnother_Click" />                                        
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr style="height:500px">
                        <td>
                            <telerik:RadGrid ID="grdViewMed" runat="server" AllowPaging="True" AllowSorting="False" AutoGenerateColumns="False" Width="100%"
                                GridLines="None" PageSize="15" EnableViewState="true" AllowMultiRowSelection="false"
                                 Skin="Allscripts" EnableEmbeddedSkins="false" OnItemDataBound="grdViewMed_ItemDataBound">
                                <MasterTableView DataKeyNames="DDI,Name,Strength,StrengthUOM,DosageForm,RouteofAdmin,RouteofAdminCode">
                                    <HeaderStyle Font-Bold="true" />
                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <NoRecordsTemplate>No medications found</NoRecordsTemplate>
                                    <Columns>
                                        <telerik:GridTemplateColumn UniqueName="RadioSelectColumn">
                                            <ItemStyle HorizontalAlign="center" Width="30px" />
                                            <ItemTemplate>
                                               <eRxCustom:GroupedRadioButton ID="radioRowSelect" runat="server" 
                                                    AutoPostBack="true"  
                                                    GroupName="SelectedItem" 
                                                    OnCheckedChanged="gridRadioSelect_CheckedChanged" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Drug Name" HeaderStyle-HorizontalAlign="left" SortExpression="Name">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Strength" HeaderText="Strength" HeaderStyle-HorizontalAlign="left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="StrengthUOM" HeaderText="Unit" HeaderStyle-HorizontalAlign="left">
                                        </telerik:GridBoundColumn>                                        
                                        <telerik:GridBoundColumn DataField="DosageForm" HeaderText="Dosage Form" HeaderStyle-HorizontalAlign="left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="RouteofAdmin" HeaderText="Route" HeaderStyle-HorizontalAlign="left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="RouteofAdminCode" HeaderText="RouteofAdminCode"  visible ="false" HeaderStyle-HorizontalAlign="left">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn HeaderText="SIG" HeaderStyle-HorizontalAlign="left">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSIG" runat="server" Width="375px" MaxLength="140"></asp:TextBox>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Selecting AllowRowSelect="True" />
                                    <ClientEvents OnRowSelected="RowSelected" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="medDataSource" runat="server" SelectMethod="SearchForDrugName"
                                TypeName="Allscripts.Impact.Medication" OldValuesParameterFormatString="original_{0}"
                                OnSelecting="medDataSource_Selecting" MaximumRowsParameterName="">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtSearchMed" ConvertEmptyStringToNull="False" Name="drugName"
                                        PropertyName="Text" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>    
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdViewMed">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewMed" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSearch">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewMed"  />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">   
</asp:Content>