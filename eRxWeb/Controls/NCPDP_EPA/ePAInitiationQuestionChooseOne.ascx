<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationQuestionChooseOne" Codebehind="ePAInitiationQuestionChooseOne.ascx.cs" %>
<div id="questionHolder" runat="server" class="ePAQuestionHolder">
    <div class="ePAQuestionTitleNcpdp">
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
    <div id="questionRadios" runat="server" class="ePAQuestionMiddleListNcpdp">
    </div>
</div>
