<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationQuestionFreeText"
    CodeBehind="ePAInitiationQuestionFreeText.ascx.cs" %>
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
    <div id="questionMiddle" runat="server" class="ePAQuestionMiddleNcpdp">
    <table>
        <tr>
            <td>
                Maximum 2000 characters
            </td>
            <td align="right">
                <span id="textCount" runat="server" name="textCount">[0 of 2000]</span>
            </td>
        </tr>
        <tr>
            <td colspan ="2">
                <textarea id="questionFreeText" runat="server" rows="5" cols="50" onblur="" onfocus></textarea><br />
            </td>
        </tr>
    </table>
    </div>
</div>
