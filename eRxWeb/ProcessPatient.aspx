<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="True" Inherits="eRxWeb.ProcessPatient" Title="ePrescribe -- Add Patient Confirmation" Codebehind="ProcessPatient.aspx.cs" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="Joel.Net.Refresh" Namespace="Joel.Net" TagPrefix="cc1" %>
<%@ PreviousPageType VirtualPath="~/AddPatient.aspx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<cc1:Refresh ID="Refresh1" runat="server" />

	<script type="text/javascript" language="javascript">

		
		var prevRow;
		var savedClass;

		function onRowClick(row)
		{   
		
			var controlArray = new Array();
			
			controlArray[controlArray.length] = document.getElementById("<%=btnEditPatient.ClientID %>");
			controlArray[controlArray.length] = document.getElementById("<%=btnReviewHistory.ClientID %>");
			controlArray[controlArray.length] = document.getElementById("<%=btnSelectDx.ClientID %>");
			controlArray[controlArray.length] = document.getElementById("<%=btnFavorite.ClientID %>");
			
			for (var index = 0; index < controlArray.length; index++)
			{
				if (controlArray[index] != null)
				{
					controlArray[index].disabled = false;
				}
			}
			
			/*
			var editPatient = document.getElementById("<%=btnEditPatient.ClientID %>");
			if(editPatient != null)
			{
				editPatient.disabled = false;
			}
			
			var reviewHistory = document.getElementById("<%=btnReviewHistory.ClientID %>");
			if(reviewHistory != null)
			{
				reviewHistory.disabled = false;
			}
			*/
			
			if(prevRow!=null)
			{
				prevRow.className=savedClass;
			}
			savedClass=row.className;
			row.className='SelectedRow';
			prevRow=row;
			cleanWhitespace(row)
			row.firstChild.childNodes[0].checked=true;
		}
		
		function cleanWhitespace(node)
		{
			for (var x = 0; x < node.childNodes.length; x++) 
			{
				var childNode = node.childNodes[x]
				if ((childNode.nodeType == 3)&&(!/\S/.test(childNode.nodeValue))) 
				{
					// that is, if it's a whitespace text node
					node.removeChild(node.childNodes[x])
					x--
				}
				if (childNode.nodeType == 1) 
				{
					// elements can have text child nodes of their own
					cleanWhitespace(childNode)
				}
			}
		}
	</script>
	
	<!-- Hidden fields from add patient page. -->
	<asp:HiddenField ID="_lastName" runat="server" />
	<asp:HiddenField ID="_paternalName" runat="server" />
	<asp:HiddenField ID="_maternalName" runat="server" />
	<asp:HiddenField ID="_firstName" runat="server" />
	<asp:HiddenField ID="_middleName" runat="server" />
	<asp:HiddenField ID="_dob" runat="server" />
	<asp:HiddenField ID="_mrn" runat="server" />
	<asp:HiddenField ID="_address1" runat="server" />
	<asp:HiddenField ID="_address2" runat="server" />
	<asp:HiddenField ID="_city" runat="server" />
	<asp:HiddenField ID="_state" runat="server" />
	<asp:HiddenField ID="_zip" runat="server" />
	<asp:HiddenField ID="_phone" runat="server" />
	<asp:HiddenField ID="_mobilePhone" runat="server" />	
	<asp:HiddenField ID="_gender" runat="server" />
	<asp:HiddenField ID="_email" runat="server" />
	<asp:HiddenField ID="_nextPageUrl" runat="server" />
    <asp:HiddenField ID="_isPatientHistoryExcluded" runat="server" />
    <asp:HiddenField ID="_preferredLanguageID" runat="server" />
    <asp:HiddenField ID="_weight" runat="server" />
    <asp:HiddenField ID="_height" runat="server" />
    <asp:HiddenField ID="_isHealthPlanDisclosable" runat="server" />
	
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr class="h1title"><td></td></tr>
	<tr class="h2title">
			<td>
				<ePrescribe:Message ID="ucMessage" runat="server" />
			</td>
		</tr>
		<tr>
			<td>
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr class="h4title">
						<td colspan="6" style="height: 20px">
							<asp:Button ID="btnEditPatient" runat="server" CssClass="btnstyle" Enabled="False"
								Text="Edit Patient" ToolTip="Edit data related to the selected patient." OnClick="btnEditPatient_Click" />&nbsp;&nbsp;
							<asp:Button ID="btnAddPatient" runat="server" CssClass="btnstyle" Text="Add Patient"
								ToolTip="Add a new patient to the database" OnClick="btnAddPatient_Click" />&nbsp;&nbsp;
							<asp:MultiView ID="ControlMultiView" runat="server" OnLoad="ControlMultiView_Load">
								<asp:View ID="ProviderControlView" runat="server">
								    <asp:Button ID="btnProviderCancel" runat="server" Text="Back" OnClick="btnProviderCancel_Click" CssClass="btnstyle" />
									<asp:Button ID="btnReviewHistory" runat="server" CssClass="btnstyle" Enabled="False"
										Text="Review History" ToolTip="Click to see patient's medication and problem history"
										OnClick="btnReviewHistory_Click" />&nbsp;&nbsp;
									<asp:Button ID="btnSelectDx" runat="server" CssClass="btnstyle" Enabled="false" Text="Select Dx &#9658;"
										ToolTip="Click to select a diagnosis" OnClick="btnSelectDx_Click" />&nbsp;&nbsp;
									<asp:Button ID="btnFavorite" runat="server" CssClass="btnstyle" Enabled="false" Text="Select Med &#9658;&#9658;"
										ToolTip="Shows your favorite full scripts" OnClick="btnFavorite_Click" />&nbsp;&nbsp;					        		
								</asp:View>
								<asp:View ID="NurseControlView" runat="server">
									<asp:Button ID="btnNurseCancel" runat="server" Text="Back" OnClick="btnNurseCancel_Click" CssClass="btnstyle" />
								</asp:View>
							</asp:MultiView>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<table width="100%" border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
					<tr>
						<td height="330px">
							<asp:GridView ID="grdViewPatients" runat="server" AllowPaging="True" AllowSorting="True"
								AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
								EmptyDataText=" No Patient Found" DataKeyNames="PatientID" PageSize="13" OnPageIndexChanging="grdViewPatients_PageIndexChanging" OnRowDataBound="grdViewPatients_RowDataBound">
								
								<Columns>
									<asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemStyle Width="30px" HorizontalAlign="Left"></ItemStyle>
										<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
										<ItemTemplate>
											<input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>' />
										</ItemTemplate>
									</asp:TemplateField>
									<asp:BoundField ItemStyle-HorizontalAlign="Center" DataField="ChartID" HeaderText="MRN">
										<ItemStyle Width="60px" />
									</asp:BoundField>
									<asp:BoundField DataField="Name" ItemStyle-HorizontalAlign="Center" HeaderText="Patient Name">
										<ItemStyle Width="215px" />
									</asp:BoundField>
									<asp:BoundField DataField="DOB" HeaderText="DOB" ItemStyle-Width="90px" ItemStyle-HorizontalAlign="Center"/>									
									<asp:ButtonField CommandName="Select" />
									<asp:BoundField DataField="Phone" HeaderText="Phone Number" ItemStyle-HorizontalAlign="Center">
										<ItemStyle Width="100px" />
									</asp:BoundField>
									<asp:BoundField DataField="Address" HeaderText="Address" ItemStyle-HorizontalAlign="Center">
										<ItemStyle Width="270px" />
									</asp:BoundField>
								</Columns>
							</asp:GridView>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</asp:Content>