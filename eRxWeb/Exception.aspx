<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.Exception1" Title="Exception" Codebehind="Exception.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<script language="javascript" type="text/javascript">
// <!CDATA[


// ]]>
</script>

    <div>
        <table width="853px" cellspacing="0" border="0">
      
       
                <tr class="h1title">
                <td colspan="2" style="height: 19px">
                <span runat="server" id="heading" class="Phead">Exception </span> 
                               
                </td>
                </tr>
                <tr class="h4title">
                    <td colspan="2" style="height: 16px">
                        &nbsp;<asp:Button ID="btnBack" runat="server" CssClass="btnstyle" OnClick="btnBack_Click"
                                                    Text="Back" Visible="False" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%" border="1" cellspacing="0" bordercolor="#b5c4c4">
                            <tr>
                                <td>
                                    <table width="100%" cellspacing="0">
                                        <tr class="h3title">
                                            <td class="adminlink1" colspan="3"></td>
                                        </tr>
                                        <tr>
                                            <td style="height: 325px; width: 115px;">
                                                &nbsp;</td>
                                            <td width="70%" style="height: 325px">
                                                <table width="100%" cellspacing="0">
                                                    <tr>
                                                        <td colspan="3">
                                                            <asp:Label ID="lblErrMsg" runat="server" Height="228px" Width="526px" Font-Bold="True" Font-Size="Larger">We apologize, but a serious error has occurred.  Please logout and log back on.</asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3">
                                                            <asp:Label ID="lblErrorID" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 228px; height: 21px">
                                                            &nbsp;</td>
                                                        <td style="width: 1px; height: 21px">
                                                            </td>
                                                        <td style="height: 21px">
                                                            &nbsp;</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 228px; height: 21px;">
                                                            &nbsp;</td>
                                                        <td style="width: 1px; height: 21px;">
                                                        </td>
                                                        <td style="height: 21px">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                               </td>
                                            <td style="height: 325px; width: 100px;">
                                            </td>
                                        </tr>
                                        <tr >
                                            <td colspan="3" >
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                
            </table>
        </div>

  <script type="text/javascript">
      if (window.parent.setException) {
          window.parent.setException(true);
      }
  </script>
    </asp:Content>