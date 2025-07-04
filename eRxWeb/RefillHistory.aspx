<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.RefillHistory" Title="Refill History" Codebehind="RefillHistory.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <script language="javascript" type="text/javascript">
 function SelectPhysician(physicianID,chkSelect,gridId,btnsubmit,pharm)
 {
 
       var ddlProviderList = document.getElementById("ctl00_ContentPlaceHolder1_ddlProviderList");
       if(ddlProviderList != null && chkSelect.checked)
       {    var i =0;
            for( i=0 ;i<ddlProviderList.length;i++)
            {
                if(ddlProviderList[i].value == physicianID)
                {
                    ddlProviderList[i].selected=true;
                    break;
                }
            }
       }
      enabledisable(gridId,btnsubmit,pharm);
       
 }
 

//EAK added for LimitTextbox
//
// Limit the text input in the specified field.
//
function LimitInput(targetField, maxLen, eventObj)
{
    var isPermittedKeystroke;
    var enteredKeystroke;
    var maximumFieldLength;
    var currentFieldLength;
    var inputAllowed = true;
    var selectionLength = 0;
    
    if (document.selection)
		selectionLength = parseInt(document.selection.createRange().text.length);
	else if (document.getSelection)
		selectionLength = parseInt(document.getSelection().length);
	
    if ( maxLen != null )
    {
     // Get the current and maximum field length
     currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        // Allow non-printing, arrow and delete keys
        enteredKeystroke = window.event ? eventObj.keyCode : eventObj.which;
        isPermittedKeystroke = ((enteredKeystroke < 32)                                // Non printing
                     ||(enteredKeystroke >= 33 && enteredKeystroke <= 40)    // Page Up, Down, Home, End, Arrow
                     ||(enteredKeystroke == 46))                            // Delete

        // Decide whether the keystroke is allowed to proceed
        if ( !isPermittedKeystroke )
        {
            if ( ( currentFieldLength - selectionLength ) >= maximumFieldLength ) 
            {
                inputAllowed = false;
                
            }
        }
    } 
    
    if (window.event)
		window.event.returnValue = inputAllowed;
	
    return (inputAllowed);
}

//
// Limit the text input in the specified field.
//

function LimitPaste(targetField, maxLen)
{
    var clipboardText;
    var resultantLength;
    var maximumFieldLength;
    var currentFieldLength;
    var pasteAllowed = true;
    var selectionLength = 0;
    
    if (document.selection)
		selectionLength = parseInt(document.selection.createRange().text.length);
	else if (document.getSelection)
		selectionLength = parseInt(document.getSelection().length);

    if ( maxLen != null )
    {
     // Get the current and maximum field length
     currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        clipboardText = window.clipboardData.getData("Text");
        resultantLength = currentFieldLength + clipboardText.length - selectionLength;
        if ( resultantLength > maximumFieldLength)
        {
            pasteAllowed = false;
        }    
    }    
    
    if (window.event)
		window.event.returnValue = pasteAllowed;
		
    return (pasteAllowed);
}

