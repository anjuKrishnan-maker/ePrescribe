<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.PharmRefillReport" Title="Pharmacy Refill Report" Codebehind="PharmRefillReport.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<asp:Panel ID= "pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
		<tr>
			<td>
				<table border="1" width="100%" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
					<tr>
						<td class="Phead indnt h1title">
							Pharmacy Refill Report
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr class="h2title">
			<td>
			</td>
		</tr>
		<tr>
			<td>
				<span class="head indnt">Provider</span>
				<span class="indnt">&nbsp;</span>	
				<asp:DropDownList ID="ddlProvider" runat="server" TabIndex="1" Width="170px">
				</asp:DropDownList>
			</td>
		</tr>
		<tr class="h3title">
			<td style="height: 20px">
			    <asp:Button ID="btnClose" CssClass="btnstyle" runat="server" TabIndex="3" Text="Back" OnClick="btnClose_Click"
					CausesValidation="false" />
				<asp:Button ID="btnReport" CssClass="btnstyle" runat="server" TabIndex="2" Text="Display Report"
					OnClick="btnReport_Click" ToolTip="Click here to see the report ." />
				
			</td>
		</tr>
	</table>
    </asp:Panel>
</asp:Content>
