<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationQuestionDateTime" Codebehind="ePAInitiationQuestionDateTime.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div id="questionHolder" runat="server" class="ePAQuestionHolder">
    <div class="ePAQuestionTitleNcpdp" >
        <table width="100%">
            <tr>
                <td>
                </td>
                <td align="right">
                    <span id="questionProgress" runat="server"></span>
                </td>
            </tr>
            <tr>
                 <td colspan="2">
                    <div id="questionTitle" runat="server">
                    </div>
                </td>
            </tr>
        </table>      
    </div>
    <br />
    <div id="questionMiddle" runat="server" class="ePAQuestionMiddleNcpdp">
        <telerik:RadDatePicker ID="dateAnswer" Width= "200px" Style="vertical-align: middle;" runat="server" MinDate="1900-01-01" MaxDate="2050-12-31" ZIndex="100000001">
             <ClientEvents OnDateSelected="OnDateValuePickedFromPopUp"></ClientEvents>
        </telerik:RadDatePicker>
        <telerik:RadDateTimePicker ID="dateTimeAnswer" Width= "200px" Style="vertical-align: middle;" runat="server" MinDate="1900-01-01" MaxDate="2050-12-31" ZIndex="100000001">
            <ClientEvents OnDateSelected="OnDateValuePickedFromPopUp"></ClientEvents>
        </telerik:RadDateTimePicker>
    </div>
</div>
