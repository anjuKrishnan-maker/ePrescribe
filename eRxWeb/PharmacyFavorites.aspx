<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" Title="Pharmacy Favorites" AutoEventWireup="true" Inherits="eRxWeb.PharmacyFavorites" Codebehind="PharmacyFavorites.aspx.cs" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">    
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr class="h1title">
            <td colspan="2">
                <table border="0" cellpadding="0" width="100%">
                    <tr>
                        <td colspan="2">
                            <span class="indnt Phead">Pharmacy Favorites </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="message">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />            
            </td>
        </tr>
        <tr class="h2title">
            <td style="height: 20px" colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table border="1" width="100%" align="center" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td style="vertical-align:middle" align="left">
                                        <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" PostBackUrl="~/sitemanagement.aspx" />
                                        <asp:Button ID="btnDelete" runat="server" CssClass="btnstyle" Text="Delete Selected" OnClick="btnDelete_Click" />
                                        <asp:Button ID="btnAddPharmacy" runat="server" CssClass="btnstyle" Text="Add a Pharmacy" OnClick="btnAddPharmacy_Click"/>
                                        
                                    </td>
                                    <td align="right">
                                        <table>
                                            <tr>
                                                <td style="vertical-align:middle">Show pharmacy favorites for: </td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSites" runat="server"
                                                        DataSourceID="siteDataSource" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        DataTextField="SiteName"
                                                        AllowCustomText="False" ShowToggleImage="True"
                                                        MarkFirstMatch="True" 
                                                        DataValueField="SiteID" AutoPostBack="true">
                                                    </telerik:RadComboBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="500">
                        <td>                            
                            <telerik:RadGrid ID="grdViewPharmacyFavorites" runat="server" AllowPaging="True" AutoGenerateColumns="False" Width="100%"
                                GridLines="None" PageSize="15" AllowMultiRowSelection="True" DataSourceID="pharmFavDataSource" Skin="Allscripts" EnableEmbeddedSkins="false" AllowSorting="true" OnItemDataBound="grdViewPharmacyFavorites_ItemDataBound">
                                <MasterTableView DataKeyNames="PharmacyFavoriteID, PracticeFavorite" DataSourceID="pharmFavDataSource">
                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <NoRecordsTemplate>No pharmacy favorites found</NoRecordsTemplate>
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridClientSelectColumn HeaderStyle-Width="40px"></telerik:GridClientSelectColumn>
                                        <telerik:GridBoundColumn DataField="Name" HeaderText="Pharmacy Name" SortExpression="Name" UniqueName="Name">
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Address1" HeaderText="Address" SortExpression="Address1" UniqueName="Address1"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="City" HeaderText="City" SortExpression="City" UniqueName="City"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="State" HeaderText="State" SortExpression="State" UniqueName="State"></telerik:GridBoundColumn>                                        
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <Selecting AllowRowSelect="True" />
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="pharmFavDataSource" runat="server" SelectMethod="LoadPharmacyFavorites"
                                TypeName="Allscripts.Impact.Pharmacy" OldValuesParameterFormatString="original_{0}" MaximumRowsParameterName="">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LicenseID" Type="String" />
                                    <asp:ControlParameter ControlID="ddlSites" PropertyName="SelectedValue" Name="siteID" Type="Int32" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="siteDataSource" runat="server" SelectMethod="LoadSites" TypeName="Allscripts.Impact.ApplicationLicense">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
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
            <telerik:AjaxSetting AjaxControlID="ddlSites">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewPharmacyFavorites" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnDelete">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdViewPharmacyFavorites" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">    
 
</asp:Content>
