<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.ProcessPatientSSO" Title="ePrescribe -- Add Patient Confirmation" Codebehind="ProcessPatientSSO.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
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
	<asp:HiddenField ID="_firstName" runat="server" />
	<asp:HiddenField ID="_middleName" runat="server" />
	<asp:HiddenField ID="_dob" runat="server" />
	<asp:HiddenField ID="_mrn" runat="server" />
	<asp:HiddenField ID="_patientGUID" runat="server" />	
	<asp:HiddenField ID="_address1" runat="server" />
	<asp:HiddenField ID="_address2" runat="server" />
	<asp:HiddenField ID="_city" runat="server" />
	<asp:HiddenField ID="_state" runat="server" />
	<asp:HiddenField ID="_zip" runat="server" />
	<asp:HiddenField ID="_phone" runat="server" />
	<asp:HiddenField ID="_mobilephone" runat="server" />
	<asp:HiddenField ID="_gender" runat="server" />
	<asp:HiddenField ID="_email" runat="server" />
	<asp:HiddenField ID="_nextPageUrl" runat="server" />
	<asp:HiddenField ID="_planid" runat="server" />
	<asp:HiddenField ID="_planname" runat="server" />
	<asp:HiddenField ID="patientdoc" runat="server" />
	<asp:HiddenField ID="_patNCPDPNo" runat="server" />
    <asp:HiddenField ID="_pharmState" runat="server" />
	
	
	<table width="100%" border="0" cellspacing="0" cellpadding="0">
	<tr class="h1title"><td></td></tr>
	<tr class="h2title">
			<td class="adminlink1">
			    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
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
									<asp:Button ID="btnReviewHistory" runat="server" CssClass="btnstyle" Enabled="False"
										Text="Review History" ToolTip="Click to see patient's medication and problem history"
										OnClick="btnReviewHistory_Click" />&nbsp;&nbsp;
									<asp:Button ID="btnSelectDx" runat="server" CssClass="btnstyle" Enabled="false" Text="Select Dx &#9658;"
										ToolTip="Click to select a diagnosis" OnClick="btnSelectDx_Click" />&nbsp;&nbsp;
									<asp:Button ID="btnFavorite" runat="server" CssClass="btnstyle" Enabled="false" Text="Select Med &#9658;&#9658;"
										ToolTip="Shows your favorite full scripts" OnClick="btnFavorite_Click" />&nbsp;&nbsp;
									<asp:Button ID="btnProviderCancel" runat="server" Text="Cancel" OnClick="btnProviderCancel_Click" CssClass="btnstyle" />
								</asp:View>
								<asp:View ID="NurseControlView" runat="server">
									<asp:Button ID="btnNurseCancel" runat="server" Text="Cancel" OnClick="btnNurseCancel_Click" CssClass="btnstyle" />
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
									<asp:TemplateField>
										<ItemTemplate>
											<input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>' />
										</ItemTemplate>
										<ItemStyle HorizontalAlign="Center" Width="30px" />
										<HeaderStyle HorizontalAlign="Center" />
									</asp:TemplateField>
									<asp:BoundField DataField="ChartID" HeaderText="MRN" SortExpression="MRN">
										<ItemStyle Width="60px" />
									</asp:BoundField>
									<asp:BoundField DataField="Name" HeaderText="Patient Name" SortExpression="Name">
										<ItemStyle Width="215px" />
									</asp:BoundField>
									<asp:BoundField DataField="DOB" HeaderText="DOB" ItemStyle-Width="90px" />									
									<asp:ButtonField CommandName="Select" />
									<asp:BoundField DataField="Phone" HeaderText="Phone Number">
										<ItemStyle Width="100px" />
									</asp:BoundField>
									<asp:BoundField DataField="Address" HeaderText="Address">
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