function enabledisable(gridId,btnsubmit,pharm)
{
   
    var btnSelectPharmacy = document.getElementById("ctl00_ContentPlaceHolder1_btnSelectPharmacy");
    var grid = document.getElementById(gridId);
    var btnSubmitRq = document.getElementById(btnsubmit);
   
	var hasScriptSelected = false;
    var hasPrintOnly =false;
    for(index =1 ;index<grid.rows.length;index++)
    {
        cleanWhitespace(grid.rows[index]);
        
        if( grid.rows[index].firstChild.firstChild != null &&  grid.rows[index].firstChild.firstChild.checked)
        {   
           
            hasScriptSelected = true;
            if (grid.rows[index].Pharmacy == "N")
				hasPrintOnly = true;
			break; 
        }
       
    }
    
    if(hasScriptSelected)
    {
		btnSubmitRq.disabled = false;
		btnSelectPharmacy.disabled = false;
	   if(hasPrintOnly)
		{
			//btnSelectPharmacy.disabled = true;
			btnSubmitRq.disabled = true;
		}
    }
    else
    {
		btnSelectPharmacy.disabled = true;
        btnSubmitRq.disabled = true;
    }
    
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
<table border="0" cellspacing="0" cellpadding="0" width="100%">
    
      <tr class="h1title">
         <td>
             <span class="Phead indnt">Prescription History</span> </td>
             <td  class="adminlink1">
         <asp:Literal ID="Literal1"  runat="server"></asp:Literal></td>
                
    </tr>
    <tr class="h2title">
		<td colspan="2">
			<asp:Label ID="lblMsg" runat="server" CssClass="errormsg" Text=""></asp:Label>
		</td>
	</tr>
        <tr>
            <td colspan="2">
                <table border="1" align="center" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4" width="100%">
					<tr>
						<!--     <td style="width: 753px">  -->
						<td width="100%">
							<table border="0" cellspacing="0" cellpadding="0" width="100%">
								<tr class="h4title">
									<td colspan="4">
									<asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnCancel_Click"
											CausesValidation="false" />										
										<asp:Button ID="btnSelectPharmacy" CssClass="btnstyle" runat="server" OnClick="btnSelectPharmacy_Click"
											Text="Choose Pharmacy" Enabled="False" />
											<asp:Button ID="btnSubmitRefillRequest" runat="server" CssClass="btnstyle" OnClick="SubmitRefillRequest_Click"
                                                    Text="Renew Prescripton" Enabled="False" />
										<!--JJ 134-->
										
									</td>
								</tr>
							</table>
						</td>
					</tr>
                    <tr>
                        <td  >
                            <asp:GridView ID="grdReviewHistory" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
                                 DataSourceID="RxHistoryObjDataSource" EmptyDataText="There is no Rx History for this patient."
                                 DataKeyNames="RxID,ProviderID,Pharmacy,ControlledSubstanceCode,Status" OnRowDataBound="grdReviewHistory_RowDataBound" OnRowCreated="grdReviewHistory_RowCreated" OnPageIndexChanging="grdReviewHistory_PageIndexChanging">
                               
                                <Columns>
                                    <asp:TemplateField>
                                    <ItemTemplate>
                                    <asp:CheckBox  runat="server" ID="Select"  EnableViewState="true" />
                                    <ajaxToolkit:MutuallyExclusiveCheckBoxExtender ID="SelectEx" TargetControlID="Select" Key="Prescription" runat="server" />
                                    </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                
                                    <asp:BoundField DataField="RxDate" HeaderText="Rx Date" SortExpression="RxDate">
                                        
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Dx" HeaderText="Diagnosis">
                                        
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Prescription" HeaderText="Medication And Sig" SortExpression="Prescription"  />
                                </Columns>
                                
                            </asp:GridView>
                            <asp:ObjectDataSource ID="RxHistoryObjDataSource" runat="server" SelectMethod="ChGetPatReviewHistory"
                                TypeName="Allscripts.Impact.Prescription" OldValuesParameterFormatString="original_{0}" OnSelected="RxHistoryObjDataSource_Selected">
                                <SelectParameters>
                                    <asp:SessionParameter Name="PatientId" SessionField="PATIENTID" Type="String" />
                                    <asp:SessionParameter Name="LicenseId" SessionField="LICENSEID" Type="String" DefaultValue="40D3B8CC-C765-4B39-8AF0-A75A389E4E6A" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            </td>
                    </tr>
                    <tr>
                        <td style="width: 753px">
                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
                               
								<asp:Panel ID="PnlMed" runat="server" Visible="false" Width="100%">
									<tr class="indnt" valign="top">
										<td style="width: 222px">
											<asp:Label ID="lblMed" runat="server" Text="Non-Cheetah Medication"></asp:Label>
										</td>
										<td valign="top">
											<asp:TextBox CssClass="txtboxcolor" ID="txtMed" runat="server" Height="79px" TextMode="MultiLine" Width="352px"></asp:TextBox>
											 <span class="smalltext">(Max. 50 characters allowed)</span>
											<br />
											<asp:RequiredFieldValidator
                                                ID="RequiredFieldValidator1" runat="server" ErrorMessage="RequiredFieldValidator" ControlToValidate="txtMed" Text="Non-Cheetah medication is an essential field. Please enter a medication/full script."></asp:RequiredFieldValidator>
										</td>
									</tr>
								</asp:Panel>                               
                                <tr class="indnt">
                                    <td style="width: 222px" >
                                        <!--StartFragment -->
                                        <asp:Label ID="lblSelPatDoc" runat="server" Text="Select Patient's Doctor"></asp:Label>
                                    </td>
                                    <td >
                                        <asp:DropDownList ID="ddlProviderList" runat="server" Width="248px" DataSourceID="ProviderListObjDataSource" DataTextField="ProviderName" DataValueField="ProviderId">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="ProviderListObjDataSource" runat="server" ConvertNullToDBNull=False
                                            SelectMethod="ListAll" TypeName="Allscripts.Impact.Provider" OldValuesParameterFormatString="original_{0}">
                                            <SelectParameters>
                                                <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </td>
                                </tr>
                                <tr class="indnt">
                                    <td style="width: 222px" >
                                        <!--StartFragment -->
                                        <asp:Label ID="lblPatComments" runat="server" Text="Patient's requests or comments"></asp:Label>                                        
                                    </td>
                                    <td >
                                        <asp:TextBox ID="txtPatientComments" runat="server" TextMode="MultiLine" Width="352px" Height="79px"></asp:TextBox>
                                         <asp:Label ID="lblMaxChar" CssClass="smalltext" runat="server" Text="(Max. 50 characters allowed)"></asp:Label>
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
